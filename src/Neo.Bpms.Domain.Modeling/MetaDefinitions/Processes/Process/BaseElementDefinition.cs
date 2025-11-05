using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Processes.Process;

public abstract class BaseElementDefinition<TElement> : BaseModelingDefinition
    where TElement : BaseElement
{
    public TElement Element { get; set; }

    public void Init(BaseElement parent)
    {
        Element = Creation(parent);
        currentBaseElement = Element;
        PreDefinitions();
        Definitions();
        AddToDefinitions(parent);
    }

    protected virtual string ElementId => GetType().Name;

    protected abstract TElement Creation(BaseElement parent);
    protected abstract string Name { get; }

    protected abstract void Definitions();

    protected virtual void PreDefinitions()
    {
    }

    protected abstract void AddToDefinitions(BaseElement parent);
}
