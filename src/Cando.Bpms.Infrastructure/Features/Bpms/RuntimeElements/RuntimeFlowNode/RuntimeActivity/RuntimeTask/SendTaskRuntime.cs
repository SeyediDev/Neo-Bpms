using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class SendTaskRuntime : TaskRuntime, IBPMNOperationRuntime
{
    public SendTaskRuntime(ProcessVersionRuntime processVersion, SendTask task)
        : base(processVersion, task)
    {
        BPMNOperationRuntime = new BPMNOperationRuntime(this);
    }

    public SendTask TaskDefinition => Activity as SendTask;
    public BPMNOperationRuntime BPMNOperationRuntime { get; set; }

    public override void CheckDataInputAvailabilityAndStartIfNeeded(ActivityInstance ai,
        bool fetchInputData, LocalParameters inputData)
    {
        BPMNOperationRuntime.RunOperationFromQueue(ai);
        base.CheckDataInputAvailabilityAndStartIfNeeded(ai, fetchInputData, inputData);
    }

    internal override async System.Threading.Tasks.Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await BPMNOperationRuntime.Run(ai, inputData);
        if (TaskDefinition.messageRef != null)
        {
            ai.pi.Execution.AddJob(new ThrowMessageJob
            {
                Message = TaskDefinition.messageRef,
                InputData = inputData?.Clone(),
                AuditTrail = ai.pi.AuditTrail
            });
        }

        ai.pi.Execution.AddJob(new ActionJob(() => Complete(ai, inputData)));
    }

    public override void Free(FlowNodeInstance ai)
    {
        BPMNOperationRuntime.Free(ai);
    }
}
