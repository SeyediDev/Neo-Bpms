namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class CapacityConstraintResource(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression, double quantity) : ResourceRole(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression,
        eRoleType.CapacityConstraintResource)
{
    public double quantity = quantity;
}