namespace Neo.Bpms.Infrastructure.Features.Orm.Entities;
public static partial class ApplyUtility<T>
{
    public static bool Delete(T oRecord, AuditTrail auditTrail, LocalParameters localParameters = null)
    {
        return NewInTrail(auditTrail, localParameters).Delete(oRecord);
    }
}

public partial class ApplyUtility
{
    public bool Delete<T>(T oRecord)
    {
        return Delete(oRecord, null);
    }

    public bool Delete<T>(T oRecord, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Delete, string subjectId = null)
    {
        if (!RunDataOperations(null, oRecord, dataOperation,
            DataOperation.eOperationType.Destructor, access, subjectId))
        {
            Release();
            return false;
        }

        var b = DataSource.Delete(oRecord);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.1.0");
        Release();
        return b;
    }

    public bool Delete(ElasticObject record)
    {
        return Delete(record, null);
    }

    public bool Delete(ElasticObject record, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Delete, string subjectId = null)
    {
        if (!RunDataOperations<ElasticObject>(record, null, dataOperation,
            DataOperation.eOperationType.Destructor, access, subjectId))
        {
            Release();
            return false;
        }

        var b = DataSource.Delete(record);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (b)
            b = ThrowTrig<ElasticObject>(record, null, dataOperation,
                DataOperation.eOperationType.Destructor);
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.0.1");
        Release();
        return b;
    }

    public bool DeleteWithFilter(string filter)
    {
        return DeleteWithFilter(Parser.Parse(filter));
    }

    public bool DeleteWithFilter(ExpressionNode deleteFilter)
    {
        if (deleteFilter != null)
        {
            var filter = deleteFilter.clone();
            filter = filter.replace(LocalParameters);
            filter = filter.processAndReplace(EntityConversions.ConvertFormulaFields, this);
            filter = filter.processAndReplace(EntityConversions.ConvertNames, this);
            if (filter is ConstantExpressionNode constantExpressionNode)
            {
                if (!ExpressionNode.CheckIfTrue(constantExpressionNode.Value))
                {
                    Release();
                    return true;
                }

                filter = null;
            }

            if (!DataSource.ConvertExpressionToScript(false, filter, out var sFilter, out var outExp, LocalParameters) ||
                outExp != null)
            {
                Release();
                return false;
            }

            DataSource.Filters = null;
            DataSource.AddFilter(sFilter, LocalParameters, null);
        }

        var record = new ElasticObject();
        if (!RunDataOperations<ElasticObject>(record, null, null,
            DataOperation.eOperationType.Destructor, UserSecurityAccessFlags.Delete, null))
        {
            Release();
            return false;
        }

        var b = DataSource.DeleteGroup(record);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (b)
            b = ThrowTrig<ElasticObject>(null, null, null,
                DataOperation.eOperationType.Destructor);
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.0.2");
        Release();
        return b;
    }
}
