using Neo.Bpms.Domain.Models.JobScheduling;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.JobScheduler;

public class Job
{
    public IJobSchedule JobSchedule { get; set; }
    public object JobItem { get; set; }
}
