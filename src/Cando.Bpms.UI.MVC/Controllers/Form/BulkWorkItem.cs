using Neo.Bpms.Domain.Entities.Security.Authorization;
using Neo.Bpms.UI.MVC.ViewModels.Forms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    public async Task<ActionResult> BulkWorkItem(WorkItemsFilter filter, string formId,
        BulkQueryType? queryType, List<string> aiList, string indexFormId, string caller,
        [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject indexFormFilterValues)
    {
        if (queryType == null)
            return Error("نوع عملیات گروهی نامعتبر است.", "BulkWorkItem");
        Domain.Model.BPMN.Processes.Process process = (ProjectDefinition.Project.GetBpmnDefinition(filter.ProcessId,
                filter.ProcessVersionId)
            ?.Process) ?? throw new Exception("فرآیند شناخته نشد!");
        Form form = FormStructRoutines.GetForm(process.EntityNamespaceId, process.EntityId,
            formId, Form.eFormType.WorkItem, null);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.WorkItem;
        IdentityUser user = GetUserAndCheckFormAccess(form);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateStructure(culture, process.EntityNamespaceId, process.EntityId,
            null, formId, form, user); // todo why create? todo is it necessarily from same namespace?
        if (structure == null || form == null)
            return Error(Messages.PageNotFound, "BulkEdit");
        structure.FormType = formType;
        SetBulkWorkItemViewBags(structure, user, filter?.ProcessId);
        SetPagePackId(form);
        FormComboData.SetCombosData(form, structure, culture, null, GetLocalParameters(user, null));
        BulkWorkItemFormViewModel model = new()
        {
            Record = new ElasticObject(), //todo indexFormFilterValues ?
            FormId = formId,
            AiList = aiList,
            Filter = filter,
            QueryType = queryType.Value,
            IndexFormFilterValues = controllerMethods.CodeFilterValues(indexFormFilterValues),
            Caller = caller
        };
        if (!string.IsNullOrEmpty(indexFormId))
        {
            _ = FormStructRoutines.GetForm(process.EntityNamespaceId, process.EntityId,
                indexFormId, Form.eFormType.Index, null);
            model.IndexStructure = await formStructRoutines.GetIndexStructure(culture, process.EntityNamespaceId,
                process.EntityId, form.FormSubjectId, indexFormId, null, form, user);
        }

        return View(model);
    }

    private void SetBulkWorkItemViewBags(CommonFormStructure structure, IdentityUser user, string filterProcessId)
    {
        ViewBag.structure = structure;
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanSeeBpmn = CheckProcessAccess(user, filterProcessId); // todo only process admins?
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
    }
}
