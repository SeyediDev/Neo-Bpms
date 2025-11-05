using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;

/// <summary>
/// A Sub-Process is an Activity whose internal details have been modeled using Activities, 
/// Gateways, Events, and Sequence Flows. 
/// A Sub-Process is a graphical object within a Process, but it also can be “opened up” to show a
/// lower-level Process. Sub-Processes define a contextual scope that can be used for attribute visibility, 
/// transactional scope, for the handling of exceptions, of Events, or for compensation.
/// 
/// The Sub-Process now corresponds to the Embedded Sub-Process
/// The Reusable Sub-Process is modeled by process and is called by Call Activity
/// </summary>
public class SubProcess : Activity, IFlowElementsContainer, IArtifactContainer
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

    /// <summary>
    /// A flag that identifies whether this Sub-Process is an Event Sub-Process.
    /// • If false, then this Sub-Process is a normal Sub-Process.
    /// • If true, then this Sub-Process is an Event Sub-Process and is subject to additional constraints
    /// </summary>
    /// <remark>
    /// An Event Sub-Process is a specialized Sub-Process that is used within a Process (or Sub-Process). 
    /// A Sub-Process is defined as an Event Sub-Process when its triggeredByEvent attribute is set to true.
    /// An Event Sub-Process is not part of the normal flow of its parent Process—there are no incoming or 
    /// outgoing Sequence Flows.
    /// 
    /// An Event Sub-Process MAY or MAY NOT occur while the parent Process is active, but it is possible that it will
    /// occur many times. Unlike a standard Sub-Process, which uses the flow of the parent Process as a trigger, an Event
    /// Sub-Process has a Start Event with a trigger. Each time the Start Event is triggered while the parent Process is
    /// active, then the Event Sub-Process will start.
    /// 
    /// There are two possible consequences to the parent Process when an Event Sub-Process is triggered: 
    /// 1) the parent Process can be interrupted, and 2) the parent Process can continue its work (not interrupted). 
    /// This is determined by the type of Start Event that is used.
    /// </remark>
    public bool triggeredByEvent;

    /// <summary>
    /// This attribute provides the list of Artifacts that are contained within the Sub-Process.
    /// </summary>
    public List<Artifact> artifacts { get; set; }

    public SubProcess(IFlowElementsContainer flowElementsContainer,
        string id, string name, bool triggeredByEvent)
        : base(flowElementsContainer, id, name, triggeredByEvent ? eActivityType.EventSubProcess : eActivityType.EmbeddedSubProcess)
    {
        this.triggeredByEvent = triggeredByEvent;
    }

    protected SubProcess(IFlowElementsContainer flowElementsContainer,
        string id, string name, eActivityType activityType)
        : base(flowElementsContainer, id, name, activityType)
    {
        triggeredByEvent = false;
    }
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
