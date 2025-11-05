using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public class EventWaitingInstance : FlowNodeInstance2
{
    public EventWaitingInstance(long instanceId, CatchRuntime catchRuntime, ProcessInstance pi,
        ProcessInstanceStateId instanceState)
        : base(instanceId, catchRuntime, pi, instanceState)
    {
    }

    public EventWaitingInstance(ActivityInstanceRecordDb flowNodeInstanceRecord,
        CatchRuntime catchRuntime, ProcessInstance pi)
        : base(flowNodeInstanceRecord, catchRuntime, pi)
    {
    }

    public CatchEvent Event => (CatchEvent)FlowNode;

    protected override Property GetProperty(string name)
    {
        return GetProperty(Event, name);
    }

    public bool IsExecuting()
    {
        return state switch
        {
            ProcessInstanceStateId.Activated => true,
            _ => false,
        };
    }
}
