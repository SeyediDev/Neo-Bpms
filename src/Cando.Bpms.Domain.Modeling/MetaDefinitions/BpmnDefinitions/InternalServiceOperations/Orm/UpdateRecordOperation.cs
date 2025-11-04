using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm;
public class UpdateRecordOperation :
    InternalServiceOperationModel<OperationInput_UpdateRecord, OperationOutput_UpdateRecord>
{
    public UpdateRecordOperation() : base("UpdateRecord")
    {
        Description = "This operation can update a single record or multiple records based on data model definitions.";
    }
}
