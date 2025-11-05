using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;

public class ProcessManagerResource(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression) : ResourceRole(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, eRoleType.ProcessManagerResource)
{
}