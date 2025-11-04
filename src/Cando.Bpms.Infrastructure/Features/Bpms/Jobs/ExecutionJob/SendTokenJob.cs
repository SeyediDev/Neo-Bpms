using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class SendToken : ExecutionJob
{
    internal override void Execute()
    {
        FlowNodeRunTime.ReceiveToken(ProcessInstance, InputData, SequenceFlow);
    }

    public ProcessInstance ProcessInstance { get; set; }
    public FlowNodeRunTime FlowNodeRunTime { get; set; }
    public SequenceFlow SequenceFlow { get; set; }
    public LocalParameters InputData { get; set; }
}
