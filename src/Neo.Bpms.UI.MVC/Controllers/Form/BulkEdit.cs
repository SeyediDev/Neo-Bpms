using Neo.Bpms.UI.MVC.ViewModels.Forms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    public ActionResult BulkEdit(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string workItemFormId, long? pid, long? wid, string TaskId, string ProcessId, string Caller,
        BulkQueryType? queryType, List<string> recordsIds,
        string indexFormId,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject indexFormFilterValues,
        int? indexFormPageNo, int callerPage = 1)
    {
        if (queryType == null)
            return Error("نوع عملیات گروهی نامعتبر است.", "BulkEdit");
        dynamic record = new ElasticObject();
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.BulkEdit, FormSubjectId) ?? throw new HttpException(404, Messages.PageNotFound);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.BulkEdit;
        IdentityUser user = GetUserAndCheckFormAccess(form);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateStructure(culture, NamespaceId, EntityId,
            FormSubjectId, FormId, form, user);
        if (structure == null || form == null)
            return Error(Messages.PageNotFound, "BulkEdit");
        CheckPossibility(structure, indexFormFilterValues);
        structure.FormType = formType;

        SetPagePackId(form);
        UiEntity entity = form.entity;
        FormComboData.SetCombosData(form, structure, culture, null, GetLocalParameters(user, null));
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false, entity);
        SetBulkEditViewBags(workItemFormId, pid, wid, TaskId, ProcessId, Caller,
        queryType, recordsIds, indexFormId, indexFormFilterValues,
        indexFormPageNo, callerPage, structure, user);
        return View(record);
    }

    private void SetBulkEditViewBags(string workItemFormId, long? pid, long? wid, string TaskId, string ProcessId,
        string Caller, BulkQueryType? queryType, List<string> recordsIds, string indexFormId, ElasticObject indexFormFilterValues,
        int? indexFormPageNo, int callerPage, CommonFormStructure structure, IdentityUser user)
    {
        ViewBag.Caller = Caller;
        ViewBag.CallerPage = callerPage;
        ViewBag.pid = pid;
        ViewBag.wid = wid;
        ViewBag.TaskId = TaskId;
        ViewBag.ProcessId = ProcessId;
        ViewBag.workItemFormId = workItemFormId;
        ViewBag.structure = structure;
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ViewBag.QueryType = queryType;
        ViewBag.RecordsIds = recordsIds;
        ViewBag.IndexFormId = indexFormId;
        ViewBag.IndexFormFilterValues = controllerMethods.CodeFilterValues(indexFormFilterValues);
        ViewBag.IndexFormPageNo = indexFormPageNo ?? 1;
    }


    [HttpPost]
    public async Task<ActionResult> BulkEdit(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string EditType, [ModelBinder(typeof(DynamicActionBinder))] ElasticObject record,
        BulkQueryType? queryType, List<string> recordsIds, string indexFormId, string indexFormFilterValues,
        int? indexFormPageNo)
    {
        if (queryType == null)
            return Error("نوع عملیات گروهی نامعتبر است.", "BulkEdit");
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.BulkEdit, FormSubjectId);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.BulkEdit;
        if (form == null)
            return Error(Messages.PageNotFound);
        if (!CheckAccess(form, out IdentityUser user, out long userGroupId))
            return Error(Messages.PageAccessDenied);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetBulkEditStructure(culture, NamespaceId, EntityId, FormSubjectId, FormId,
            form, user);
        if (structure == null || form == null)
            return Error(Messages.PageNotFound, "BulkEdit");
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ElasticObject indexFilterValues = controllerMethods.DecodeFilterValues(indexFormFilterValues);
        ViewBag.structure = structure;
        structure.FormType = formType;
        SetPagePackId(form);
        if (!ModelState.IsValid)
            goto returnBulkEdit;
        switch (queryType)
        {
            case BulkQueryType.List:
                await UpdateRecordsAndCreateProcess(NamespaceId, EntityId, recordsIds,
                    null, null,
                    record, form, null, null, null, user, userGroupId, culture, false);
                break;
            case BulkQueryType.Query:
                CheckPossibility(structure, indexFilterValues);
                await UpdateRecordsAndCreateProcess(NamespaceId, EntityId, null,
                    form.entity.getForm(indexFormId), indexFilterValues,
                    record, form, null, null, null, user, userGroupId, culture, false);
                break;
        }
    returnBulkEdit:
        indexFilterValues?.SetField("FormId", indexFormId);
        if (!ModelState.IsValid)
        {
            Exception error = ModelState.Values.FirstOrDefault(m => m.Errors.Any())?.Errors?[0].Exception ?? new Exception("Unknown error");
            throw error;
        }
        return RedirectToAction("Index", "Form", new
        {
            NamespaceId,
            EntityId,
            FormSubjectId,
            FormId = indexFormId,
            Page = indexFormPageNo ?? 1,
            newPage = indexFormPageNo?.ToString() ?? "1",
            Values = indexFilterValues
        });
    }
}
