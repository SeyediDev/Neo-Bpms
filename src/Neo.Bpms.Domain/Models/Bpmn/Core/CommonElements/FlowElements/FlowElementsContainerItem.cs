namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;

public class FlowElementsContainerItem(IFlowElementsContainer parent, string id, string name = null) : BaseElement(parent as BaseElement, id, name)
{
}
