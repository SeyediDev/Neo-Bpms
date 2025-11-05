using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

public class ExecutionInstance(AuditTrail auditTrail, string description)
{
    internal List<ExecutionJob> Jobs { get; set; }
    public AuditTrail AuditTrail { get; set; } = auditTrail;
    public string? Description { get; set; } = description;

    internal void AddJob(ExecutionJob job)
    {
        Jobs ??= [];
        Jobs.Add(job);
    }

    public void DoJobs()
    {
        while (Jobs != null && Jobs.Count > 0)
        {
            List<ExecutionJob> jobs = Jobs;
            Jobs = null;
            foreach (ExecutionJob job in jobs)
            {
                job.Execute();
            }
        }
    }
}
