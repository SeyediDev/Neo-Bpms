namespace Neo.Bpms.Infrastructure.Features.Orm.Entities;

public static partial class ApplyUtility<T>
{
    public static bool Update(T oRecord, IdentityUser user = null, LocalParameters localParameters = null)
    {
        return New(user, localParameters).Update(oRecord);
    }

    public static bool Update(ElasticObject record, IdentityUser user = null, LocalParameters localParameters = null)
    {
        return New(user, localParameters).Update(record);
    }

    public static bool UpdateInTrail(T oRecord, AuditTrail auditTrail = null, LocalParameters localParameters = null)
    {
        return NewInTrail(auditTrail, localParameters).Update(oRecord);
    }

    public static bool UpdateInTrail(ElasticObject record, AuditTrail auditTrail = null,
        LocalParameters localParameters = null)
    {
        return NewInTrail(auditTrail, localParameters).Update(record);
    }
}

public partial class ApplyUtility
{
    public bool Update<T>(T oRecord, T keyValues, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null,
        bool compareWithPrevious = true)
    {
        Provider.SetConnectionParams(ConnectionValues, LocalParameters);
        AddFieldsIfEmpty(oRecord, true);
        if (!RunDataOperations(null, oRecord, dataOperation, DataOperation.eOperationType.Update, access,
            subjectId))
        {
            Release();
            return false;
        }

        RemoveKeyFields();
        var b = DataSource.update(oRecord, keyValues, compareWithPrevious);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        Release();
        return b;
    }

    public bool Update(ElasticObject record, ElasticObject keyValues, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null, bool forVirtualDelete = false,
        bool compareWithPrevious = true)
    {
        Provider.SetConnectionParams(ConnectionValues, LocalParameters);
        AddFieldsIfEmpty(record);
        if (!RunDataOperations<ElasticObject>(record, null, dataOperation, DataOperation.eOperationType.Update,
            access, subjectId))
        {
            Release();
            return false;
        }

        if (forVirtualDelete)
        {
            AddDateToAudit(record, nameof(ISoftDelete.ExpireDate));
            AddField(nameof(ISoftDelete.IsDeleted), true );
        }
        var b = DataSource.Update(record, keyValues, compareWithPrevious);
        RecordsAffected = DataSource.RecordsAffected;
        if (b)
            b = ThrowTrig<ElasticObject>(record, null, dataOperation, DataOperation.eOperationType.Update);
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.0.10");
        if (b)
        {
            if (!forVirtualDelete)
            {
                ParentEntitiesUpdateReport(record);
                ReportElasticUpdate(record);
            }

        }
        Release();
        return b;
    }

    public bool Update<T>(T oRecord, bool compareWithPrevious = true)
    {
        return Update(oRecord, null, "fa", compareWithPrevious);
    }

    public bool Update<T>(T oRecord, DataOperation dataOperation, string culture,
        bool compareWithPrevious = true,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null)
    {
        try
        {
            Provider.SetConnectionParams(ConnectionValues, LocalParameters);
            AddFieldsIfEmpty(oRecord, true);
            if (!RunDataOperations(null, oRecord, dataOperation, DataOperation.eOperationType.Update, access,
                subjectId))
            {
                Release();
                return false;
            }

            RemoveKeyFields();
            var b = DataSource.update(oRecord, oRecord, compareWithPrevious);
            RecordsAffected = DataSource.RecordsAffected;
            CommandTxt = DataSource.SqlCommand;
            if (!b) Rollback();
            if (!b)
                LogCommandTxtError("95.0.11");
            Release();
            return b;
        }
        catch (Exception e)
        {
            DataSource.Exceptions?.Add(new DataLayerException(e));
            Release();
            return false;
        }
    }

    public bool Update(ElasticObject record, bool compareWithPrevious = true)
    {
        return Update(record, compareWithPrevious, dataOperation: null);
    }

