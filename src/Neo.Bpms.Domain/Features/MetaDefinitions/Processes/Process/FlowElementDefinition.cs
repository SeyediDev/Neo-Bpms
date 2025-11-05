global using Neo.Bpms.Domain.Features.MetaDefinitions.Processes.Process;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class FlowElementDefinition<TFlowElement> : BaseElementDefinition<TFlowElement>
    where TFlowElement : FlowElement
{
    protected override TFlowElement Creation(BaseElement parent)
    {
        return FlowElementCreation(parent as IFlowElementsContainer);
    }

    protected abstract TFlowElement FlowElementCreation(IFlowElementsContainer flowElementsContainer);

    protected sealed override void AddToDefinitions(BaseElement parent)
    {
        if (parent is IFlowElementsContainer flowElementsContainer)
        {
            flowElementsContainer.flowElements.Add(Element.Id, Element);
        }
    }
}

public abstract partial class ProcessDefinition
{
    private void AddFlowElement(FlowElement element)
    {
        currentBaseElement = element;
        if (currentSubProcess != null)
            currentSubProcess.flowElements.Add(element.Id, element);
        else
            process.flowElements.Add(element.Id, element);
    }
}
