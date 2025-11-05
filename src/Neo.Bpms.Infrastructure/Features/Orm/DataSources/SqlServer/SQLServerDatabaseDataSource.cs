using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using Neo.Bpms.Engine.Data.ADODotNet;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;
using Microsoft.Data.SqlClient;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.SqlServer;

public class SqlServerDatabaseDataSource(ILogger logger,
    ADODotNetDatabaseDataSourceDefinition definition, AdoDotNetDatabaseConnection connection, AuditTrail auditTrail) 
    : AdoDotNetDatabaseDataSource(logger, definition, connection, auditTrail)
{
    private readonly ILogger _logger = logger;
    protected override DbCommand OpenCommand(string command, DbConnection dbConnection)
    {
        if (dbConnection is not SqlConnection) return null;
        return new SqlCommand(command, (SqlConnection)dbConnection);
    }

    public override bool InsertXML(string commandText, string xmlValueDef, SqlXml xmlData)
    {
        var cmd = new SqlCommand(commandText,
            (Connection as AdoDotNetDatabaseConnection)?.dbConnection as SqlConnection);
        cmd.Parameters.Add(
            new SqlParameter(xmlValueDef, SqlDbType.Xml)
            {
                Direction = ParameterDirection.Input,
                Value = xmlData
            });
        var ret = cmd.ExecuteNonQuery() != 0;
        cmd.Dispose();
        return ret;
    }

    protected override bool SaveFileData(string guid, byte[] fileInfoData, DbConnection con)
    {
        SqlCommand = "INSERT INTO FileData (Id,Data) Values('" + guid + "',@File)";
        var command = new SqlCommand(SqlCommand, con as SqlConnection);
        command.Parameters.Add("@File", SqlDbType.VarBinary, fileInfoData.Length).Value = fileInfoData;
        var b = command.ExecuteNonQuery();
        var ret = b != 0 && b != -1;
        command.Dispose();
        return ret;
    }

    public override SqlXml GetSqlXml(int i)
    {
        return ((SqlDataReader)DataReader).GetSqlXml(i);
    }

    protected override string SetNoCountOn => "SET NOCOUNT ON;";

    protected override string GetParamDeclarationText()
    {
        return "declare ";
    }

    protected override string GetParamName(string name)
    {
        return "@" + name;
    }

    protected override string GetFieldType(Type type)
    {
        if (type == null) return "BIGINT";
        if (type == typeof(string))
            return "NVARCHAR(MAX)";
        if (type == typeof(long) || type.IsEnum || type == typeof(TimeSpan))
            return "BIGINT";
        if (type == typeof(int))
            return "INT";
        if (type == typeof(short) || type == typeof(char))
            return "SMALLINT";
        if (type == typeof(bool))
            return "BIT";
        if (type == typeof(DateTime))
            return "DATETIME";
        if (type == typeof(double))
            return "FLOAT";
        return "BIGINT";
    }

    public override string GetFieldType(TVariableTypes fieldType)
    {
        switch (fieldType)
        {
            //case TVariableTypes.varBOOL0:				return "bit";
            case TVariableTypes.BOOL:
                return "bit";
            case TVariableTypes.Char:
                return "smallint";
            case TVariableTypes.UChar:
                return "smallint";
            case TVariableTypes.Short:
                return "smallint";
            case TVariableTypes.Int:
                return "int";
            case TVariableTypes.Long:
                return "bigint";
            case TVariableTypes.ULong:
                return "bigint";
            case TVariableTypes.Decimal:
                return "money";
            case TVariableTypes.StringListItem:
                return "bigint";
            case TVariableTypes.StringListBitMask:
                return "bigint";
            case TVariableTypes.DayHourMinute:
                return "bigint";
            case TVariableTypes.DoubleMinuteSecond:
                return "bigint";
            case TVariableTypes.Association:
                return "bigint";
            case TVariableTypes.UShort:
                return "int";
            case TVariableTypes.HourMinute:
                return "int";
            case TVariableTypes.Double:
                return "float";
            case TVariableTypes.DurHourMinute:
                return "float";
            case TVariableTypes.DateTime:
                return "datetime";
            case TVariableTypes.Date:
                return "datetime";
            case TVariableTypes.ByteArray:
                return "varbinary";
            case TVariableTypes.String:
            case TVariableTypes.DateStr:
            default:
                return "nvarchar";
        }
    }

    public override string GetFullFieldType(string fieldType, int maxLen)
    {
        var sqlFieldType = fieldType;
        if (fieldType is "nvarchar" or "varchar" or "varbinary")
        {
            var length = maxLen;
            if (length <= 0 || length >= 4000)
                sqlFieldType += "(max)";
            else
                sqlFieldType += "(" + length + ")";
        }

        return sqlFieldType;
    }

    public override string GetFullFieldType(TVariableTypes fieldType, int maxLen)
    {
        var sqlFieldType = GetFieldType(fieldType);
        switch (fieldType)
        {
            case TVariableTypes.BOOL:
                break;
            case TVariableTypes.Char:
                break;
            case TVariableTypes.UChar:
                break;
            case TVariableTypes.Short:
                break;
            case TVariableTypes.Int:
                break;
            case TVariableTypes.Long:
                break;
            case TVariableTypes.ULong:
                break;
            case TVariableTypes.Decimal:
                break;
            case TVariableTypes.StringListItem:
                break;
            case TVariableTypes.StringListBitMask:
                break;
            case TVariableTypes.DayHourMinute:
                break;
            case TVariableTypes.DoubleMinuteSecond:
                break;
            case TVariableTypes.Association:
                break;
            case TVariableTypes.UShort:
                break;
            case TVariableTypes.HourMinute:
                break;
            case TVariableTypes.Double:
                break;
            case TVariableTypes.DurHourMinute:
                break;
            case TVariableTypes.DateTime:
                break;
            case TVariableTypes.Date:
                break;
            //case TVariableTypes.String:
            //case TVariableTypes.PW:
            //case TVariableTypes.DateStr:
            //case TVariableTypes.ByteArray:
            default:
                {
                    var length = maxLen;
                    if (length <= 0 || length == 4000)
                        sqlFieldType += "(max)";
                    else
                        sqlFieldType += "(" + length + ")";
                    break;
                }
        }

        return sqlFieldType;
    }

    protected override string ISNULL_Command()
    {
        return "ISNULL";
    }

    public override bool BulkInsert<T>(IEnumerable<T> objects)
    {
        Exceptions = null;
        var table = new DataTable(DataSrcDefinition.name);
        foreach (var fld in Fields)
        {
            var type = DataSrcDefinition?.Entity?.GetField(fld.Key)?.CSharpType;
            if (type != null)
                table.Columns.Add(new DataColumn(fld.Key, type));
        }

        const int jump = 1000000;
        var objectsCount = objects.Count();
        if (objectsCount < jump)
            BulkInsertIteration(table, objects);
        else
        {
            for (var rowIndex = 0; rowIndex < objectsCount; rowIndex += jump)
            {
                table.Rows.Clear();
                var list = objects.Skip(rowIndex).Take(Math.Min(jump, objectsCount));
                BulkInsertIteration(table, list);
            }
        }

        return Exceptions == null;
    }

    private void BulkInsertIteration<T>(DataTable table, IEnumerable<T> list)
    {
        foreach (var obj in list)
        {
            object[] values = new object[Fields.Count];
            var i = 0;
            foreach (var fld in Fields)
            {
                values[i] = GetField(obj, fld.Value.overFieldName ?? fld.Value.fieldName);
                if (values[i] is DateTime time && time == DateTime.MinValue)
                    values[i] = null;
                i++;
            }

            table.Rows.Add(values);
        }

        using var bulkCopy = new SqlBulkCopy((SqlConnection)((SqlServerDatabaseConnection)Connection).dbConnection);
        bulkCopy.BulkCopyTimeout = 600; // in seconds
        foreach (var fld in Fields)
        {
            bulkCopy.ColumnMappings.Add(fld.Key, fld.Key);
        }

        bulkCopy.DestinationTableName = DataSrcDefinition.name;
        //lock (string.Intern($"ParallelBulkInsertInTable{Name}"))
        {
            bulkCopy.WriteToServer(table);
        }
        bulkCopy.Close();
    }

    protected override int ExecuteNonQuery(out bool succeed, List<object> parameters = null)
    {
        succeed = false;
        if (Connection is not AdoDotNetDatabaseConnection con) return 0;
        var retries = 10;
        while (retries > 0)
        {
            try
            {
                return ExecuteNonQuery(out succeed, parameters, con);
            }
            catch (SqlException exception)
            {
                // exception is a deadlock
                if (exception.Number == 1205)
                {
                    // Delay processing to allow retry. 
                    Thread.Sleep(500);
                    retries--;
                    if (retries == 0)
                        AddException("12.3.3.1", exception);
                }
                // exception is not a deadlock
                else
                {
                    AddException("12.3.3.2", exception);
                    break;
                }
            }
            catch (Exception e)
            {
                AddException(_logger, "12.3.3.3", new DataLayerException(e));
                break;
            }
        }

        return -1;
    }

    protected override bool ExecuteReader(DbCommand oleSelectCommand,
        out DbDataReader dataReader, out int recordsAffected)
    {
        var con = Connection as AdoDotNetDatabaseConnection;
        var retries = 10;
        while (retries > 0)
        {
            try
            {
                ExecuteReader(oleSelectCommand, out dataReader, out recordsAffected, con);
                return true;
            }
            catch (SqlException exception)
            {
                // exception is a deadlock
                if (exception.Number == 1205)
                {
                    // Delay processing to allow retry. 
                    Thread.Sleep(500);
                    retries--;
                    if (retries == 0)
                        AddException("12.3.4.1", exception);
                }
                // exception is not a deadlock
                else
                {
                    AddException("12.3.4.2", exception);
                    break;
                }
            }
            catch (Exception e)
            {
                AddException(_logger, "12.3.4.3", new DataLayerException(e));
                break;
            }
        }

        dataReader = null;
        recordsAffected = -1;
        return false;
    }

    protected override bool ReadFromDataReader()
    {
        var retries = 10;
        while (retries > 0)
        {
            try
            {
                if (DataReader != null && DataReader.Read())
                    return true;
                break;
            }
            catch (SqlException exception)
            {
                // exception is a deadlock
                if (exception.Number == 1205)
                {
                    // Delay processing to allow retry. 
                    Thread.Sleep(500);
                    retries--;
                    if (retries == 0)
                        AddException("12.3.5.1", exception);
                }
                // exception is not a deadlock
                else
                {
                    AddException("12.3.5.2", exception);
                    break;
                }
            }
            catch (Exception e)
            {
                AddException(_logger, "12.3.5.3", new DataLayerException(e));
                break;
            }
        }

        return false;
    }

    private void AddException(string errorCode, SqlException exception)
    {
        AddException(_logger, errorCode,
            SqlServerDatabaseConnection.CreateSqlException(_logger, exception.Message, exception.Errors, exception.Source));
    }
}