    public bool Update(ElasticObject record, bool compareWithPrevious, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null)
    {
        Provider.SetConnectionParams(ConnectionValues, LocalParameters);
        AddFieldsIfEmpty(record);
        if (!RunDataOperations<ElasticObject>(record, null, dataOperation, DataOperation.eOperationType.Update,
            access, subjectId))
        {
            Release();
            return false;
        }

        RemoveKeyFields();
        var b = DataSource.Update(record, record, compareWithPrevious);
        RecordsAffected = DataSource.RecordsAffected;
        if (b)
            b = ThrowTrig<ElasticObject>(record, null, dataOperation, DataOperation.eOperationType.Update);
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.0.12");
        if (b)
        {
            ParentEntitiesUpdateReport(record);
            ReportElasticUpdate(record);
        }
        Release();
        return b;
    }
    private void ParentEntitiesUpdateReport(ElasticObject record)
    {
        if (Entity.ParentEntities != null)
        {
            foreach (var parentEntity in Entity.ParentEntities)
            {
                var map = parentEntity.AssociationEntity.Maps.First();

                if (CheckParentField(map.SourceField, record, out var parentId))
                {
                    var parentRecord = GenerateParentRecord(record, parentEntity);
                    parentRecord[map.DestField] = parentId;
                }
            }
        }
    }

    private bool CheckParentField(string sourceField, ElasticObject record, out object value)
    {
        value = DataSource.ParentAssociationFields.FirstOrDefault(f => f.entityField.Id == sourceField).value;
        return value != null || record.GetField(sourceField, out value);
    }

    private void ReportElasticUpdate(ElasticObject record)
    {
        var updatingRecord = new ElasticObject();
        foreach (var fieldName in Entity.entityFields.Keys.Where(record.HasAttribute))
        {
            updatingRecord[fieldName] = record[fieldName];
        }
    }

    private void RemoveKeyFields()
    {
        if (DataSource?.Fields == null || Entity?.KeyFields == null)
            return;
        foreach (var key in Entity.KeyFields)
        {
            DataSource.Fields.Remove(key.Id);
        }
    }

    public bool UpdateWithFilter(string filter, ElasticObject record, DataOperation dataOperation = null)
    {
        var filters = filter != null ? new List<ExpressionNode> { Parser.Parse(filter) } : null;
        return UpdateWithFilterExpression(filters, record, dataOperation);
    }

    public bool UpdateWithFilters(ElasticObject record, params string[] filters)
    {
        var filtersNodes = new List<ExpressionNode>();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            filtersNodes.Add(Parser.Parse(filter));
        return UpdateWithFilterExpression(filtersNodes, record, null);
    }

    public bool UpdateWithFilterExpression(List<ExpressionNode> filters, ElasticObject record,
        DataOperation dataOperation)
    {
        DataSource.Filters = null;
        foreach (var filterItem in filters ?? Enumerable.Empty<ExpressionNode>())
        {
            var filter = filterItem.clone().replace(LocalParameters);
            filter = filter.processAndReplace(EntityConversions.ConvertFormulaFields, this);
            filter = filter.processAndReplace(EntityConversions.ConvertNames, this);
            if (filter is ConstantExpressionNode)
            {
                if (!ExpressionNode.CheckIfTrue((filter as ConstantExpressionNode).Value))
                {
                    Release();
                    return false;
                }

                filter = null;
            }

            if (!DataSource.ConvertExpressionToScript(true, filter, out var sFilter, out var outExp,
                    LocalParameters) || outExp != null)
            {
                Release();
                return false;
            }

            DataSource.AddFilter(sFilter, LocalParameters, null);
        }

        AddFieldsIfEmpty(record);
        if (!RunDataOperations<ElasticObject>(record, null, dataOperation, DataOperation.eOperationType.UpdateAll
            , UserSecurityAccessFlags.Update, null))
        {
            Release();
            return false;
        }

        var b = DataSource.UpdateGroup(record);
        RecordsAffected = DataSource.RecordsAffected;
        //if (b) b = throwTrig<ElasticObject>(record, null, dataOperation, DataOperation.eOperationType.Update, culture);
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.0.13");
        Release();
        return b;
    }

    public void SetTopRows(int topRows)
    {
        DataSource.TopRows = topRows;
    }
}
