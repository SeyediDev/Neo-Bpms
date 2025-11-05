namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
public class IntermediateThrowEvent(IFlowElementsContainer flowElementsContainer, string id, string name) : ThrowEvent(flowElementsContainer, id, name, ThrowEventLocation.IntermediateThrow)
{
}
