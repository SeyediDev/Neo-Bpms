using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource(ILogger logger, 
    ADODotNetDatabaseDataSourceDefinition definition,
    AdoDotNetDatabaseConnection connection,
    AuditTrail auditTrail)
    : DataSource(definition, connection, auditTrail)
{
    protected Dictionary<string, EntityField> _fieldInfos;
    protected DbDataReader DataReader { get; set; }

    public ADODotNetDatabaseDataSourceDefinition Definition => DataSrcDefinition as ADODotNetDatabaseDataSourceDefinition;

    public override bool InsertXML(string commandText, string xmlValueDef, SqlXml xmlData)
    {
        return false;
    }

    protected abstract DbCommand OpenCommand(string command, DbConnection dbConnection);

    public override string GenerateQuery()
    {
        SqlCommand = GenerateQuery(Connection, this, true);
        //set breakpoint here to see queries
        return SqlCommand;
    }

    public override bool SaveFile(string guid, byte[] fileInfoData)
    {
        if (Connection is not AdoDotNetDatabaseConnection con)
        {
            return false;
        }

        _ = Connection.Open(DataSrcDefinition.name, AuditTrail?.DataTransaction);
        if (con.dbConnection == null || con.dbConnection.State != ConnectionState.Open)
        {
            return false;
        }

        bool ret = SaveFileData(guid, fileInfoData, con.dbConnection);
        if (!ret)
        {
            SetExceptionIfExists(logger, "1210", Connection.Exceptions,
                new DataLayerException(DataEngineException.DefaultQueryFailed,
                    "error in command " + SqlCommand));
        }

        return ret;
    }

    protected virtual bool SaveFileData(string guid, byte[] fileInfoData, DbConnection bnConnection)
    {
        return false;
    }

    public sealed override bool openForRead()
    {
        if (IsOpen)
        {
            return false;
        }

        if (string.IsNullOrEmpty(SqlCommand))
        {
            _ = GenerateQuery();
        }

        return openForRead(SqlCommand);
    }

    public override async Task<bool> OpenForReadAsync()
    {
        var r = openForRead();
        await Task.CompletedTask;
        return r;
    }

    protected sealed override bool openForRead(string selectCommand)
    {
        //Transaction?.RunCommands();//todo
        SetQueryCommand(selectCommand);
        AdoDotNetDatabaseConnection con = Connection as AdoDotNetDatabaseConnection;
        _ = Connection.Open(DataSrcDefinition.name, AuditTrail?.DataTransaction);
        if (con?.dbConnection == null || con.dbConnection.State != ConnectionState.Open)
        {
            return false;
        }

        using (DbCommand oleSelectCommand = OpenCommand(selectCommand, con.dbConnection))
        {
            oleSelectCommand.CommandTimeout = 500;

            if (!ExecuteReader(oleSelectCommand, out int recordsAffected))
            {
                return false;
            }

            RecordsAffected = recordsAffected;
        }

        IsOpen = true;

        if (Definition.connection.MetaDataCouldBeExtracted() &&
            Definition.bExtractMetaData)
        {
            _ = extractEntityMetaData(logger);
        }
        else if (DataSrcDefinition.Entity == null)
        {
            _ = Close();
            return false;
        }

        return true;
    }

    protected sealed override bool openForUpdate(string selectCommand, bool runCommand = false, List<object> parameters = null)
    {
        if (Connection == null)
        {
            return false;
        }

        Connection.Exceptions = null;
        Exceptions = null;
        SetQueryCommand(selectCommand);
        if (Connection is not AdoDotNetDatabaseConnection con)
        {
            return false;
        }

        _ = Connection.Open(DataSrcDefinition.name, AuditTrail?.DataTransaction);
        if (con.dbConnection == null || con.dbConnection.State != ConnectionState.Open)
        {
            return false;
        }

        if (AuditTrail?.Batch == null || runCommand)
        {
            SqlCommand = selectCommand;
        }
        else
        {
            AuditTrail?.Batch.Append(selectCommand);
        }

        try
        {
            if (AuditTrail?.Batch != null && !runCommand)
            {
                RecordsAffected += (int)AuditTrail?.Batch.CheckAndRunCommands();
            }
            else
            {
                RecordsAffected = ExecuteNonQuery(out bool succeed, parameters);
                if (!succeed)
                {
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            RecordsAffected = 0;
            AddException(logger, "1202", new DataLayerException(e));
            return false;
        }

        if (Exceptions == null)
        {
            SetExceptionIfExists(logger, "1201", Connection.Exceptions);
        }

        IsOpen = true;
        return true;
    }

    public override bool Close()
    {
        SqlCommand = null;
        CloseDataReader();

        Connection?.Try(() => Connection?.Close());

        Connection = null;

        bool b = IsOpen;
        IsOpen = false;
        return b;
    }

    private void CloseDataReader()
    {
        if (DataReader != null)
        {
            DataReader?.Try(() =>
            {
                if (!DataReader.IsClosed)
                {
                    DataReader?.Close();
                }
            });
            DataReader?.Try(() => DataReader?.Dispose());
            DataReader = null;
        }
    }

    #region write

    protected virtual string GetParamDeclarationText()
    {
        return "DECLARE ";
    }

    protected virtual string GetParamName(string name)
    {
        return name;
    }

    #endregion
}
