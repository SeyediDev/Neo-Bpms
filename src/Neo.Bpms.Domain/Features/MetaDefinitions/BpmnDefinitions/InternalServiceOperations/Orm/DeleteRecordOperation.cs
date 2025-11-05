using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm;
public class DeleteRecordOperation :
    InternalServiceOperationModel<OperationInput_DeleteRecord, OperationOutput_DeleteRecord>
{
    public DeleteRecordOperation() : base("DeleteRecord")
    {
        Description =
        "This operation can delete a single record or multiple records based on data model definitions.";
    }
}
