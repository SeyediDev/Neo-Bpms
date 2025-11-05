using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataFlow;

public class DataObject : DataFlowElement, IFieldAwareElement
{
    public bool isCollection;

    public DataObject(IFlowElementsContainer flowElementsContainer, string id, string name, ItemDefinition item, bool isCollection)
        : base(flowElementsContainer, id, name, eDataFlowElementTypes.DataObject)
    {
        itemSubjectRef = item;
        this.isCollection = item?.isCollection ?? isCollection;
    }

    public DataObject(IFlowElementsContainer flowElementsContainer, string id, string name, ItemDefinition item)
        : this(flowElementsContainer, id, name, item, item.isCollection)
    {
    }

    public string fieldId => itemSubjectRef?.entityField?.Id ?? Name;
    public EntityField field => itemSubjectRef?.entityField;
}
