using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm;
public class InsertRecordOperation :
    InternalServiceOperationModel<OperationInput_InsertRecord, OperationOutput_InsertRecord>
{
    public InsertRecordOperation() : base("InsertRecord")
    {
        Description =
        "This operation can insert a single record based on data model definitions.";
    }
}
