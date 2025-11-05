namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class CostingResource(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression, bool isDirect, double quantity) 
    : ResourceRole(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, eRoleType.CostingResource)
{
    public bool isDirect = isDirect;
    public double quantity = quantity;
}
