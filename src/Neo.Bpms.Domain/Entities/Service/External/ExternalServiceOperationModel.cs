using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Service.External;

public abstract class ExternalServiceOperationModel<TIn, TOut>(string name, ServiceOperationMethod method = ServiceOperationMethod.POST) : ServiceOperationDefinition(null, name, name, null, null, method),
    IExternalServiceOperation
    where TIn : new()
    where TOut : new()
{
    public abstract void AddDescription();
    public ExternalServiceOperationRuntime<TIn, TOut> Runtime { get; set; }

    public IExternalServiceOperationRuntime RunTime
    {
        get => Runtime;
        set => Runtime = value as ExternalServiceOperationRuntime<TIn, TOut>;
    }

    public Type InputStructure => typeof(TIn);
    public Type OutputStructure => typeof(TOut);
}
