using Neo.Bpms.Domain.Entities.Cmmn.Data.Transaction;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.ApplyUtilities;

public static class BatchDataManipulation<T> where T : new()
{
    public static BatchDataManipulation New(LocalParameters connectionValues, IAuditTrail auditTrail)
    {
        var entity = ProjectDefinition.Project.GetEntity<T>();
        return EntityBatchDataManipulation.New(entity, connectionValues, auditTrail);
    }

    public static AuditTrail BeginBatch(LocalParameters connectionValues, int batchCount = 0)
    {
        var auditTrail = new AuditTrail(TriggerTypeId.BeginBatchInApplyUtility, "");
        auditTrail.Batch = New(connectionValues, auditTrail);
        auditTrail.Batch?.BeginBatch(batchCount);
        return auditTrail;
    }
}

public static class BatchDataManipulationRunner<T> where T : new()
{
    public static BatchDataManipulationRunner New(LocalParameters connectionValues, IAuditTrail auditTrail)
    {
        var entity = ProjectDefinition.Project.GetEntity<T>();
        return EntityBatchDataManipulationRunner.New(entity, connectionValues, auditTrail);
    }
}

public static class EntityBatchDataManipulation

{
    public static BatchDataManipulation New(Entity entity, LocalParameters connectionValues, IAuditTrail auditTrail)
    {
        var runner = EntityBatchDataManipulationRunner.New(entity, connectionValues, auditTrail);
        return entity == null ? null : new BatchDataManipulation(runner);
    }

}

public static class EntityBatchDataManipulationRunner
{
    public static BatchDataManipulationRunner New(Entity entity,
        LocalParameters connectionValues, IAuditTrail auditTrail)
    {
        return new BatchDataManipulationRunner(entity, connectionValues, auditTrail);
    }
}
