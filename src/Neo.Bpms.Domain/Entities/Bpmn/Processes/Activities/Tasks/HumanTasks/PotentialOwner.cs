using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

/// <summary>
/// Potential owners of a User Task are persons who can claim and work on it. 
/// A potential owner becomes the actual owner of a Task, usually by explicitly claiming it.
/// </summary>
public class PotentialOwner(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef, ResourceAssignmentExpression resourceAssignmentExpression, PotentialOwnerType type) : HumanPerformer(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, (eRoleType)(int)type)
{
}

public enum PotentialOwnerType
{
    FirstLevelPotentialOwner = 1,
    DelegatePotentialOwner,
    SupervisorPotentialOwner,
}
