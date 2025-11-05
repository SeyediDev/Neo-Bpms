using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Service.Internal;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.Data;

public class InsertRecordOperationRuntime :
    InternalServiceOperationRuntime<OperationInput_InsertRecord, OperationOutput_InsertRecord>
{

    protected override Task<OperationOutput_InsertRecord> RunOperationAsync(
        IAuditTrail auditHeader, OperationInput_InsertRecord input)
    {
        var nameSpace = input.NameSpace;
        var entity = input.Entity;
        var updatingFields = input.UpdatingFields;
        var r = updatingFields as ElasticObject ?? new ElasticObject("", updatingFields);
        bool success = new ApplyUtility(nameSpace, entity, auditHeader, null)
            .Insert(r);
        return !success
            ? throw new Exception("Insert record failed")
            : Task.FromResult(new OperationOutput_InsertRecord()
            {
                Successful = true
            });
    }
}
