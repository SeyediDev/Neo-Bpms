using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public class StorageInterfaceWithStringKey<TEntity, TLogicModel>
    : StorageInterfaceBase<TEntity, TLogicModel, string, string>
    where TEntity : IStateBasedEntityWithKey<string>, new()
    where TLogicModel : IdentityBase<string>, new()

{
    protected StorageInterfaceWithStringKey(string filter, bool bLoad = true, bool activeDataSynchronizer = false)
    : base(filter, bLoad, activeDataSynchronizer)
    {
    }

    protected override string NullValueReplacement()
    {
        return "";
    }

    public override void SetRecord(TLogicModel item)
    {
    }

    protected override bool CheckKeyIsEmpty(string keyId)
    {
        return string.IsNullOrEmpty(keyId);
    }

    protected override string FetchKeyValue(string keyId)
    {
        return keyId;
    }

    protected override string FetchKeyValue(ElasticObject eRecord)
    {
        return eRecord.GetString("Id");
    }

    protected override void ResponseMethod(Dictionary<string, TLogicModel> requestedItems)
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
