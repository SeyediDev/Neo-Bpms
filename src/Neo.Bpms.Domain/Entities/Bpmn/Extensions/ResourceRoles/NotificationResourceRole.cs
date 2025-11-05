using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;

public class NotificationResourceRole(IResourceRoleContainer resourceRoleContainer,
        string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression, NotificationTimes notificationTimes) : ResourceRole(resourceRoleContainer, id, name, resourceRef, resourceAssignmentExpression, eRoleType.NotificationResource)
{
    public NotificationTimes notificationTimes = notificationTimes;
}

[Flags]
public enum NotificationTimes
{
    none = 0,
    onNoResource = 0x01,
    onAllocate = 0x02,
    onDo = 0x04,
    onComplete = 0x08,
    onDueTimeExpiration = 0x10,
    onError = 0x20,
    onCancel = 0x40
}