namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
public class EndEvent(IFlowElementsContainer flowElementsContainer, string id, string name) : ThrowEvent(flowElementsContainer, id, name, ThrowEventLocation.End)
{
}