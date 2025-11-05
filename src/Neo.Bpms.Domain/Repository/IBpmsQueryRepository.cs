using Neo.Domain.Repository;

namespace Neo.Bpms.Domain.Repository;

public interface IBpmsQueryRepository<TEntity, TKey> : IQueryRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
{
}
