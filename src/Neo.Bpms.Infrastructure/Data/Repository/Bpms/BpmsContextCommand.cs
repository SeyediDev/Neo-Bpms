using Neo.Bpms.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Neo.Bpms.Infrastructure.Data.Repository.Bpms;

public partial class BpmsContextCommand(DbContextOptions<BpmsContextCommand> options)
    : BpmsContext<BpmsContextCommand>(options), IBpmsUnitOfWorkCommand
{
    protected override Assembly ContextAssembly => typeof(BpmsContextCommand).Assembly;
}
