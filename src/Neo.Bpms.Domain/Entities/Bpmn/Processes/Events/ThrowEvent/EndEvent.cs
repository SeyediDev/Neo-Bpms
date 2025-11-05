using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;
public class EndEvent(IFlowElementsContainer flowElementsContainer, string id, string name) : ThrowEvent(flowElementsContainer, id, name, ThrowEventLocation.End)
{
}