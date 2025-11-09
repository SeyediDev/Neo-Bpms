using Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities;
public static partial class ApplyUtility<T>
{
    public static bool Insert(T oRecord, bool dontGiveOutput = false, IdentityUser user = null)
    {
        return New(user).Insert(oRecord, dontGiveOutput);
    }

    public static bool InsertInTrail(T oRecord, bool dontGiveOutput = false, AuditTrail auditTrail = null,
        LocalParameters localParameters = null)
    {
        return NewInTrail(auditTrail, localParameters).Insert(oRecord, dontGiveOutput);
    }

    public static bool InsertInTrail(ElasticObject record, bool dontGiveOutput = false, AuditTrail auditTrail = null,
        LocalParameters localParameters = null)
    {
        return NewInTrail(auditTrail, localParameters).Insert(record, dontGiveOutput);
    }

    public static bool BulkInsert(IEnumerable<T> oRecords, IdentityUser user = null,
        LocalParameters localParameters = null, bool noCheckAllConstraint = true)
    {
        return New(user, localParameters).BulkInsert(oRecords, noCheckAllConstraint);
    }
}

public partial class ApplyUtility
{
    public static T GetItemOrInsert<T>(IDictionary<string, T> items, string code, Func<T> func, out bool isNewItem)
        where T : new()
    {
        var item = items.GetItem(code);
        if (item == null)
        {
            item = func();
            ApplyUtility<T>.Insert(item);
            isNewItem = true;
            items.Add(code, item);
        }
        else
        {
            isNewItem = false;
        }

        return item;
    }

    public bool InsertFromQuery(QueryUtility query)
    {
        AddFieldsIfEmpty();
        InitQueryDataSource(query);
        var b = query.InsertFromQuery(DataSource);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.14.0");
        query.ReleaseQuery();
        Release();
        return b;
    }

    public bool BulkInsert<T>(IEnumerable<T> oRecords, bool noCheckAllConstraint = true)
    {
        bool b;
        if (noCheckAllConstraint)
        {
            lock (string.Intern($"ParallelBulkInsertInTable{Entity.Id}"))
            {
                DeactivateAllConstraint();
                b = BulkInsert(oRecords);
                ActiveAllConstraint();
            }
        }
        else
            b = BulkInsert(oRecords);

        Release();
        return b;
    }

    public bool Insert<T>(T oRecord, bool dontGiveOutput = false)
    {
        return Insert(oRecord, null, UserSecurityAccessFlags.Create, null, dontGiveOutput);
    }

    public bool Insert(ElasticObject record, bool dontGiveOutput = false)
    {
        return Insert(record, null, UserSecurityAccessFlags.Create, null, dontGiveOutput);
    }

    public bool Insert(ElasticObject record, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Create, string subjectId = null, bool dontGiveOutput = false)
    {
        Provider.SetConnectionParams(ConnectionValues, LocalParameters);
        AddFieldsIfEmpty(record);
        if (!RunDataOperations<ElasticObject>(record, null, dataOperation,
            DataOperation.eOperationType.Constructor, access, subjectId))
        {
            Release();
            return false;
        }

        NormalizePkValueBeforeInsert();
        var b = DataSource.Insert(record, dontGiveOutput);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (b)
        {
            b = ThrowTrig<ElasticObject>(record, null, dataOperation,
                DataOperation.eOperationType.Constructor);
        }

        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.14.5");
        if (b)
        {
            ParentEntitiesInsertReport(record);
        }

        Release();
        return b;
    }

    private void ParentEntitiesInsertReport<T>(T record)
    {
        if (Entity.ParentEntities != null)
        {
            foreach (var parentEntity in Entity.ParentEntities)
            {
                if (!FetchMemberValue(record, parentEntity.Id, out var parentRecord))
                    continue;
                ParentEntityInsertReport(parentRecord, parentEntity, record);
            }
        }
    }

    private static bool FetchMemberValue<T>(T record, string fieldId, out object value)
    {
        var memberInfo = ReflectionField.FetchMember(record.GetType(), fieldId);
        if (memberInfo == null)
        {
            value = null;
            return false;
        }

        ReflectionField.GetMemberValue(record, memberInfo, out value);
        return true;
    }

