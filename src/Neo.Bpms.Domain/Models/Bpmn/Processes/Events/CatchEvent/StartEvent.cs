namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
public class StartEvent(IFlowElementsContainer flowElementsContainer, string id, string name, bool isInterrupting = true) : CatchEvent(flowElementsContainer, id, name, CatchEventLocation.Start)
{
    public bool isInterrupting = isInterrupting;
}