namespace Neo.Bpms.Domain.Models.Service.External;

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
