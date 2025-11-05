using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

// ReSharper disable once UnusedTypeParameter
public abstract partial class StorageInterfaceBase<TEntity, TLogicModel, TKey, TNullableKey> :
    // ReSharper disable RedundantExtendsListEntry
    MemoryStorage<TLogicModel, TKey, TNullableKey>, IEntityChangedDriver, IDataSynchronizerItem<TKey>
    where TEntity : IStateBasedEntityWithKey<TKey>, new()
    where TLogicModel : IdentityBase<TNullableKey>, IBaseClassId<TNullableKey>, new()
{
    public string Name => typeof(TLogicModel).Name;
    public EntityAddress EntityAddress { get; private set; }
    protected Entity Entity { get; set; }

    public virtual void InsertDBReport<T>(T record)
    {
        var item = FetchRecord(record, false);
        if (!IsNotInThisStorage(item))
            AfterInsert(item);
        ActionAfterGetInsertReport(item);
    }

    protected virtual void ActionAfterGetInsertReport<T>(T record)
    {
    }

    public virtual void UpdateDBReport<T>(T record)
    {
        var item = FetchRecord(record, true);
        var oldItem = GetItem(item.Id);
        if (oldItem != null)
        {
            if (IsNotInThisStorage(item) || VirtualDeleted(record))
                AfterDelete(oldItem);
            AfterUpdate(item, oldItem);
        }
        else
            AfterInsert(item);

        ActionAfterGetUpdateReport(item, oldItem);
    }

    protected virtual void ActionAfterGetUpdateReport<T>(T item, T oldItem)
    {
    }

    public void UpdateGroupDBReport(List<ExpressionNode> filters)
    {
        ReloadDBReport(); //todo
    }

    public virtual void DeleteDBReport<T>(T record)
    {
        var item = FetchRecord(record, true);
        if (!IsNotInThisStorage(item))
            AfterDelete(item);
        ActionAfterGetDeleteReport(item);
    }

    protected virtual void ActionAfterGetDeleteReport<T>(T record)
    {
    }

    public void DeleteGroupDBReport(ExpressionNode deleteFilter)
    {
        ReloadDBReport(); //todo
    }

    public void ReloadDBReport()
    {
        AfterClear();
        Items = null;
        Load();
    }

    private bool VirtualDeleted<T>(T record)
    {
        if (!Entity.IsStateBase)
            return false;
        object stateId;
        if (record is ElasticObject eRecord)
        {
            if (!eRecord.GetField("StateId", out stateId))
                return false;
        }
        else if (!ReflectionField.GetValue(record, "StateId", out stateId))
            return false;

        var state = stateId != null ? Entity.GetState(stateId.ToString()) : null;
        if (state != null)
            return !state.IsActive;
        return stateId?.ToString() == "101";
    }

    private TLogicModel FetchRecord<T>(T record, bool loadOld)
    {
        switch (record)
        {
            case TLogicModel item:
                return item;
            case ElasticObject eRecord:
                var keyId = FetchKeyValue(eRecord);
                FillRecord(eRecord);
                var oldItem = loadOld ? GetItem(keyId) : default;
                if (oldItem == null)
                    return ConvertRecord(eRecord);
                var eItem = ElasticObject.ToElastic(oldItem);
                eItem.Merge(eRecord);
                return ConvertRecord(eItem);
            default:
                return default;
        }
    }

    protected void FillRecord(ElasticObject eRecord)
    {
        foreach (var f in Entity.entityFields.Values.Where(f => f.AssociationEntity != null))
        {
            if (f.AssociationEntity?.Maps == null || !eRecord.GetField(f.Id, out var value)) continue;
            if (f.AssociationEntity.Maps.Count == 1)
            {
                var srcField = Entity.GetField(f.AssociationEntity.Maps.FirstOrDefault()?.SourceField);
                if (value != null && !eRecord.GetField(srcField.Id, out _))
                {
                    eRecord.SetField(srcField.Id, value);
                }
                var type = value?.GetType();
                if ((type?.IsClass ?? false) && type != srcField.GetType())
                {
                    eRecord.GetField(srcField.Id, out var val);
                    eRecord.SetField(f.Id, val);
                }
            }
            else
            {
                var ids = value?.ToString().Split('#') ?? new string[1];
                var iIds = 0;
                foreach (var map in f.AssociationEntity.Maps)
                {
                    var ef = Entity.GetField(map.SourceField);
                    if (ef != null)
                    {
                        var v = iIds < ids.Length ? ids[iIds] : null;
                        eRecord.SetField(ef.Id, v);
                    }
                    iIds++;
                }
            }
        }
    }

    private TLogicModel ConvertRecord(ElasticObject eRecord)
    {
        return eRecord.To<TLogicModel>();
    }

    protected abstract bool IsNotInThisStorage(TLogicModel item);
    protected abstract TNullableKey FetchKeyValue(ElasticObject eRecord);
}
