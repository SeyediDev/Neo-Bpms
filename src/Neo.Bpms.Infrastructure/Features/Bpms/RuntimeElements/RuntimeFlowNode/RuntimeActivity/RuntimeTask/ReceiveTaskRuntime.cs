using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class ReceiveTaskRuntime : TaskRuntime, IBPMNOperationRuntime, IMessageCatchRuntime
{
    public ReceiveTaskRuntime(ProcessVersionRuntime processVersion, ReceiveTask receiveTask)
        : base(processVersion, receiveTask)
    {
        MessageDefinition = new MessageEventDefinition(null, receiveTask.Id + ".Message",
            receiveTask.messageRef, receiveTask.operationRef);
        BPMNOperationRuntime = new BPMNOperationRuntime(this);
    }

    public ReceiveTask ReceiveTask => Activity as ReceiveTask;
    public BPMNOperationRuntime BPMNOperationRuntime { get; set; }

    public MessageEventDefinition MessageDefinition { get; set; }

    public bool MessageReceived(ExecutionInstance execution, MessageEventDefinition message,
        LocalParameters messageParams)
    {
        var catchingMessage = new CatchingMessage(this, message, execution, null, null, messageParams);
        return catchingMessage.EventReceived(CatchEventLocation.IntermediateCatch);
    }

    public override void CheckDataInputAvailabilityAndStartIfNeeded(ActivityInstance ai,
        bool fetchInputData, LocalParameters inputData)
    {
        //if (ai.state != ActivityInstanceStateId.Ready)
        //	return;
        //OperationRuntime.RunOperationFromQueue(ai);
    }

    internal override async System.Threading.Tasks.Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await BPMNOperationRuntime.Run(ai, inputData);
    }

    public override void Free(FlowNodeInstance ai)
    {
        BPMNOperationRuntime.Free(ai);
    }
}
