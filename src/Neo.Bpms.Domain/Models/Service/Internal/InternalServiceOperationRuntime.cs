using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Domain.Models.Service.Internal;

public abstract class InternalServiceOperationRuntime<TIn, TOut> : IInternalServiceOperationRuntime
    where TIn : new()
    where TOut : new()
{
    protected abstract Task<TOut> RunOperationAsync(IAuditTrail auditTrail, TIn input);
    public async Task<LocalParameters> RunOperation(IAuditTrail auditTrail, LocalParameters inData)
    {
        TIn inputParams = inData.To<TIn>();
        TOut outputParams = await RunOperationAsync(auditTrail, inputParams);
        return LocalParameters.DeepToLocalParameters(outputParams);
    }

    public Type InputStructure => typeof(TIn);

    public Type OutputStructure => typeof(TOut);
}
