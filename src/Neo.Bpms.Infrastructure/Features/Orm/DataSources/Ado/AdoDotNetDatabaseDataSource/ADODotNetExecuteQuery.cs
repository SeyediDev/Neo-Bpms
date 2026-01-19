using System.Data.Common;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    protected virtual bool ExecuteReader(DbCommand oleSelectCommand, out DbDataReader dataReader,
        out int recordsAffected)
    {
        try
        {
            var con = Connection as AdoDotNetDatabaseConnection;
            ExecuteReader(oleSelectCommand, out dataReader, out recordsAffected, con);
        }
        catch (Exception e)
        {
            AddException(logger, "1204", new DataLayerException(e));
            dataReader = null;
            recordsAffected = -1;
            return false;
        }
        return true;
    }
    protected virtual bool ExecuteReader(DbCommand oleSelectCommand, out int recordsAffected)
    {
        try
        {
            var con = Connection as AdoDotNetDatabaseConnection;
            ExecuteReader(oleSelectCommand, out recordsAffected, con);
        }
        catch (Exception e)
        {
            AddException(logger, "1204", new DataLayerException(e));
            DataReader = null;
            recordsAffected = -1;
            return false;
        }
        return true;
    }
    protected virtual int ExecuteNonQuery(out bool succeed, List<object> parameters = null)
    {
        try
        {
            if (Connection is not AdoDotNetDatabaseConnection con)
            {
                succeed = false;
                return 0;
            }
            return ExecuteNonQuery(out succeed, parameters, con);
        }
        catch (Exception e)
        {
            AddException(logger, "1203", new DataLayerException(e));
            succeed = false;
            return -1;
        }
    }

    protected int ExecuteNonQuery(out bool succeed, List<object> parameters, AdoDotNetDatabaseConnection con)
    {
        var t0 = DateTime.UtcNow;
        int rowAffected;
        using (var oleSelectCommand = OpenCommand(SqlCommand, con.dbConnection))
        {
            oleSelectCommand.CommandTimeout = 500;
            if (parameters != null)
                AddParametersToCommand(oleSelectCommand, parameters);
            rowAffected = oleSelectCommand.ExecuteNonQuery();
        }
        succeed = true;
        return rowAffected;
    }

    protected void ExecuteReader(DbCommand oleSelectCommand, out DbDataReader dataReader, out int recordsAffected, AdoDotNetDatabaseConnection con)
    {
        if (con == null) { recordsAffected = 0; dataReader = null; return; }
        var t0 = DateTime.UtcNow;
        dataReader = oleSelectCommand.ExecuteReader();
        recordsAffected = DataReader.RecordsAffected;
    }
    protected void ExecuteReader(DbCommand oleSelectCommand, out int recordsAffected, AdoDotNetDatabaseConnection con)
    {
        if (con == null) { recordsAffected = 0; DataReader = null; return; }
        var t0 = DateTime.UtcNow;
        DataReader = oleSelectCommand.ExecuteReader();
        recordsAffected = DataReader.RecordsAffected;
    }

    protected static void AddParametersToCommand(DbCommand dbCommand, List<object> parameters)
    {
        if (dbCommand == null || parameters == null) return;
        foreach (var parameter in parameters)
        {
            dbCommand.Parameters.Add(parameter);
        }
    }

}
