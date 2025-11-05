using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;

public class AuthorizedUser(IResourceRoleContainer resourceRoleContainer,
    string id, string name, HumanResource resource,
    ResourceAssignmentExpression resourceAssignmentExpression) 
    : ResourceRole(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, eRoleType.AuthorizedUser)
{
}
