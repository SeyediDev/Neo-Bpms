using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
internal class CatchingSignal : CatchingEvent
{
    internal CatchingSignal(FlowNodeRunTime flowNodeRuntime, SignalEventDefinition signalDefinition,
        ExecutionInstance execution, ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputData)
        : base(flowNodeRuntime, signalDefinition, execution, pi, ai, inputData)
    {
    }

    protected override bool CatchingInStartEvent()
    {
        if (!flowNode.InSubProcess)
        {
            Pi = CreateProcessInstanceAndCatching((flowNode as IDataOutputContainer)?.dataOutputs, null);
        }
        else
        {
            var pis = DataStorage.LoadProcessInstances(ProcessVersion, Execution);
            foreach (var item in pis.Values)
            {
                try
                {
                    Pi = DataStorage.LockControl(item, "catch signal in event sub process start", out _);
                    CatchingInStartEventInPi();
                }
                catch
                {
                    // ignored
                }
            }
        }

        return Pi != null;
    }

    protected override bool CatchingFlowNode(FlowNodeRunTime flowNodeRuntime)
    {
        var flowNodeInstances = DataStorage.FetchFlowNodeInstancesOfFlowNode(flowNodeRuntime, Execution, null);
        if (flowNodeInstances != null)
        {
            foreach (var item in flowNodeInstances.Values)
                CatchingFlowNodeInstance(item);
            return flowNodeInstances.Values.Count > 0;
        }

        return false;
    }

    protected override bool CatchingInBoundaryEvent(BoundaryEvent boundaryEvent)
    {
        var activityRunTime = ProcessVersion.GetFlowNodeRuntime(boundaryEvent.attachedToRef);
        return activityRunTime != null &&
               CatchingFlowNode(activityRunTime);
    }
}