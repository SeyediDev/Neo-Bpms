using Neo.Bpms.Domain.Entities.Cmmn.Data.Transaction;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.ApplyUtilities;

public static class AuditTrail<T> where T : new()
{
    public static AuditTrail New(LocalParameters connectionParameters, TriggerTypeId triggerTypeId,
        string title, IdentityUser user)
    {
        var entity = ProjectDefinition.Project.GetEntity<T>();
        return EntityAuditTrail.New(entity, triggerTypeId, title, user, 0 /*todo*/, connectionParameters);
    }

    public static AuditTrail BeginBatch(IdentityUser user, LocalParameters connectionParameters,
        int batchCount = 0)
    {
        var entity = ProjectDefinition.Project.GetEntity<T>();
        var auditTrail = New(connectionParameters, TriggerTypeId.Business, "Business", user);
        auditTrail.BeginBatch(EntityBatchDataManipulationRunner.New(entity, connectionParameters, auditTrail), batchCount);
        return auditTrail;
    }
}

public static class EntityAuditTrail
{
    public static AuditTrail New(Entity entity, TriggerTypeId triggerTypeId, string title,
        IdentityUser user, long userGroupId, LocalParameters connectionParameters)
    {
        var auditTrail = new AuditTrail(triggerTypeId, title, user, userGroupId);
        auditTrail.Batch = new BatchDataManipulation(EntityBatchDataManipulationRunner.New(entity, connectionParameters, auditTrail));
        return auditTrail;
    }
}
