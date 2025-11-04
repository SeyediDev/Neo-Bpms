using Neo.Bpms.Domain.Repository;
using Neo.Infrastructure.Data.Repository.Ef;

namespace Neo.Bpms.Infrastructure.Data.Repository.Bpms;

public class BpmsEntityRepositoryCommand<TEntity, TKey>(IBpmsUnitOfWorkCommand commandUnitOfWork)
    : EfCommandRepository<TEntity, TKey, IBpmsUnitOfWorkCommand>(commandUnitOfWork), IBpmsCommandRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
    where TKey : struct
{
}
