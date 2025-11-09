using Neo.Bpms.Domain.Models.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.MetaDb;

public abstract class MetaDataManager<TMetaData> : BaseMetaData
        where TMetaData : MetaData, new()
{
    protected MetaDataManager()
    {
        _metaData = new TMetaData();
        HasNotExistsTable = !_metaData.Init(true);
        ExceptionInfos.AppendErrors(ref Errors, _metaData.Errors);
        if (!HasNotExistsTable)
        {
            _backupMetaData = new TMetaData();
            HasNotExistsTable = !_backupMetaData.Init(false);
            ExceptionInfos.AppendErrors(ref Errors, _metaData.Errors);
        }
    }

    protected readonly TMetaData _metaData;
    protected readonly TMetaData _backupMetaData;

    // ReSharper disable once StaticMemberInGenericType
    protected abstract void SyncMetaData();
    protected abstract void BackupUnusedItems();

    public void SaveMetadataToDatabase()
    {
        try
        {
            if (!HasNotExistsTable)
            {
                SyncMetaData();
                BackupUnusedItems();
            }
        }
        catch (Exception e)
        {
            AddError("Sync", "Sync", e);
        }
    }

    protected T FetchItem<T>(T newItem,
        Func<T, string> keyItem,
        Func<T, T, bool> compareItem,
        Func<T, T, bool> setItem,
        IDictionary<string, T> items,
        IDictionary<string, T> backupItems, string itemId)
        where T : class, IStateBasedEntity, new()
    {
        T item = items.GetItem(itemId) ?? backupItems.GetItem(itemId);
        if (item == null)
        {
            item = newItem;
            item.StateId = (long)PublicEntityStateId.Active;
            _ = Insert(item);
            items.Add(keyItem(item), item);
        }
        else
        {
            long oldState = item.StateId;
            if (item.StateId != (long)PublicEntityStateId.WaitForBackup ||
                compareItem(item, newItem))
            {
                item.StateId = (long)PublicEntityStateId.Active;
                _ = setItem(item, newItem);
                _ = Update(item);
                if (oldState == (long)PublicEntityStateId.Backup)
                {
                    items.Add(keyItem(item), item);
                }
            }
            else
            {
                item.StateId = (long)PublicEntityStateId.Active;
                if (oldState == (long)PublicEntityStateId.Backup)
                {
                    _ = Update(item);
                }
            }
        }

        return item;
    }

    protected bool Insert<T>(T oRecord) where T : new()
    {
        return DbOperation(oRecord, (record, au) => au.Insert(record));
    }

    protected bool Update<T>(T oRecord) where T : new()
    {
        return DbOperation(oRecord, (record, au) => au.Update(record));
    }

    protected void BackupUnusedItems<T>(IEnumerable<T> items) where T : IStateBasedEntity, new()
    {
        foreach (T item in items.Where(p => p.StateId == (long)PublicEntityStateId.WaitForBackup))
        {
            BackupUnusedItem(item);
        }
    }

    private void BackupUnusedItem<T>(T item) where T : IStateBasedEntity, new()
    {
        item.StateId = (long)PublicEntityStateId.Backup;
        _ = Update(item);
    }

    private bool DbOperation<T>(T oRecord, Func<T, ApplyUtility, bool> func) where T : new()
    {
        ApplyUtility au = ApplyUtility<T>.New();
        bool success = false;
        try
        {
            success = func(oRecord, au);
            if (!success)
            {
                AddError(typeof(T).Name, au.ErrorText);
            }
        }
        catch (Exception e)
        {
            AddError(typeof(T).Name, typeof(T).Name, e);
        }

        return success;
    }
}
