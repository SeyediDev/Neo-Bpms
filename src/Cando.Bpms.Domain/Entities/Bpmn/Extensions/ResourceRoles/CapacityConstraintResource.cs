using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;

public class CapacityConstraintResource(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression, double quantity) : ResourceRole(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression,
        eRoleType.CapacityConstraintResource)
{
    public double quantity = quantity;
}