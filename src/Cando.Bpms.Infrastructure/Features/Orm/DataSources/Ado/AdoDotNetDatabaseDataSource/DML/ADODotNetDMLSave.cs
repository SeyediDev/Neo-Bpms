namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    public override bool Upset(ElasticObject elasticObject, ElasticObject keyObj,
        bool dontGiveOutput)
    {
        if (elasticObject == null) return false;
        if (DataSrcDefinition.Entity.KeyFields == null)
            throw new Exception($"There is no pkv in table {DataSrcDefinition.Entity.Name}");
        var keyFilterValues = FetchKeyFilterValue(elasticObject, keyObj, out var keyValue);
        var sqlValueFields = GetSqlValueFields(DataSrcDefinition.Entity);
        if (AuditTrail?.Batch != null && DataSrcDefinition.Entity.Auditable is not null)
        {
            SqlCommand = GenerateSaveScript(sqlValueFields, keyFilterValues,
                !dontGiveOutput, keyValue, DataSrcDefinition.Entity);
            var ret = dontGiveOutput ? openForUpdate(SqlCommand) : openForRead(SqlCommand);
            if (ret && !dontGiveOutput)
                ret = SetKeyValue(elasticObject);
            else
                SetException("12.1.14");
            return ret;
        }

        var oldRecord = new DatabaseDataReader(this, DataSrcDefinition.Entity)
            .FetchParentIds()
            .FetchOldRecord(sqlValueFields)
            .ReadData(keyFilterValues);
        if (oldRecord == null)
            return Insert(elasticObject, dontGiveOutput);
        return UpdateOldRecord(keyValue,
            keyFilterValues, sqlValueFields, oldRecord, DataSrcDefinition.Entity);
    }

    public override bool upset<T>(T obj, T keyObj, bool dontGiveOutput)
    {
        if (obj == null) return false;
        var keyFilterValues = FetchKeyFilterValue(obj, keyObj, out var keyValue);
        var sqlValueFields = GetSqlValueFields(DataSrcDefinition.Entity);
        if (sqlValueFields.All(valueField => valueField.Field.AuditField))
            return true;
        if (AuditTrail?.Batch != null && DataSrcDefinition.Entity.Auditable is not null)
        {
            SqlCommand = GenerateSaveScript(sqlValueFields, keyFilterValues,
                !dontGiveOutput, keyValue, DataSrcDefinition.Entity);
            var ret = dontGiveOutput ? openForUpdate(SqlCommand) : openForRead(SqlCommand);
            if (ret && !dontGiveOutput)
                ret = SetKeyValue(obj);
            else
                SetException("12.1.17");
            return ret;
        }

        var oldRecord = new DatabaseDataReader(this, DataSrcDefinition.Entity)
            .FetchParentIds()
            .FetchOldRecord(sqlValueFields)
            .ReadData(keyFilterValues);
        if (oldRecord == null)
            return insert(obj, dontGiveOutput);
        return UpdateOldRecord(keyValue, keyFilterValues, sqlValueFields, oldRecord, DataSrcDefinition.Entity);
    }
}
