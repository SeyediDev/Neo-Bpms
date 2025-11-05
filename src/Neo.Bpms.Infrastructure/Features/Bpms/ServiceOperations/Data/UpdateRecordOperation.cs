using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.Data;

public class UpdateRecordOperationRuntime :
    InternalServiceOperationRuntime<OperationInput_UpdateRecord, OperationOutput_UpdateRecord>
{
    protected override Task<OperationOutput_UpdateRecord> RunOperationAsync(
        IAuditTrail auditHeader, OperationInput_UpdateRecord input)
    {
        var nameSpace = input.NameSpace;
        var entity = input.Entity;
        var whereClause = input.WhereClause;
        var updatingFields = input.UpdatingFields;
        var r = updatingFields as ElasticObject ?? new ElasticObject("", updatingFields);
        bool success = new ApplyUtility(nameSpace, entity, auditHeader, null)
            .UpdateWithFilter(whereClause, r);
        return !success
            ? throw new Exception("Update record failed")
            : Task.FromResult(new OperationOutput_UpdateRecord()
            {
                Succussfull = true
            });
    }
}
