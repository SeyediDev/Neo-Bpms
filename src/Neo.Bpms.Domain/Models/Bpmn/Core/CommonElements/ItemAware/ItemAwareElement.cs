namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

public static class ItemAwareElement
{
    public static bool CheckItem(IItemAwareElement itemAwareElement, string id, string name)
    {
        if (itemAwareElement is not BaseElement baseElement) return false;
        if (!string.IsNullOrEmpty(id))
        {
            if (baseElement.Id == id)
                return true;
        }
        if (!string.IsNullOrEmpty(name))
        {
            if (itemAwareElement.Name == name)
                return true;
        }
        return false;
    }
}

