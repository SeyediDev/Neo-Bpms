using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Domain.Model.BPMN.Processes;

public partial class DataStoreReference(IFlowElementsContainer flowElementsContainer,
    string id, string name, DataStore dataStore) : DataFlowElement(flowElementsContainer, id, name, eDataFlowElementTypes.DataStoreRef)
{
    public DataStore dataStore = dataStore;
}
