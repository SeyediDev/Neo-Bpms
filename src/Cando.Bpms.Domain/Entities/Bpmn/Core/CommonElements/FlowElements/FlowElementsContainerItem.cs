using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

public class FlowElementsContainerItem(IFlowElementsContainer parent, string id, string name = null) : BaseElement(parent as BaseElement, id, name)
{
}
