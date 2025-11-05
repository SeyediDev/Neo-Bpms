using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;

public class Property : DataElement
{
    public Property(IPropertyContainer propertyContainer, string id, string name, ItemDefinition item) :
        base(propertyContainer, id, name, eDataElementTypes.Property) => itemSubjectRef = item;
}
