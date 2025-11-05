namespace Neo.Bpms.Domain.Entities.Cmmn.Storage;

public interface ILogStorageInterface<TEntity> : IMemoryStorageBase
        where TEntity : new()
{
    bool Insert(IList<TEntity> inItems);
    bool Insert(TEntity inItem);
}