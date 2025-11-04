using Microsoft.SqlServer.Management.Smo.Agent;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

public class JobSummary(Job job)
{
    public string Name { get; set; } = job.Name;
    public DateTime LastRunDate { get; set; } = job.LastRunDate;
    public DateTime NextRunDate { get; set; } = job.NextRunDate;
}
