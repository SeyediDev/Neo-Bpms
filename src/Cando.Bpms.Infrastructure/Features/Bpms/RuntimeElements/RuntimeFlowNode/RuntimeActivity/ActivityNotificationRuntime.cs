using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public abstract partial class ActivityRuntime
{
    protected void Notificate(ResourceRole notificationResourceRole, ActivityInstance ai, string note)
    {
        var description = GetAiNotificationDescription(ai, note);
        AuditTrace(notificationResourceRole.Name + " " + description, ai.pi, ai);
        //todo
    }

    protected void Notificate(IdentityUser user, ActivityInstance ai, string note)
    {
        var description = GetAiNotificationDescription(ai, note);
        AuditTrace(user.UserName + " " + description, ai.pi, ai);
    }

    private static string GetAiNotificationDescription(ActivityInstance ai, string note)
    {
        var entityDescription = "Id" + ai.pi.EntityPkv;//todo
        var description = $"In {ai.activity.Name} {entityDescription} : {note}";
        return description;
    }
}
