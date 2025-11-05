using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.CallActivity;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;

namespace Neo.Bpms.Domain.Model.BPMN.Processes;

public partial class Process(BpmnDefinitions parent, string id, string name, Entity entity) : CallableElement(parent, id, name)
{
    public BpmnDefinitions BpmnDefinitions => Parent as BpmnDefinitions;
    public Entity Entity { get; set; } = entity;
    public string EntityNamespaceId => Entity?.NamespaceId;
    public string EntityId => Entity?.Id;
    public string StateProperty { get; set; } = "StateId";
    public string DisplayFields { get; set; }
    public ProcessStatus Status { get; set; }

    public enum ProcessStatus
    {
        Draft = 1,
        Test = 2,
        Final = 3,
        Active = 4
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

    public Activity GetActivity(string elementName)
    {
        return (Activity)GetFlowNode(elementName);
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
            {
                return lane;
            }
        }

        return null;
    }

    public Lane FindLaneByFlowNodeId(string flowNodeId)
    {
        foreach (LaneSet laneSet in laneSets ?? Enumerable.Empty<LaneSet>())
        {
            Lane lane = laneSet.FindLaneByFlowNodeId(flowNodeId);
            if (lane != null)
            {
                return lane;
            }
        }

        return null;
    }

    public Property GetProperty(string property)
    {
        return properties?.FirstOrDefault(p => p.Name == property);
    }

    public Property EntityPkProperty(Entity entity)
    {
        return properties?.FirstOrDefault(p => p.itemSubjectRef?.entityField?.IncludeInPkv ?? false);
    }
}
