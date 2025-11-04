using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

public class LaneSet : FlowElementsContainerItem
{
    //		public string name;

    public List<Lane> lanes = [];

    public LaneSet(IFlowElementsContainer flowElementsContainer, string id, string name, Lane parentLane) :
        base(flowElementsContainer, id, name)
    {
        //this.name = name;
        this.parentLane = parentLane;
        if (parentLane != null)
            parentLane.childLaneSet = this;
        flowElementsContainer.laneSets.Add(this);
    }

    public Lane parentLane { get; set; }

    internal Lane findLane(string laneId)
    {
        foreach (Lane lane in lanes)
        {
            if (lane.Id == laneId)
                return lane;
            Lane ln = lane.childLaneSet?.findLane(laneId);
            if (ln != null)
                return ln;
        }
        return null;
    }

    public Lane FindLaneByFlowNodeId(string flowNodeId)
    {
        foreach (Lane lane in lanes ?? Enumerable.Empty<Lane>())
        {
            if (lane.flowNodeRefs == null) continue;
            if (lane.flowNodeRefs.Any(f => f == flowNodeId))
                return lane;
            Lane childLane = lane.childLaneSet?.FindLaneByFlowNodeId(flowNodeId);
            if (childLane != null)
                return childLane;
        }
        return null;
    }
}