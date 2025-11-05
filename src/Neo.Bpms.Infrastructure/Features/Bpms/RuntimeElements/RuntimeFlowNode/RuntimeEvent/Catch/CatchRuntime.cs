using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
public class CatchRuntime(ProcessVersionRuntime processVersion, CatchEvent catchEvent) : FlowNodeRunTime(processVersion, catchEvent)
{
    public CatchEvent CatchEvent => flowNode as CatchEvent;

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        switch (CatchEvent)
        {
            case StartEvent _:
                Catch(pi, null, inputData);
                break;
            case IntermediateCatchEvent _:
                var ei = new EventWaitingInstance(0, this, pi, ProcessInstanceStateId.Activated)
                {
                    CreationTime = DateTime.Now
                };
                ei.Save();
                break;
            case BoundaryEvent _:
                Catch(pi, null, inputData);
                break;
        }
    }

    internal void CatchAndSavePi(ProcessInstance pi, FlowNodeInstance ai, LocalParameters outputData)
    {
        Catch(pi, ai, outputData);
        DataStorage.SaveProcessInstance(pi, ai, "TRT.2", true, DataStorage.LockChangeRequest.Unlock);
    }

    internal void Catch(ProcessInstance pi, FlowNodeInstance ai, LocalParameters outputData)
    {
        if (ai?.FlowNode is IntermediateCatchEvent)
            ai.CompleteAndSave();
        CheckCancelActivity(pi);
        RunDataOutputAssociations(pi, ai, outputData);
        SendTokenToOutgoing(pi, outputData, TokenPattern.Parallel);
        pi.Execution.DoJobs();
    }

    private void CheckCancelActivity(ProcessInstance pi)
    {
        var cancelActivity = false;
        switch (flowNode)
        {
            case BoundaryEvent boundaryEvent:
                cancelActivity = boundaryEvent.cancelActivity;
                break;
            case StartEvent startEvent:
                cancelActivity = startEvent.isInterrupting;
                break;
        }

        if (cancelActivity)
            pi.CancelFlowNodeInstances();
    }
}