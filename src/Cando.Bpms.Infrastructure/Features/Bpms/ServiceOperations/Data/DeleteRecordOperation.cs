using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.Data;

public class DeleteRecordOperationRuntime :
    InternalServiceOperationRuntime<OperationInput_DeleteRecord, OperationOutput_DeleteRecord>
{
    protected override Task<OperationOutput_DeleteRecord> RunOperationAsync(
        IAuditTrail auditHeader, OperationInput_DeleteRecord input)
    {
        var nameSpace = input.NameSpace;
        var entity = input.Entity;
        var whereClause = input.WhereClause;
        if (whereClause == null)
        {
            throw new Exception("where clause can not be null");
        }

        bool success = new ApplyUtility(nameSpace, entity, auditHeader, null)
            .DeleteWithFilter(whereClause);
        return !success
            ? throw new Exception("Delete record failed")
            : Task.FromResult(new OperationOutput_DeleteRecord()
            {
                Successful = true
            });
    }
}
