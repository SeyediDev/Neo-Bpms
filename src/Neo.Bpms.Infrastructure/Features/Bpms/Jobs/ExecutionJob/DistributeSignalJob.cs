using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;


namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class DistributeSignalJob : ExecutionJob
{
    internal override void Execute()
    {
        _ = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).DistributeSignal(Execution, Signal?.Name, InputData);
    }

    public Message Message { get; set; }
    public LocalParameters InputData { get; set; }
    public Signal Signal { get; set; }
    public ExecutionInstance Execution { get; set; }
}
