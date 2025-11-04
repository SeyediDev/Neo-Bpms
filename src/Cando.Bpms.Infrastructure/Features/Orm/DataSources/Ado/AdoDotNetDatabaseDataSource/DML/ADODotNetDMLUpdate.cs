using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    public override List<(EntityField, object)> ParentAssociationFields { get; set; } = [];
    public override bool UpdateGroup(ElasticObject elasticObject)
    {
        if (elasticObject == null) return false;
        var sqlValueFields = GetSqlValueFields(DataSrcDefinition.Entity);
        SqlCommand = GenerateGroupUpdateScript(sqlValueFields);
        if (string.IsNullOrEmpty(SqlCommand)) return true;
        return openForUpdate(SqlCommand);
    }

    public override bool Update(ElasticObject record, ElasticObject keyObj, bool compareWithPrevious)
    {
        if (record == null) return false;
        var keyFilterValues = FetchKeyFilterValue(record, keyObj, out var keyValue);
        return UpdateOneTable(keyValue, keyFilterValues, DataSrcDefinition.Entity, compareWithPrevious);
    }

    public override bool update<T>(T obj, T keyObj, bool compareWithPrevious)
    {
        if (obj == null) return false;
        var keyFilterValues = FetchKeyFilterValue(obj, keyObj, out var keyValue);
        return UpdateOneTable(keyValue, keyFilterValues, DataSrcDefinition.Entity, compareWithPrevious);
    }

    private bool UpdateOneTable(string keyValue, string keyFilterValues, Entity entity,
        bool compareWithPrevious)
    {
        RecordsAffected = 0;
        var sqlValueFields = GetSqlValueFields(entity);
        var data = new DatabaseDataReader(this, entity).FetchParentIds();
        if (sqlValueFields.All(valueField => valueField.Field.AuditField))
        {
            data.ReadData(keyFilterValues);
            return UpdateParent(entity, compareWithPrevious);
        }
        ElasticObject oldRecord = null;
        if (compareWithPrevious)
        {
            oldRecord = data.FetchOldRecord(sqlValueFields).ReadData(keyFilterValues);
            ParentAssociationFields.AddRange(GetParentIds(data.ParentFieldList, oldRecord));
        }
        if (!compareWithPrevious && entity.Auditable == AuditableVersion.V1)
            return false;
        return (!compareWithPrevious || oldRecord != null) &&
               UpdateOldRecord(keyValue, keyFilterValues, sqlValueFields, oldRecord, entity) &&
               UpdateParent(entity, compareWithPrevious);
    }

    private List<(EntityField, object)> GetParentIds(List<EntityField> dataParentFieldList, ElasticObject oldRecord)
    {
        var l = new List<(EntityField, object)>();
        foreach (var entityField in dataParentFieldList)
        {
            if (oldRecord.GetField(entityField.Id, out var val))
            {
                l.Add((entityField, val));
            }
        }

        return l;
    }

    private bool UpdateParent(Entity entity, bool compareWithPrevious)
    {
        var b = true;
        foreach (var referenceField in Parents(entity))
        {
            var parentEntityRelationship = referenceField.Relationship as ParentEntity;
            if (parentEntityRelationship?.Maps == null)
                continue;
            var booleanFieldId = parentEntityRelationship.BooleanFieldIdInParentThatPresentMe;

            Fields.Remove(booleanFieldId);
            Fields.TryGetValue(parentEntityRelationship.Maps.FirstOrDefault()?.SourceField ?? "",
                out var relationMapField);

            if (!UpdateOneTable(GetSqlValueField(false, relationMapField, entity).SqlValue,
                FetchParentEntityKeyFilterValue(referenceField),
                referenceField.Relationship.DestEntity, compareWithPrevious))
                b = false;
        }

        return b;
    }

    private string FetchParentEntityKeyFilterValue(ReferenceField referenceField)
    {
        return string.Join(" AND ", referenceField.Relationship.DestEntity.KeyFields.Select(key =>
        {
            var map = referenceField.ParentAssociationField?.ParentEntity.Maps.FirstOrDefault(m => m.DestField == key.Id);
            Fields.TryGetValue(map?.SourceField ?? "", out var f);
            var value = GetSqlValue(f?.formula_value, key, key.CSharpType);
            return $"[{key.DbFieldName}]={value}";
        }));
    }

    private bool UpdateOldRecord(string keyValue, string keyFilterValues,
        IEnumerable<SqlValueField> sqlValueFields, ElasticObject oldRecord, Entity entity)
    {
        try
        {
            SqlCommand = GenerateUpdateScript(oldRecord, sqlValueFields, keyFilterValues, keyValue, entity);
            if (string.IsNullOrEmpty(SqlCommand)) return true;
            RecordsAffected = 0;
            openForUpdate(SqlCommand);
            var ret = AuditTrail?.Batch != null || (RecordsAffected != 0 && RecordsAffected != -1);
            if (!ret)
                SetException($"12.1.13.{RecordsAffected}");
            return ret;
        }
        catch (Exception e)
        {
            SetException("12.1.13", e);
            return false;
        }
    }

    private List<SqlValueField> GetSqlValueFields(Entity entity)
    {
        return [.. Fields.Where(fld => !entity.KeyFields.Any(k => k.Id == fld.Value.Field?.Id || k.DbFieldName == fld.Key)
                                            && entity.entityFields.TryGetValue(fld.Value.Field.Id, out var f)
                                            && f.MappedToDataInThisEntity)
            .Select(fld => GetSqlValueField(false, fld.Value, entity))];
    }

    private string FetchKeyFilterValueFromEntity(Entity entity)
    {
        return string.Join(" AND ", entity.KeyFields.Select(key =>
        {
            Fields.TryGetValue(key.DbFieldName, out var f);
            var value = GetSqlValue(f?.formula_value, key, key.CSharpType);
            return "[" + key.DbFieldName + "]=" + value;
        }));
    }
    class DatabaseDataReader(AdoDotNetDatabaseDataSource dataSource, Entity entity)
    {
        private readonly List<EntityField> _fieldList = [];
        public List<EntityField> ParentFieldList = [];

        public DatabaseDataReader FetchParentIds()
        {
            foreach (var parent in dataSource.Parents(entity))
            {
                var parentEntityRelationship = parent.Relationship as ParentEntity;
                if (parentEntityRelationship?.Maps?.FirstOrDefault() == null)
                    continue;

                parentEntityRelationship.Maps.ForEach(m =>
                {
                    var relationshipMapField = entity.GetField(m?.SourceField ?? "");
                    if (!_fieldList.Contains(relationshipMapField))
                    {
                        _fieldList.Add(relationshipMapField);
                        ParentFieldList.Add(relationshipMapField);
                    }
                });
            }

            return this;
        }
        public DatabaseDataReader FetchOldRecord(IEnumerable<SqlValueField> sqlValueFields)
        {
            _fieldList.AddRange(sqlValueFields.Select(f => f.Field)
                .Where(f => !f.AuditField));
            return this;
        }

        public ElasticObject ReadData(string keyFilterValues)
        {
            var oldValues = new ElasticObject("oldData");
            if (_fieldList.Count == 0) return oldValues;
            var dbTableName = GetTableDbName(entity, dataSource.DataSrcDefinition.connection.DatabaseName);
            dataSource.SqlCommand = $"SELECT {string.Join(",", _fieldList.Select(f => $"[{f.DbFieldName}]"))} FROM {dbTableName} WHERE {keyFilterValues}";
            if (!dataSource.openForRead(dataSource.SqlCommand))
                return null;
            if (dataSource.DataReader == null)
                return null;
            if (!dataSource.ReadFromDataReader())
                return null;
            var values = new object[_fieldList.Count];
            dataSource.DataReader.GetValues(values);
            var i = 0;
            foreach (var field in _fieldList)
            {
                var value = values[i++];
                if (value is not DBNull && value as string != "NULL")
                    oldValues.SetField(field.DbFieldName, value);
            }

            foreach (var parent in dataSource.Parents(entity))
            {
                var parentEntityRelationship = parent.Relationship as ParentEntity;
                parentEntityRelationship?.Maps?.ForEach(m =>
                {
                    var parentRelationshipMapFieldId = m?.SourceField ?? "";
                    var relationshipMapField = entity.GetField(parentRelationshipMapFieldId);
                    if (!dataSource.Fields.ContainsKey(relationshipMapField.DbFieldName))
                    {
                        dataSource.Fields.Add(relationshipMapField.DbFieldName,
                            new ColumnDefinition(relationshipMapField, oldValues[relationshipMapField.DbFieldName]));
                    }
                });
            }

            return oldValues;
        }
    }
}
