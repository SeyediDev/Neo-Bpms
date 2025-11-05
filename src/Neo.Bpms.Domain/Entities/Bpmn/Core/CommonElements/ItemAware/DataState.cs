using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

public class DataState(IItemAwareElement parent, string id, string name) : BaseElement(parent as BaseElement, id, name)
{
    //public string name;

}
