namespace Neo.Bpms.Domain.Models.Cmmn.Storage;

public interface IStorageInterface<TLogicModel>
    : IMemoryStorage<long, long?, TLogicModel>
    where TLogicModel : IdentityBase<long?>, IBaseClassId<long?>, new()
{
}