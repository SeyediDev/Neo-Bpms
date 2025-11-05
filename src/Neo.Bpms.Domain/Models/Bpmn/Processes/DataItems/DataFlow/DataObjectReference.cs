using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

public class DataObjectReference(IFlowElementsContainer flowElementsContainer, string id, string name,
    DataObject dataObject) : DataFlowElement(flowElementsContainer, id, name, eDataFlowElementTypes.DataObjectRef), IFieldAwareElement
{
    public DataObject dataObject = dataObject;

    public string fieldId => dataObject?.fieldId ?? itemSubjectRef?.entityField?.Id ?? Name;
    public EntityField field => dataObject?.field ?? itemSubjectRef?.entityField;
}
