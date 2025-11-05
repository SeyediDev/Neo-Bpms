using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;

public class DataElement(IDataElementContainer parent, string id, string name, DataElement.eDataElementTypes dataElementType) : BaseElement(parent as BaseElement, id, name), IFieldAwareElement
{
    //		public string name { get; set; }
    public ItemDefinition itemSubjectRef { get; set; }
    public DataState dataState { get; set; }
    public bool CheckItem(string itemId, string itemName)
    {
        return ItemAwareElement.CheckItem(this, itemId, itemName);
    }

    public string fieldId => itemSubjectRef?.entityField?.Id ?? Name;
    public EntityField field => itemSubjectRef?.entityField;

    public eDataElementTypes type { get; set; } = dataElementType;

    public enum eDataElementTypes
    {
        Property,
        DataInput,
        DataOutput
    }
}

public interface IDataElementContainer : IItemAwareContainer
{
}
