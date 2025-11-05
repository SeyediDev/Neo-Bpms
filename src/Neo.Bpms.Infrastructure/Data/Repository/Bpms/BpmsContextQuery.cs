using Neo.Bpms.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Neo.Bpms.Infrastructure.Data.Repository.Bpms;

public partial class BpmsContextQuery(DbContextOptions<BpmsContextQuery> options)
    : BpmsContext<BpmsContextQuery>(options), IBpmsUnitOfWorkQuery
{
    protected override Assembly ContextAssembly => typeof(BpmsContextQuery).Assembly;
}
