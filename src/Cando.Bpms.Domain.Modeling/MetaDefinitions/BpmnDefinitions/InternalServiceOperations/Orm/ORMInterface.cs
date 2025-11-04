using Neo.Bpms.Domain.Entities.Service.Internal;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm;

public class ORMInterface : InternalServiceGroupDefinition
{
    public ORMInterface() : base("ORMInterface")
    {
        Description = "Object relationship mapping and database operations";
    }

    public override void AddOperations()
    {
        AddOperation(new DeleteRecordOperation());
        AddOperation(new UpdateRecordOperation());
        AddOperation(new InsertRecordOperation());
        //AddOperation(new InsertRecordFromQueryOperation());
        //AddOperation(new BulkInsertRecordOperation());
        //AddOperation(new SelectRecordOperation());
    }
}
