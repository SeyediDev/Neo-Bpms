using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public abstract class MemoryStorageWithEntityChangeReport<TEntity, TLogicModel>(string filter, Action<TLogicModel> setRecord)
    : StorageInterfaceBase<TEntity, TLogicModel, long, long?>(filter), IStorageInterface<TLogicModel>
    where TEntity : IStateBasedEntity, new()
    where TLogicModel : IdentityBase<long?>, IBaseClassId<long?>, new()
{
    public override void SetRecord(TLogicModel item)
    {
        setRecord?.Invoke(item);
    }

    protected override void ClearEvent()
    {
    }

    protected override void LoadData()
    {
        if (Items != null) return;
        Items = new ConcurrentDictionary<long, TLogicModel>();
    }

    protected override bool InsertData(TLogicModel newItem, out string errorText)
    {
        errorText = "";
        return true;
    }

    protected override bool UpdateData(TLogicModel inItem, out string errorText)
    {
        errorText = "";
        return true;
    }

    protected override bool DeleteData(TLogicModel inItem, out string errorText)
    {
        errorText = "";
        return true;
    }

    protected override void ActionAfterGetInsertReport<T>(T record)
    {
    }
    protected override void ActionAfterGetUpdateReport<T>(T item, T oldItem)
    {
    }
    protected override void ActionAfterGetDeleteReport<T>(T record)
    {
    }
}
