using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

/// <summary>
/// People can be assigned to Activities in various roles (called “generic human roles” in WS-HumanTask). 
/// BPMN 1.2 traditionally only has the Performer role. In addition to supporting the Performer role, 
/// BPMN 2.0 defines a specific HumanPerformer element allowing specifying more specific human roles as specialization of HumanPerformer, 
/// such as PotentialOwner.
/// </summary>
public class HumanPerformer : Performer
{
    protected HumanPerformer(IResourceRoleContainer resourceRoleContainer,
        string id, string name, Resource resourceRef,
        ResourceAssignmentExpression resourceAssignmentExpression, eRoleType type) :
        base(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, type)
    {
    }
}
