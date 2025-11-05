namespace Neo.Bpms.Domain.Entities.Cmmn.Storage;

public interface IStorageInterface<TLogicModel>
    : IMemoryStorage<long, long?, TLogicModel>
    where TLogicModel : IdentityBase<long?>, IBaseClassId<long?>, new()
{
}