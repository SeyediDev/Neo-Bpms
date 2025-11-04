using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
public class IntermediateCatchEvent(IFlowElementsContainer flowElementsContainer, string id, string name) : CatchEvent(flowElementsContainer, id, name, CatchEventLocation.IntermediateCatch)
{
}