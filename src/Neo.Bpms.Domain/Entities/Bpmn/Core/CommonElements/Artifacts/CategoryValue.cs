using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;

public class CategoryValue(Category category, string id, string value) : BaseElement(category, id)
{
    public string value = value;
}