    private void ParentEntityInsertReport<T, TMain>(T record, EntityField parentEntity, TMain mainRecord)
    {
        var parentRecord = new ElasticObject();
        foreach (var field in parentEntity.ParentEntity.DestEntity.entityFields.Values)
        {
            object value;
            if (field.IncludeInPkv)
            {
                var map = parentEntity.AssociationEntity.Maps.FirstOrDefault(m => m.DestField == field.Id);
                if (map == null) continue;
                if (!FetchMemberValue(mainRecord, map.SourceField, out value))
                    continue;
            }
            else if (!FetchMemberValue(record, field.Id, out value))
                continue;

            if (field.ParentEntity != null)
                ParentEntityInsertReport(value, field, record);
            else
                parentRecord.SetField(field.Id, value);
        }
    }

    private void ParentEntitiesInsertReport(ElasticObject record)
    {
        if (Entity.ParentEntities != null)
        {
            foreach (var parentEntity in Entity.ParentEntities)
            {
                var parentRecord = GenerateParentRecord(record, parentEntity);
            }
        }
    }

    private static ElasticObject GenerateParentRecord(ElasticObject record, EntityField parentEntity)
    {
        var parentRecord = new ElasticObject();
        foreach (var field in parentEntity.ParentEntity.DestEntity.entityFields.Values)
        {
            bool hasValue;
            object value;
            var fieldId = field.Id;
            if (field.IncludeInPkv)
            {
                var map = parentEntity.AssociationEntity.Maps.FirstOrDefault(m => m.DestField == field.Id);
                if (map == null) continue;
                hasValue = record.GetField(map.SourceField, out value);
            }
            else
            {
                hasValue = record.GetField(fieldId, out value);
            }

            if (hasValue)
                parentRecord.SetField(fieldId, value);
        }

        return parentRecord;
    }

    private static EntityChangedReporterConfig NewConfig(EntityField parentEntity)
    {
        return new EntityChangedReporterConfig
        {
            EntityAddress = parentEntity.ParentEntity.DestEntity.EntityAddress
        };
    }

    public ApplyUtility DeactivateAllConstraint()
    {
        DataSource.Command($"ALTER TABLE {EntityDbNameManager.GetTableDbFullName(Entity)} NOCHECK CONSTRAINT ALL");
        CommandTxt = DataSource.SqlCommand;
        return this;
    }

    public ApplyUtility ActiveAllConstraint()
    {
        DataSource.Command($"ALTER TABLE {EntityDbNameManager.GetTableDbFullName(Entity)} CHECK CONSTRAINT ALL");
        CommandTxt = DataSource.SqlCommand;
        return this;
    }

    private bool BulkInsert<T>(IEnumerable<T> oRecords)
    {
        AddFieldsIfEmpty();
        if (!OpenConnection())
        {
            Release();
            return false;
        }

        var b = DataSource.BulkInsert(oRecords);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.14.3");
        Release();
        return b;
    }

    private bool Insert<T>(T oRecord, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Create, string subjectId = null, bool dontGiveOutput = false)
    {
        AddFieldsIfEmpty(oRecord, false);
        if (!RunDataOperations(null, oRecord, dataOperation,
            DataOperation.eOperationType.Constructor, access, subjectId))
        {
            Release();
            return false;
        }

        NormalizePkValueBeforeInsert();
        var b = DataSource.insert(oRecord, dontGiveOutput);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (b)
        {
            b = ThrowTrig(null, oRecord, dataOperation,
                DataOperation.eOperationType.Constructor);
        }

        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.14.4");
        if (b)
        {
            ParentEntitiesInsertReport(oRecord);
        }

        Release();
        return b;
    }

    private void NormalizePkValueBeforeInsert()
    {
        var keyFields = Entity.KeyFields.ToList();
        var dsFields = DataSource.Fields.Where(f => keyFields.Any(kf =>
            {
                if (kf.Id != f.Key)
                    return false;
                var auto = Entity.AutoCalcs?.Calculations?.FirstOrDefault(ac => ac.FieldId == kf.Id);
                return auto != null && !auto.RecalcOnAnyChange &&
                       auto.GenerationType == AutoCalc.eGenerationType.DBInsert;
            }))
            .ToList();
        foreach (var dsField in dsFields)
        {
            DataSource.Fields.Remove(dsField.Key);
        }
    }
}
