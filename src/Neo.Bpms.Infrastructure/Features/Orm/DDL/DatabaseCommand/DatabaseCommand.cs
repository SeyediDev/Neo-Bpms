using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.Engine.DDL;
public abstract partial class DDLManager
{
    protected string GetDefaultValue(string fullFieldType)
    {
        return DDLGenerator.GetDefaultValue(fullFieldType);
    }

    protected string GetSqlFullFieldType(EntityField pField)
    {
        return DDLGenerator.GetFullFieldType(pField.FieldType, pField.MaxLen);
    }

    protected string GetSqlFullFieldType(DbField pField)
    {
        return DDLGenerator.GetFullFieldType(pField.Type, pField.Len);
    }

    protected string GetSqlFieldType(TVariableTypes fieldType)
    {
        return DDLGenerator.GetFieldType(fieldType);
    }

    protected bool IsStringType(string sqlFieldType)
    {
        return DDLGenerator.IsStringType(sqlFieldType);
    }
}
