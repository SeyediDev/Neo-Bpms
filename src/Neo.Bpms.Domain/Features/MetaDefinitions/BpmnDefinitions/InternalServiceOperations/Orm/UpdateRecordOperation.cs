using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm;
public class UpdateRecordOperation :
    InternalServiceOperationModel<OperationInput_UpdateRecord, OperationOutput_UpdateRecord>
{
    public UpdateRecordOperation() : base("UpdateRecord")
    {
        Description = "This operation can update a single record or multiple records based on data model definitions.";
    }
}
