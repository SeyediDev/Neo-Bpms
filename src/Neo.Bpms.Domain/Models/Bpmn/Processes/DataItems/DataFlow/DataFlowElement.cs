namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

public class DataFlowElement(IFlowElementsContainer flowElementsContainer, string id, string name, DataFlowElement.eDataFlowElementTypes type) : FlowElement(flowElementsContainer, id, name, (eFlowElementType)type), IItemAwareElement
{
    public ItemDefinition itemSubjectRef { get; set; }
    public DataState dataState { get; set; }
    public bool CheckItem(string itemId, string itemName)
    {
        return ItemAwareElement.CheckItem(this, itemId, itemName);
    }

    public eDataFlowElementTypes type => (eDataFlowElementTypes)flowElementType;

    public enum eDataFlowElementTypes
    {
        DataObject = eFlowElementType.DataObject,
        DataObjectRef = eFlowElementType.DataObjectRef,
        DataStoreRef = eFlowElementType.DataStoreRef
    }
}
