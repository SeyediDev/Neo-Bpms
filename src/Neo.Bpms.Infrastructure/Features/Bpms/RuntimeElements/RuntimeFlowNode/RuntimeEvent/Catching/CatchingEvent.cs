using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
internal partial class CatchingEvent : Catching
{
    internal CatchingEvent(FlowNodeRunTime flowNodeRuntime, EventDefinition eventDefinition,
        ExecutionInstance execution, ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputData)
        : base(flowNodeRuntime, execution, pi, ai, inputData)
    {
        EventDefinition = eventDefinition;
    }

    internal bool EventReceived(CatchEventLocation location)
    {
        Execution.AuditTrail.AddDetail((long)BPMNAuditDetailTypeId.EventReceived,
            $"Received event {EventDefinition.GetType().Name[..^"Definition".Length]} {EventDefinition.Code}",
            ProcessVersion.DbId, Runtime.DbId, Pi?.Id, Ai?.Id);
        switch (location)
        {
            case CatchEventLocation.Start:
                return CatchingInStartEvent();
            case CatchEventLocation.IntermediateCatch:
                return CatchingInIntermediate();
            case CatchEventLocation.Boundary:
                return CatchingInBoundaryEvent(flowNode as BoundaryEvent);
            default:
                return false;
        }
    }

    protected virtual bool CatchingInStartEvent()
    {
        return false;
    }

    protected virtual bool CatchingInBoundaryEvent(BoundaryEvent boundaryEvent)
    {
        return false;
    }

    protected virtual bool CatchingFlowNode(FlowNodeRunTime flowNodeRuntime)
    {
        return false;
    }

    protected void CatchingFlowNodeInstance(FlowNodeInstance item)
    {
        var traceCode = $"catch in {flowNode.GetType().Name} {flowNode.Name}";
        var flowNodeInstance = DataStorage.LockControl(item, traceCode);
        if (flowNodeInstance == null)
            return;
        flowNodeInstance.AddAuditDetail(BPMNAuditDetailTypeId.EventReceived, traceCode);
        Pi = flowNodeInstance.pi;
        Ai = flowNodeInstance;
        CatchAndSavePi();
    }

    protected ProcessInstance CreateProcessInstanceAndCatching(List<DataOutput> dataOutputs, string entityPkv)
    {
        if (string.IsNullOrEmpty(entityPkv))
            entityPkv = ProcessVersion.FetchEntityPkv(InputData);
        Pi = ProcessVersion.CreateInstance(Execution, InputData, entityPkv, null, dataOutputs);
        Runtime.ReceiveToken(Pi, InputData);
        if (Runtime is not CatchRuntime)
        {
            FetchAndCatchAndSavePi();
        }
        else
            DataStorage.SaveProcessInstance(Pi, Ai, "CE.4", true, DataStorage.LockChangeRequest.Unlock);
        return Pi;
    }

    protected void CatchingInStartEventInPi()
    {
        if (IncomingEventBasedGatewayRuntime != null)
            CatchingEventGatewayInStartInOnePi();
        else
            CatchAndSavePi();
    }

    private bool CatchingInIntermediate()
    {
        if (IncomingEventBasedGatewayRuntime != null && IncomingEventBasedGatewayRuntime.IsStarting)
            return CatchingInStartEvent();
        return CatchingFlowNode(Runtime);
    }
}
