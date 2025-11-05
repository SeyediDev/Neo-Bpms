using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class ServiceTaskRuntime : TaskRuntime, IBPMNOperationRuntime //, IMessageCatchRuntime
{
    public ServiceTaskRuntime(ProcessVersionRuntime processVersion, ServiceTask serviceTask)
        : base(processVersion, serviceTask)
    {
        BPMNOperationRuntime = new BPMNOperationRuntime(this);
    }

    public ServiceTask ServiceTask => Activity as ServiceTask;
    public BPMNOperationRuntime BPMNOperationRuntime { get; set; }

    public override void CheckDataInputAvailabilityAndStartIfNeeded(ActivityInstance ai,
        bool fetchInputData, LocalParameters inputData) //todo fetchInputData
    {
        if (ai.state != ActivityInstanceStateId.Ready)
            return;
        BPMNOperationRuntime.RunOperationFromQueue(ai);
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
