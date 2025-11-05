namespace Neo.Bpms.Infrastructure.Features.Orm.Entities;

public partial class ApplyUtility
{
    public bool Save<T>(T oRecord)
    {
        return Save(oRecord, null);
    }

    public bool Save<T>(T oRecord, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null)
    {
        AddFieldsIfEmpty(oRecord, true);
        var operationType = access == UserSecurityAccessFlags.Create
            ? DataOperation.eOperationType.Constructor
            : DataOperation.eOperationType.Update;
        var keyField = Entity.KeyFields.FirstOrDefault();
        if (keyField != null)
        {
            DataSource.Fields.TryGetValue(keyField.Id, out var kField);
            if (kField == null)
                operationType = DataOperation.eOperationType.Constructor;
            var v = DataSource.GetField(oRecord, keyField.Id);
            if (string.IsNullOrEmpty(v?.ToString()) || v.ToString() == "undefined")
                operationType = DataOperation.eOperationType.Constructor;
        }

        if (!RunDataOperations(null, oRecord, dataOperation,
            operationType, access, subjectId))
        {
            Release();
            return false;
        }

        if (operationType == DataOperation.eOperationType.Constructor)
            NormalizePkValueBeforeInsert();
        var b = operationType == DataOperation.eOperationType.Constructor
            ? DataSource.insert(oRecord, false)
            : DataSource.upset(oRecord, oRecord, false);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.2.8");
        Release();
        return b;
    }

    public bool Save(ElasticObject record)
    {
        return Save(record, null);
    }

    public bool Save(ElasticObject record, DataOperation dataOperation,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null)
    {
        AddFieldsIfEmpty(record);
        var operationType = access == UserSecurityAccessFlags.Create
            ? DataOperation.eOperationType.Constructor
            : DataOperation.eOperationType.Update;
        var keyField = Entity.KeyFields.FirstOrDefault();
        if (keyField != null)
        {
            DataSource.Fields.TryGetValue(keyField.Id, out var kField);
            if (kField == null)
                operationType = DataOperation.eOperationType.Constructor;
            else
            {
                var v = DataSource.GetField(record, keyField.Id);
                if (string.IsNullOrEmpty(v?.ToString()) || v.ToString() == "undefined")
                    operationType = DataOperation.eOperationType.Constructor;
            }
        }

        if (!RunDataOperations<ElasticObject>(record, null, dataOperation,
            operationType, access, subjectId))
        {
            Release();
            return false;
        }

        if (operationType == DataOperation.eOperationType.Constructor)
            NormalizePkValueBeforeInsert();
        var b = operationType == DataOperation.eOperationType.Constructor
            ? DataSource.Insert(record, false)
            : DataSource.Upset(record, record, false);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (b)
            b = ThrowTrig<ElasticObject>(record, null, dataOperation,
                DataOperation.eOperationType.Constructor);
        if (!b) Rollback();
        if (!b)
            LogCommandTxtError("95.2.9");
        Release();
        return b;
    }
}