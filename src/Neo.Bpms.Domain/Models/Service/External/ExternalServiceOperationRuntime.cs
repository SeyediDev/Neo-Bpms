using System.Net;
using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Domain.Models.Service.External;

public abstract class ExternalServiceOperationRuntime<TIn, TOut> : IExternalServiceOperationRuntime
    where TIn : new()
    where TOut : new()
{
    public void RunOperation(IAuditTrail auditTrail, LocalParameters inData,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        out LocalParameters outData, out HttpStatusCode statusCode)
    {
        TIn inputParams = inData.To<TIn>();
        TOut outputParams = RunOperation(auditTrail, method, endPoint, inputParams, out statusCode);
        outData = outputParams != null ? LocalParameters.DeepToLocalParameters(outputParams) : null;
    }

    public void RunOperationWithRequestBody(IAuditTrail auditTrail, string requestBody,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        out LocalParameters outData, out HttpStatusCode statusCode)
    {
        TOut outputParams = RunOperation(auditTrail, method, endPoint, requestBody, out statusCode);
        outData = outputParams != null ? LocalParameters.DeepToLocalParameters(outputParams) : null;
    }

    public abstract object RunOperationJson(IAuditTrail auditTrail,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        string requestBody, out HttpStatusCode statusCode);

    protected abstract TOut RunOperation(IAuditTrail auditTrail,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        string requestBody, out HttpStatusCode statusCode);
    protected abstract TOut RunOperation(IAuditTrail auditTrail,
        ServiceOperationMethod method, IServiceEndPoint endPoint, TIn input, out HttpStatusCode statusCode);

    public Type InputStructure => typeof(TIn);

    public Type OutputStructure => typeof(TOut);
}
