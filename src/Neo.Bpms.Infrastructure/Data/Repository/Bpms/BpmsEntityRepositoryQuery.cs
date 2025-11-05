using Neo.Bpms.Domain.Repository;
using Neo.Infrastructure.Data.Repository.Ef;

namespace Neo.Bpms.Infrastructure.Data.Repository.Bpms;

public class BpmsEntityRepositoryQuery<TEntity, TKey>(IBpmsUnitOfWorkQuery queryUnitOfWork)
    : EfQueryRepository<TEntity, TKey, IBpmsUnitOfWorkQuery>(queryUnitOfWork), IBpmsQueryRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
    where TKey : struct
{
}
