using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataFlow;

public class DataObjectReference(IFlowElementsContainer flowElementsContainer, string id, string name,
    DataObject dataObject) : DataFlowElement(flowElementsContainer, id, name, eDataFlowElementTypes.DataObjectRef), IFieldAwareElement
{
    public DataObject dataObject = dataObject;

    public string fieldId => dataObject?.fieldId ?? itemSubjectRef?.field?.Id ?? Name;
    public EntityField field => dataObject?.field ?? itemSubjectRef?.field;
}
