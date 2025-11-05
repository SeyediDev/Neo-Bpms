using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
public class StartEvent(IFlowElementsContainer flowElementsContainer, string id, string name, bool isInterrupting = true) : CatchEvent(flowElementsContainer, id, name, CatchEventLocation.Start)
{
    public bool isInterrupting = isInterrupting;
}