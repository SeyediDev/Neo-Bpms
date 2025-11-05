namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class ProcessManagerResource(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression) : ResourceRole(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, eRoleType.ProcessManagerResource)
{
}