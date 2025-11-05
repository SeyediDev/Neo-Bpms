namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class AuthorizedUser(IResourceRoleContainer resourceRoleContainer,
    string id, string name, HumanResource resource,
    ResourceAssignmentExpression resourceAssignmentExpression) 
    : ResourceRole(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, eRoleType.AuthorizedUser)
{
}
