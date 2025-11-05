using System.Data.Common;
using System.Data.OracleClient;
using Neo.Bpms.Engine.Data.ADODotNet;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Oracle;

public class OracleDatabaseDataSource(ILogger logger,
    OracleDatabaseDataSourceDefinition definition,
    OracleDatabaseConnection connection, AuditTrail auditTrail) 
    : AdoDotNetDatabaseDataSource(logger, definition, connection, auditTrail)
{
    protected override DbCommand OpenCommand(string command, DbConnection dbConnection)
    {
        var oracleConnection = dbConnection as OracleConnection;//todo : Obsolete class is used 
        return oracleConnection == null
            ? null
            : new OracleCommand(command, oracleConnection);//todo : Obsolete constructor is used 
    }

    protected override bool SupportTopRowsInQueryCommands()
    {
        return false;
    }

    public override string GetFieldType(TVariableTypes fieldType)
    {
        switch (fieldType)
        {
            //case TVariableTypes.varBOOL0:				return "NUMBER";
            case TVariableTypes.BOOL: return "NUMBER";
            case TVariableTypes.Char: return "NUMBER";
            case TVariableTypes.UChar: return "NUMBER";
            case TVariableTypes.Short: return "NUMBER";
            case TVariableTypes.Int: return "NUMBER";
            case TVariableTypes.Long: return "NUMBER";
            case TVariableTypes.Decimal: return "NUMBER";
            case TVariableTypes.ULong: return "NUMBER";
            case TVariableTypes.StringListItem: return "NUMBER";
            case TVariableTypes.StringListBitMask: return "NUMBER";
            case TVariableTypes.DayHourMinute: return "NUMBER";
            case TVariableTypes.DoubleMinuteSecond: return "NUMBER";
            case TVariableTypes.Association: return "NUMBER";
            case TVariableTypes.UShort: return "NUMBER";
            case TVariableTypes.HourMinute: return "NUMBER";
            case TVariableTypes.Double: return "FLOAT";
            case TVariableTypes.DurHourMinute: return "FLOAT";
            case TVariableTypes.String: return "VARCHAR2";
            case TVariableTypes.DateStr: return "VARCHAR2";
            case TVariableTypes.DateTime: return "DATE";
            case TVariableTypes.Date: return "DATE";
            case TVariableTypes.ByteArray: return "RAW";
            default: return "VARCHAR2";
        }
    }

    public override string GetFullFieldType(string fieldType, int maxLen)
    {
        var sqlFieldType = fieldType;
        if (fieldType == "VARCHAR2" || fieldType == "VARCHAR")
        {
            var length = maxLen;
            if (length <= 0 || length >= 4000)
                sqlFieldType += "(4000)";
            else
                sqlFieldType += "(" + length + ")";
        }

        return sqlFieldType;
    }

    protected override string GetFieldType(Type type)
    {
        if (type == null) return "NUMBER";
        if (type == typeof(string))
            return "VARCHAR2(4000)";
        if (type == typeof(long) || type.IsEnum || type == typeof(TimeSpan))
            return "NUMBER";
        if (type == typeof(int))
            return "NUMBER";
        if (type == typeof(short))
            return "NUMBER(5)";
        if (type == typeof(char))
            return "NUMBER(3)";
        if (type == typeof(bool))
            return "NUMBER(1)";
        if (type == typeof(DateTime))
            return "DATE";
        if (type == typeof(double))
            return "FLOAT";
        return "NUMBER";
    }

    public override string GetFullFieldType(TVariableTypes fieldType, int maxLen)
    {
        var sqlFieldType = GetFieldType(fieldType);
        switch (fieldType)
        {
            case TVariableTypes.BOOL:
                sqlFieldType += "(1)";
                break;
            case TVariableTypes.Char:
                sqlFieldType += "(3)";
                break;
            case TVariableTypes.UChar:
                sqlFieldType += "(3)";
                break;
            case TVariableTypes.Short:
                sqlFieldType += "(5)";
                break;
            case TVariableTypes.Int: break;
            case TVariableTypes.Long: break;
            case TVariableTypes.ULong: break;
            case TVariableTypes.Decimal: break;
            case TVariableTypes.StringListItem: break;
            case TVariableTypes.StringListBitMask: break;
            case TVariableTypes.DayHourMinute: break;
            case TVariableTypes.DoubleMinuteSecond: break;
            case TVariableTypes.Association: break;
            case TVariableTypes.UShort: break;
            case TVariableTypes.HourMinute: break;
            case TVariableTypes.Double: break;
            case TVariableTypes.DurHourMinute: break;
            case TVariableTypes.DateTime: break;
            case TVariableTypes.Date: break;
            //case TVariableTypes.String:
            //case TVariableTypes.PW:
            //case TVariableTypes.DateStr:
            //case TVariableTypes.ByteArray:
            default:
                {
                    var length = maxLen;
                    if (length <= 0 || length == 4000)
                        sqlFieldType += "(4000)";
                    else
                        sqlFieldType += "(" + length + ")";
                    break;
                }
        }

        return sqlFieldType;
    }

    protected override string ISNULL_Command()
    {
        return "NVL";
    }
}
