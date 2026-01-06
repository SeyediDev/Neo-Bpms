using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Domain.Model.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.UI.MVC.Exceptions;

namespace Neo.Bpms.Api.Modules.EditableGrid.Services;

public partial class EditableGridService
{
    /// <summary>
    /// Call service operation for Index form (similar to FormController.CallIndexGetServiceOperation)
    /// </summary>
    private async Task<IndexFormData> CallIndexGetServiceOperation(
        Form form,
        ElasticObject filterValues,
        string? sortFields,
        IdentityUser? user,
        int pageNo,
        int pageSize)
    {
        try
        {
            if (user == null)
            {
                throw new UnauthenticatedUserException();
            }

            filterValues ??= new ElasticObject();
            filterValues["_PageNo"] = pageNo;
            filterValues["_RecordsPerPage"] = pageSize;
            filterValues["_SortField"] = sortFields;

            var serviceOperationFormData = new ServiceOperationFormData(filterValues);
            var auditTrail = new AuditTrail(
                TriggerTypeId.Business,
                $"{form.GetServiceOperation} Form {form.Id} in entity {form.entity.Id}",
                user,
                0)
            {
                MetaEntityId = form.entity.DbId,
                MetaFormId = form.DbId,
                FormId = form.Id
            };

            await _formServiceOperation.CallGetServiceOperation(serviceOperationFormData, form, auditTrail);

            var list = serviceOperationFormData.Response["Rows"] as List<ElasticObject>;
            var result = new IndexFormData(filterValues)
            {
                recordCount = list?.Count ?? 0
            };

            if (list != null)
            {
                result.Rows = list;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling service operation for form {FormId}", form.Id);
            return new IndexFormData(filterValues);
        }
    }
}

