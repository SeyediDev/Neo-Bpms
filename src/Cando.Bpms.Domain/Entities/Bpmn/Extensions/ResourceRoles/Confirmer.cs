using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;

public class Confirmer(IResourceRoleContainer resourceRoleContainer,
    string id, string name, HumanResource resource,
    ResourceAssignmentExpression resourceAssignmentExpression, int confirmStepId) : ResourceRole(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, eRoleType.Confirmer)
{
    public int confirmStepId = confirmStepId;
}
