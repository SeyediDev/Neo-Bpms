using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;

namespace Neo.Bpms.Domain.Models.Bpmn.Choreographies;

public class Choreography(BpmnDefinitions parent, string id, string name) : Collaboration(parent, id, name), IFlowElementsContainer
{
    #region from FlowElementsContainer

    /// <summary>
    /// Flow elements are Events, Gateways, Sequence Flows, Activities, Data Objects, Data Associations, and Choreography Activities.
    /// Note that: 
    /// • Choreography Activities MUST NOT be included as a flowElement for a Process.
    /// • Activities, Data Associations, and Data Objects MUST NOT be included as a flowElement for a Choreography.
    /// </summary>
    public Dictionary<string, FlowElement> flowElements { get; set; } = [];

    public List<LaneSet> laneSets { get; set; } = [];

    #endregion from FlowElementsContainer
    public FlowElement GetFlowElement(string elementName)
    {
        flowElements.TryGetValue(elementName, out FlowElement flowElement);
        return flowElement;
    }

    public FlowNode GetFlowNode(string elementName)
    {
        return (FlowNode)GetFlowElement(elementName);
    }

    public ThrowEvent GetThrowEvent(string elementName)
    {
        return (ThrowEvent)GetFlowElement(elementName);
    }

    public CatchEvent GetCatchEvent(string elementName)
    {
        return (CatchEvent)GetFlowElement(elementName);
    }

    public BoundaryEvent GetBoundaryEvent(string elementName)
    {
        return (BoundaryEvent)GetFlowElement(elementName);
    }
    public Lane findLane(string laneId)
    {
        foreach (LaneSet ls in laneSets)
        {
            Lane lane = ls.findLane(laneId);
            if (lane != null)
                return lane;
        }
        return null;
    }
    public Lane FindLaneByFlowNodeId(string flowNodeId)
    {
        foreach (LaneSet laneset in laneSets ?? Enumerable.Empty<LaneSet>())
        {
            Lane lane = laneset.FindLaneByFlowNodeId(flowNodeId);
            if (lane != null) return lane;
        }
        return null;
    }
}