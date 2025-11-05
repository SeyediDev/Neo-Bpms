using Neo.Bpms.Domain.Entities.Bpmn.Choreographies;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Auditing;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Monitoring;
using Neo.Bpms.Domain.Model.BPMN.Processes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

/// <summary>
/// FlowElement is the abstract super class for all elements that can appear in a Process flow, which are FlowNodes 
/// which consist of (Activities, Choreography Activities, Gateways, and Events), 
/// Data Objects, Data Associations, and Sequence Flows.
/// </summary>
public class FlowElement(IFlowElementsContainer flowElementsContainer, string id, string name,
    FlowElement.eFlowElementType flowElementType) : FlowElementsContainerItem(flowElementsContainer, id, name),
        IAuditingContainer,
        IMonitoringContainer
{
    //		public string name { get; set; }

    ///// <summary>
    ///// A reference to the Category Values that are associated with this Flow Element.
    ///// </summary>
    public List<CategoryValue> categoryValueRef;

    /// <summary>
    /// A hook for specifying audit related properties. Auditing can only be defined for a Process.
    /// </summary>
    public Auditing auditing { get; set; }

    /// <summary>
    /// A hook for specifying monitoring related properties. Monitoring can only be defined for a Process.
    /// </summary>
    public Monitoring monitoring { get; set; }

    //		public string Name => name;
    public IFlowElementsContainer FlowElementsContainer => Parent as IFlowElementsContainer;

    public Choreography Choreography => FlowElementsContainer as Choreography;
    public Process Process => FlowElementsContainer as Process;
    public SubProcess SubProcess => FlowElementsContainer as SubProcess;
    public bool InSubProcess => SubProcess != null;

    public eFlowElementType flowElementType { get; set; } = flowElementType;

    public enum eFlowElementType
    {
        Gateway = 1,
        Activity = 2,
        Event = 4,
        ChoreographyActivity = 11,

        DataObject = 21,
        DataObjectRef = 23,
        DataStoreRef = 24,
        SequenceFlow = 31
    }
}
