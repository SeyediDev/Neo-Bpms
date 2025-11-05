using Neo.Bpms.Domain.Entities.Base.Audit;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    [HttpGet]
    public ActionResult CustomPage(string NamespaceId, string EntityId, string FormId)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId,
                EntityId, FormId, Form.eFormType.CustomPage) ?? throw new HttpException(404, Messages.PageNotFound);
        if (!CheckAccess(form, out IdentityUser user, out _))
        {
            throw new HttpException(403, Messages.PageAccessDenied);
        }

        CommonFormStructure structure = formStructRoutines.GetCommonFormStructure(CultureHelper.GetCurrentNeutralCulture(),
                form.NamespaceId,
                form.EntityId, form.Id, form.FormType, form.FormSubjectId, form, user) ?? throw new Exception("FormStructure not found!");
        ElasticObject record = new();
        FormComboData.SetCombosData(form, structure,
            CultureHelper.GetCurrentNeutralCulture(),
            record, GetLocalParameters(user, record));
        //			ComboDataRoutines.SetComboDataSelectedId(structure, record, false, form.entity);
        SetCustomPageViewBags(user);
        SetPagePackId(form);
        return View(structure);
    }

    [HttpPost]
    public async Task<JsonResult> RunOperationControl(string NamespaceId,
        string EntityId, string FormId, string ControlId, [ModelBinder(typeof(DynamicActionBinder))] ElasticObject Qs)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId,
                EntityId, FormId,
                Form.eFormType.CustomPage /*just because it is needed!*/) ?? throw new HttpException(404, Messages.PageNotFound);
        if (!CheckAccess(form, out IdentityUser user, out long userGroupId))
        {
            throw new HttpException(403, Messages.PageAccessDenied);
        }

        FormField formField = (form.formFields?.FirstOrDefault(f => f.Id == ControlId)) ?? throw new Exception($"formField not found {FormId} {ControlId}");
        if (!Enum.TryParse(
                formField.GetProperty(eControlPropertyId.ServiceInterfaceProtocol)?.value?.ToString() ?? "InternalLibrary",
                true, out ServiceInterfaceProtocol serviceInterfaceProtocol))
        {
            throw new Exception($"Invalid ServiceInterfaceProtocol {FormId} {ControlId}");
        }

        string operationName = formField.GetProperty(eControlPropertyId.OperationName)?.value?.ToString();
        LocalParameters inputData = Qs.ToLocalParameters();
        AuditTrail auditTrail = new(TriggerTypeId.EngineBusiness,
                $"Run Operation {NamespaceId}.{EntityId}.{FormId}.{ControlId}.{operationName}", user, userGroupId);
        (LocalParameters outParams, System.Net.HttpStatusCode statusCode) = await serviceOperationManager.CallServiceOperation(auditTrail,
            serviceInterfaceProtocol, operationName, inputData);
        return Json(new
        {
            Message = statusCode.ToString(),
            OutParams = outParams
        });
    }

    private void SetCustomPageViewBags(IdentityUser user)
    {
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
    }
}
