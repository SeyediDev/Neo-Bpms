using System.Net;
using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Domain.Models.Service.External;

public interface IExternalServiceOperationRuntime : IServiceOperationRuntime
{
    void RunOperation(IAuditTrail auditTrail, LocalParameters inData,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        out LocalParameters outData, out HttpStatusCode statusCode);
    void RunOperationWithRequestBody(IAuditTrail auditTrail, string requestBody,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        out LocalParameters outData, out HttpStatusCode statusCode);

    object RunOperationJson(IAuditTrail auditTrail,
        ServiceOperationMethod method, IServiceEndPoint endPoint,
        string requestBody, out HttpStatusCode statusCode);
}
