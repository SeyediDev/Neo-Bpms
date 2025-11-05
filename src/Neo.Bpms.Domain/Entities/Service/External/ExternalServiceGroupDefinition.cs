using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Service.External;

public abstract class ExternalServiceGroupDefinition(string name) : ServiceGroupDefinition(name, name)
{
    public abstract void AddDescription();
    public abstract void AddOperations();

    public void AddOperation<TIn, TOut>(ExternalServiceOperationModel<TIn, TOut> operation)
        where TIn : new()
        where TOut : new()
    {
        Operations.Add(operation.Name, operation);
    }
}
