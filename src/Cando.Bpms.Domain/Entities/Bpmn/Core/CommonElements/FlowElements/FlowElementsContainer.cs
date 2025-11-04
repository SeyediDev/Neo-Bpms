using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

/// <summary>
/// FlowElementsContainer is an abstract super class for BPMN diagrams (or views) and defines the superset of elements that are contained in those diagrams.
/// There are four (4) types of FlowElementsContainers: Process, Sub-Process, Choreography, and Sub-Choreography.
/// </summary>
public interface IFlowElementsContainer
{
    /// <summary>
    /// Flow elements are Events, Gateways, Sequence Flows, Activities, Data Objects, Data Associations, and Choreography Activities.
    /// Note that: 
    /// • Choreography Activities MUST NOT be included as a flowElement for a Process.
    /// • Activities, Data Associations, and Data Objects MUST NOT be included as a flowElement for a Choreography.
    /// </summary>
    Dictionary<string, FlowElement> flowElements { get; set; }

    //	/// <summary>
    //	/// LaneSets are not used for Choreographies or Sub-Choreographies
    //	/// </summary>
    List<LaneSet> laneSets { get; set; }
    //	public enum eType
    //	{
    //		Process = 1, SubProcess = 2,
    //		Choreography = 11, SubChoreography = 12,
    //	};

    FlowElement GetFlowElement(string elementName);
    FlowNode GetFlowNode(string elementName);
    ThrowEvent GetThrowEvent(string elementName);
    CatchEvent GetCatchEvent(string elementName);
    BoundaryEvent GetBoundaryEvent(string elementName);
    Lane findLane(string laneId);
    Lane FindLaneByFlowNodeId(string flowNodeId);
}
