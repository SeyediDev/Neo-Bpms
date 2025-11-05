using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Service.Internal;

public abstract class InternalServiceGroupDefinition(string name) : ServiceGroupDefinition(name, name)
{
    public abstract void AddOperations();

    public void AddOperation<TIn, TOut>(InternalServiceOperationModel<TIn, TOut> operation)
        where TIn : new()
        where TOut : new()
    {
        Operations.Add(operation.Name, operation);
    }
    //public void AddOperation<T, TIn, TOut>() 
    //	where T: InternalServiceOperationModel<TIn, TOut>, new()
    //	where TIn : new()
    //	where TOut : new()
    //{
    //	var operation = new T();
    //	Operations.Add(operation.name, operation);
    //}
}
