using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway.RuntimeEventBasedGateway;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
internal class Catching
{
    internal Catching(FlowNodeRunTime flowNodeRuntime,
        ExecutionInstance execution, ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputData)
    {
        Runtime = flowNodeRuntime;
        Execution = execution;
        InputData = inputData;
        Pi = pi;
        Ai = ai;
    }

    protected FlowNodeRunTime Runtime { get; set; }
    protected EventDefinition EventDefinition { get; set; }
    protected ProcessVersionRuntime ProcessVersion => Runtime.ProcessVersion;
    protected EventBasedGatewayRuntime IncomingEventBasedGatewayRuntime => Runtime.IncomingEventBasedGatewayRuntime;
    protected FlowNode flowNode => Runtime.flowNode;
    protected ExecutionInstance Execution { get; set; }
    protected LocalParameters InputData { get; set; }
    protected ProcessInstance Pi { get; set; }
    protected FlowNodeInstance Ai { get; set; }

    protected void FetchAndCatchAndSavePi()
    {
        Execution.AddJob(new ActionJob(() =>
        {
            Ai = DataStorage.FetchFlowNodeInstancesOfFlowNode(Runtime, Execution, Pi)
                ?.Values.FirstOrDefault();
            if (Ai == null)
                Pi.AddAuditDetail(BPMNAuditDetailTypeId.Error,
                    $"can not find {flowNode.GetType().Name} {flowNode.Id} instance.");
            CatchAndSavePi();
        }));
    }

    protected internal void CatchAndSavePi()
    {
        switch (Runtime)
        {
            case CatchRuntime catchRuntime:
                catchRuntime.CatchAndSavePi(Pi, Ai, InputData);
                DataStorage.SaveProcessInstance(Pi, Ai, "CE.3", true, DataStorage.LockChangeRequest.Unlock);
                break;
            case ActivityRuntime activityRuntime when Ai is ActivityInstance ai:
                activityRuntime.StartActivity(ai, InputData);
                DataStorage.SaveProcessInstance(ai.pi, ai, "CE.1", true, DataStorage.LockChangeRequest.Unlock);
                break;
            default:
                if (Ai != null)
                    Runtime.CompleteAndSavePi(Ai, InputData, FlowNodeRunTime.TokenPattern.Inclusive);
                else
                {
                    Pi.AddAuditDetail(BPMNAuditDetailTypeId.Error, "Ai is null in CatchAndSavePi method.");
                    DataStorage.SaveProcessInstance(Pi, Ai, "CE.2", true, DataStorage.LockChangeRequest.Unlock);
                }
                break;
        }
    }

    protected internal void Catch()
    {
        switch (Runtime)
        {
            case CatchRuntime catchRuntime:
                catchRuntime.Catch(Pi, Ai, InputData);
                break;
            case ActivityRuntime activityRuntime when Ai is ActivityInstance ai:
                activityRuntime.StartActivity(ai, InputData);
                break;
            default:
                if (Ai != null)
                    Runtime.Complete(Ai, InputData, FlowNodeRunTime.TokenPattern.Inclusive);
                else
                    Pi.AddAuditDetail(BPMNAuditDetailTypeId.Error, "Ai is null in CatchAndSavePi method.");
                break;
        }
    }
}