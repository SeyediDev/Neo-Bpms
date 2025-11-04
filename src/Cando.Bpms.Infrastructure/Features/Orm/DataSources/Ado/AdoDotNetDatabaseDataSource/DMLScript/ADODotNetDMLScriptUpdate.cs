namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement update script functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    private string GenerateGroupUpdateScript(List<SqlValueField> newAuditValues)
    {
        var filter = GetFilterSql();
        var id = $"'{filter.Replace("'", "''").Limit(40)}'";
        return GenerateUpdateScript(new ElasticObject(), newAuditValues, "", id, DataSrcDefinition.Entity);
    }

    private string GenerateUpdateScript(ElasticObject oldRecord,
        IEnumerable<SqlValueField> newValues,
        string keyFilterValues, string keyValue, Entity entity)
    {
        var oldValuesOfChanged = new List<SqlValueField>();
        var newValuesOfChanged = new List<SqlValueField>();
        var fieldSetList = newValues.Select(
                newValue =>
                {
                    if (!CheckValueIsChanged(oldRecord, newValue, out var oldValue))
                        return null;
                    oldValuesOfChanged.Add(
                        new SqlValueField(newValue.Field)
                        {
                            FieldId = newValue.FieldId,
                            Value = oldValue
                        });
                    newValuesOfChanged.Add(newValue);
                    return $"[{newValue.FieldId}]={newValue.SqlValue}";
                }).Where(f => f != null).ToList();
        if (newValuesOfChanged.All(oldValue => oldValue.Field.AuditField))
            return "";
        var setValues = string.Join(",", fieldSetList);
        var commandString = new CandoStringBuilder("UPDATE");
        if (TopRows > 0)
            commandString += $" TOP({TopRows})";
        var dbTableName = GetTableDbName(entity, DataSrcDefinition.connection.DatabaseName);
        commandString += $" {dbTableName} SET {setValues}";
        commandString += GenerateDmlCommandWhereClause(keyFilterValues, entity);
        return commandString.ToString();
    }

    private bool CheckValueIsChanged(ElasticObject oldRecord, SqlValueField newAuditValue, out object oldValue)
    {
        var isNewValue = true;
        oldValue = null;
        if (oldRecord != null && oldRecord.GetField(newAuditValue.FieldId, out oldValue))
        {
            if (Equals(oldValue, newAuditValue.Value))
                isNewValue = false;
            else
            {
                var oldSqlValue = GetSqlValue(oldValue, newAuditValue.Field, oldValue.GetType());
                if (Equals(oldSqlValue, newAuditValue.SqlValue))
                    isNewValue = false;
            }
        }
        else if (newAuditValue.SqlValue == "null")
            isNewValue = false;

        return isNewValue;
    }
}
