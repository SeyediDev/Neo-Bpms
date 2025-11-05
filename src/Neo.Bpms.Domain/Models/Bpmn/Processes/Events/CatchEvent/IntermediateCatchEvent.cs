namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
public class IntermediateCatchEvent(IFlowElementsContainer flowElementsContainer, string id, string name) : CatchEvent(flowElementsContainer, id, name, CatchEventLocation.IntermediateCatch)
{
}