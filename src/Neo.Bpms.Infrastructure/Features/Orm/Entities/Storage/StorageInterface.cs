using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public class StorageInterface<TEntity, TLogicModel>(
    string filter, Action<TLogicModel> setRecord, bool bLoad = true)
    : StorageInterfaceBase<TEntity, TLogicModel, long, long?>(filter, bLoad), IStorageInterface<TLogicModel>
    where TEntity : IStateBasedEntity, new()
    where TLogicModel : IdentityBase<long?>, IBaseClassId<long?>, new()
{
    protected override long NullValueReplacement()
    {
        return 0;
    }

    public override void SetRecord(TLogicModel item)
    {
        setRecord?.Invoke(item);
    }

    protected override bool CheckKeyIsEmpty(long? keyId)
    {
        return keyId == null || keyId.Value == 0;
    }

    protected override long FetchKeyValue(long? keyId)
    {
        return keyId ?? 0;
    }
    protected override long? FetchKeyValue(ElasticObject eRecord)
    {
        return eRecord.Id;
    }

    protected override void ResponseMethod(Dictionary<long, TLogicModel> requestedItems)
    {
    }

    protected override string SynchronizationFilter()
    {
        return "";
    }

    protected override List<string> SynchronizationSelectFields()
    {
        return [];
    }

    protected override LocalParameters SynchronizationFilterValues()
    {
        return [];
    }

    protected override bool IsNotInThisStorage(TLogicModel item)
    {
        return false;
    }
}
