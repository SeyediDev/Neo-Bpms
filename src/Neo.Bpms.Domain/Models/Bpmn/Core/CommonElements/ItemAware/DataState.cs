namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

public class DataState(IItemAwareElement parent, string id, string name) : BaseElement(parent as BaseElement, id, name)
{
    //public string name;

}
