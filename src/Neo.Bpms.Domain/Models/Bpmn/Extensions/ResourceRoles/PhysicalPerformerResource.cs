namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class PhysicalPerformerResource(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression) : Performer(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, eRoleType.PhysicalPerformerResource)
{
}