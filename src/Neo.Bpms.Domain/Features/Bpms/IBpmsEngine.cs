using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Domain.Features.Bpms;
public interface IBpmsEngine
{
    IBpmsRepository BpmsRepository { get; set; }
    void Load();

    bool CompleteTaskByUser(AuditTrail auditTrail,
        string processId, string processVersion, string taskId, long workItemId, string workDescription);

    long BPMNEngineId { get; }
}
