using System.Net;
using Neo.Bpms.Domain.Models.Service.ServiceOperation;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

public class FormServiceOperation(ServiceOperationManager serviceOperationManager, ILogger<FormServiceOperation> logger)
{
    public async Task<ExceptionInfos> CallApplyServiceOperation(ServiceOperationFormData result,
        Form form, AuditTrail auditTrail)
    {
        string operationName = form.ApplyServiceOperation;
        ServiceInterfaceProtocol serviceInterfaceProtocol = form.ServiceInterfaceProtocol;
        LocalParameters inputData = result.FilterValues.DeepToLocalParameters();
        ExceptionInfos errors = [];
        try
        {
            (LocalParameters outParam, HttpStatusCode statusCode) = await serviceOperationManager.CallServiceOperation(auditTrail,
                serviceInterfaceProtocol, operationName, inputData);
            result.Response = outParam?.ToElastic();
        }
        catch (ServiceOperationException e)
        {
            logger.LogError(e, e.Message);
            ExceptionInfos.AddError(ref errors, "", e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            ExceptionInfos.AddError(ref errors, "", Texts.SystemError);
        }

        return errors;
    }
    public async Task<ExceptionInfos> CallGetServiceOperation(ServiceOperationFormData result,
        Form form, AuditTrail auditTrail)
    {
        string operationName = form.GetServiceOperation;
        ServiceInterfaceProtocol serviceInterfaceProtocol = form.ServiceInterfaceProtocol;
        LocalParameters inputData = result.FilterValues.ToLocalParameters();
        try
        {
            (LocalParameters outParam, HttpStatusCode statusCode) operationResult = await serviceOperationManager.CallServiceOperation(auditTrail,
                serviceInterfaceProtocol, operationName, inputData);
            result.Response = operationResult.outParam?.DeepToElastic();
        }
        catch (ServiceOperationException e)
        {
            logger.LogError(e, e.Message);
            return new ExceptionInfos().Add("", e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
            return new ExceptionInfos().Add("", "An error occurred.");
        }

        return null;
    }

    public Task<(LocalParameters outParam, HttpStatusCode statusCode)> 
        CallServiceOperation(AuditTrail auditTrail, ServiceInterfaceProtocol serviceInterfaceProtocol, 
        string operationName, LocalParameters inputData)
    {
        return serviceOperationManager.CallServiceOperation(auditTrail,
            serviceInterfaceProtocol, operationName, inputData);
    }
}
