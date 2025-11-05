using System.Net;
using Neo.Bpms.Domain.Entities.Service.External;
using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Entities.Service.ServiceEndPoint;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public class ServiceOperationManager(IServiceProvider serviceProvider)
{
    public async Task<(LocalParameters outParam, HttpStatusCode statusCode)> CallServiceOperation(
        AuditTrail auditTrail, ServiceInterfaceProtocol serviceInterfaceProtocol, string operationName,
        LocalParameters inputData)
    {
        ServiceOperationDefinition usingOperation = ProjectDefinition.Project.ServiceInterfaces.GetUsingOperation("bpms://",
            serviceInterfaceProtocol, operationName);
        LocalParameters outParams;
        HttpStatusCode statusCode;
        switch (usingOperation)
        {
            case IInternalServiceOperation op:
                if (serviceProvider.GetService(op.RunTimeType) is not IInternalServiceOperationRuntime operationRuntime)
                {
                    throw new Exception($"{op.RunTimeType} is not configured or is of wrong type");
                }
                outParams = await operationRuntime.RunOperation(auditTrail, inputData);
                statusCode = HttpStatusCode.OK;
                break;
            case IExternalServiceOperation op:
                string relativePath = !string.IsNullOrEmpty(usingOperation.RelativePath)
                    ? usingOperation.RelativePath
                    : $"{usingOperation.Interface.Name}/{usingOperation.Name}";
                op.RunTime.RunOperation(auditTrail, inputData,
                    usingOperation.Method, FetchEndPoint(inputData, relativePath), out outParams, out statusCode);
                break;
            default:
                throw new Exception($"Invalid Operation {operationName}");
        }

        return (outParams, statusCode);
    }

    public static object CallNetworkElementServiceOperation(IAuditTrail auditTrail,
        string operationName, long networkElementId, string requestBody, out HttpStatusCode statusCode)
    {
        ServiceOperationDefinition usingOperation = ProjectDefinition.Project.ServiceInterfaces.GetUsingOperation("bpms://",
            ServiceInterfaceProtocol.NetworkElementRest, operationName);
        object outParams;
        //LocalParameters outData;
        statusCode = HttpStatusCode.Ambiguous;
        if (usingOperation is IExternalServiceOperation op)
        {
            string relativePath = !string.IsNullOrEmpty(usingOperation.RelativePath)
                ? usingOperation.RelativePath
                : $"{usingOperation.Interface.Name}/{usingOperation.Name}";
            IServiceEndPoint endPoint = FetchEndPoint(networkElementId, relativePath);
            //op.RunTime.RunOperation(auditTrail, requestBody,
            //	usingOperation.Method, endPoint, out outData, out statusCode);
            outParams = op.RunTime.RunOperationJson(auditTrail,
                usingOperation.Method, endPoint, requestBody, out statusCode);
        }
        else
        {
            throw new Exception($"Invalid Operation {operationName}");
        }

        return outParams;
        //return outData
    }

    private static IServiceEndPoint FetchEndPoint(LocalParameters inputData, string relativePath)
    {
        long? networkElementId = inputData.GetLong("NetworkElement");
        return networkElementId == null
            ? throw new Exception("Please set the network element")
            : FetchEndPoint(networkElementId.Value, relativePath);
    }

    public static IServiceEndPoint FetchEndPoint(long networkElementId, string relativePath)
    {
        throw new NotImplementedException();
    }
}
