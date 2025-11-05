namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class Confirmer(IResourceRoleContainer resourceRoleContainer,
    string id, string name, HumanResource resource,
    ResourceAssignmentExpression resourceAssignmentExpression, int confirmStepId) : ResourceRole(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, eRoleType.Confirmer)
{
    public int confirmStepId = confirmStepId;
}
