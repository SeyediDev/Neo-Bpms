using Neo.Domain.Repository;

namespace Neo.Bpms.Domain.Repository;

public interface IBpmsCommandRepository<TEntity, TKey> : ICommandRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
{
}
