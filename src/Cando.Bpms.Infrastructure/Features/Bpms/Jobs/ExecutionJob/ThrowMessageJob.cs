using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class ThrowMessageJob : ExecutionJob
{
    internal override void Execute()
    {
        _ = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).MessageReceivedByMessage(Message, InputData, AuditTrail);
    }

    public Message Message { get; set; }
    public LocalParameters InputData { get; set; }
    public AuditTrail AuditTrail { get; set; }
}
