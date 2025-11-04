using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract class LaneDefinition : BaseElementDefinition<Lane>
{
    /*protected override Lane Creation(BaseElement parent)
		{
			return FlowElementCreation(parent as IFlowElementsContainer);
		}

		protected override void AddToDefinitions(BaseElement parent)
		{
			if (parent is IFlowElementsContainer flowElementsContainer)
			{
				flowElementsContainer.flowElements.Add(Element.Id, Element);
			}
		}*/
}

public abstract partial class ProcessDefinition
{
    private LaneSet _currentLaneSet;
    private Lane _currentLane;

    /// <summary>
    /// Adds the lane.  for detail descriptions please refer to the model.
    /// </summary>
    /// <param name="laneId">The lane identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="parentLaneId">The parent lane identifier.</param>
    /// <returns></returns>
    protected bool AddLane(string laneId, string name, string parentLaneId = "")
    {
        if (_flowElementsContainer is not BaseElement flowElementsContainerElement) return false;//todo

        if (process.laneSets.Count == 0)
            _currentLaneSet = new LaneSet(_flowElementsContainer,
                flowElementsContainerElement.Id + ".BaseLaneSet", name, null);
        else _currentLaneSet ??= _flowElementsContainer.laneSets[0];
        if (!string.IsNullOrEmpty(parentLaneId))
        {
            var parentLane = process.findLane(parentLaneId);
            parentLane.childLaneSet ??= new LaneSet(_flowElementsContainer,
                    parentLane.Id, parentLane.Name, parentLane);
            _currentLaneSet = parentLane.childLaneSet;
        }

        if (_currentLaneSet == null) return false;
        currentBaseElement = _currentLaneSet;
        _currentLane = new Lane(process, laneId, name);
        currentBaseElement = _currentLane;
        _currentLaneSet.lanes.Add(_currentLane);
        return true;
    }

    /// <summary>
    /// Sets the lane default performer.
    /// </summary>
    /// <param name="humanResourceId">The human resource identifier.</param>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    protected bool SetLaneDefaultPerformer(string humanResourceId, PotentialOwnerType type, string name = null)
    {
        if (_currentLane == null) return false;
        _currentResource = ProjectDefinition.Project.GetResource(humanResourceId);
        if (CurrentHumanResource == null)
            throw new Exception($"can not find resource {humanResourceId} for lane {_currentLane?.Id}");
        _currentLane.defaultPerformer =
            new PotentialOwner(_currentLane, $"{_currentLane?.Id}.DefaultPerformer.{humanResourceId}",
                name, CurrentHumanResource, /*todo*/null, type);
        _currentResourceRole = _currentLane.defaultPerformer;
        return true;
    }

    /// <summary>
    /// Finds the lane.
    /// </summary>
    /// <param name="laneId">The lane identifier.</param>
    /// <returns></returns>
    private Lane FindLane(string laneId)
    {
        var lane = process.findLane(laneId);
        if (lane != null) return lane;
        foreach (var item in process.flowElements.Values)
        {
            if (item is not SubProcess sp) continue;
            lane = sp.findLane(laneId);
            if (lane != null)
                return lane;
        }

        return null;
    }
}