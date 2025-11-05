namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;

/// <summary>
/// The FlowNode element is used to provide a single element as the source and target Sequence Flow associations.
/// Only the Gateway, Activity, Choreography Activity, and Event elements can connect to Sequence Flows
/// and thus, these elements are the only ones that are sub-classes of FlowNode.
/// </summary>
public class FlowNode(IFlowElementsContainer flowElementsContainer, string id, string name, FlowNode.eFlowNodeType nodeType) : FlowElement(flowElementsContainer, id, name, (eFlowElementType)nodeType)
{
    /// <summary>
    /// the incoming Sequence Flow of the FlowNode
    /// </summary>
    public List<SequenceFlow> incoming = [];

    /// <summary>
    /// the outgoing Sequence Flow of the FlowNode. This is an ordered collection.
    /// </summary>
    public List<SequenceFlow> outgoing = [];

    public eFlowNodeType nodeType { get; set; } = nodeType;
    public int? outputStateId { get; set; }
    public int? inputStateId { get; set; }
    public bool HasIncoming => incoming != null && incoming.Count > 0;

    public enum eFlowNodeType
    {
        Gateway = 1,
        Activity = 2,
        Event = 4,
        ChoreographyActivity = 11
    }
}
