using Neo.Bpms.Domain.Entities.Security.Authorization;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.UI.MVC.ViewModels.Forms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    public ActionResult BulkCreateProcess(string NamespaceId, string EntityId, string FormSubjectId,
        string bulkFormId,
        string TaskId, string ProcessId, string ProcessVersion, string Caller,
        BulkQueryType? queryType, List<string> recordsIds,
        string indexFormId, string associationFieldId,
        [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject indexFormFilterValues,
        int? indexFormPageNo, int callerPage = 1)
    {
        if (queryType == null)
            return Error("نوع عملیات گروهی نامعتبر است.", "BulkCreateProcess");
        dynamic record = new ElasticObject();
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, bulkFormId,
            Form.eFormType.ProcessCreateOnExistingRecord, FormSubjectId);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.ProcessCreateOnExistingRecord;
        if (form == null)
            return Error(Messages.PageNotFound);
        if (!CheckAccess(form, out IdentityUser user, out _))
            return Error(Messages.PageAccessDenied, "BulkCreateProcess");
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateStructure(culture, NamespaceId, EntityId,
            FormSubjectId, bulkFormId, form, user);
        if (structure == null)
            return Error(Messages.PageNotFound, "BulkCreateProcess");
        CheckPossibility(structure, indexFormFilterValues);
        structure.FormType = formType;
        SetPagePackId(form);
        UiEntity entity = form.entity;
        FormComboData.SetCombosData(form, structure, culture, null, GetLocalParameters(user, null));
        ComboDataRoutines.SetComboDataSelectedId(structure, record, true, entity);
        SetBulkCreateProcessViewBags(TaskId, ProcessId, ProcessVersion, Caller,
            queryType, recordsIds, indexFormId, associationFieldId,
            indexFormFilterValues, indexFormPageNo, callerPage, structure, user);
        return View(record);
    }

    public enum BulkCreateReturn
    {
        Index,
        MyWorkItems
    }

    [HttpPost]
    public async Task<ActionResult> BulkCreateProcess(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string TaskId, string ProcessId, string ProcessVersion,
        string EditType, [ModelBinder(typeof(DynamicActionBinder))]
            ElasticObject formData,
        BulkQueryType? queryType, List<string> recordsIds, string indexFormId, string associationFieldId,
        string indexFormFilterValues, int? indexFormPageNo, 
        BulkCreateReturn returnPage = BulkCreateReturn.Index, CancellationToken cancellationToken=default)
    {
        if (queryType == null)
            return Error("نوع عملیات گروهی نامعتبر است.", "BulkCreateProcess");
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId,
            Form.eFormType.ProcessCreateOnExistingRecord,
            FormSubjectId);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.ProcessCreateOnExistingRecord;
        if (form == null)
            return Error(Messages.PageNotFound);
        if (!CheckAccess(form, out IdentityUser user, out long userGroupId))
            return Error(Messages.PageAccessDenied);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateProcessStructure(culture, NamespaceId, EntityId, FormSubjectId,
            FormId,
            form, user);
        if (structure == null || form == null)
            return Error(Messages.PageNotFound, "BulkCreateProcess");
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ElasticObject indexFilterValues = controllerMethods.DecodeFilterValues(indexFormFilterValues);
        UiEntity indexEntity = form.entity;
        ViewBag.structure = structure;
        structure.FormType = formType;
        if (!ModelState.IsValid)
            goto returnBulk;
        Domain.Entities.Cmmn.Fields.EntityField associationField =
            string.IsNullOrEmpty(associationFieldId) ? null : form.entity.GetField(associationFieldId);
        if (associationField != null)
            indexEntity = associationField.AssociationEntity.Entity() as UiEntity ?? form.entity;
        Form indexForm = (indexEntity?.getForm(indexFormId)) ?? throw new Exception("Index form not found");
        indexFilterValues["q"] = formData;
        switch (queryType)
        {
            case BulkQueryType.List:
                await UpdateRecordsAndCreateProcess(NamespaceId, EntityId, recordsIds,
                    indexForm, indexFilterValues,
                    formData, form, TaskId, ProcessId, ProcessVersion, user, userGroupId, culture, true, associationFieldId);
                break;
            case BulkQueryType.Query:
                CheckPossibility(structure, indexFilterValues);
                await UpdateRecordsAndCreateProcess(NamespaceId, EntityId, null,
                    indexForm, indexFilterValues,
                    formData, form, TaskId, ProcessId, ProcessVersion, user, userGroupId, culture, true, associationFieldId);
                break;
        }

    returnBulk:
        if (!ModelState.IsValid)
        {
            //				return BulkCreateProcess(NamespaceId, EntityId, FormSubjectId, FormId,
            //					TaskId, ProcessId, "", queryType,
            //					recordsIds, indexFormId, indexFilterValues,
            //					indexFormPageNo, false);
            string error = string.Join("\n",
                ModelState.Values.Where(ms => ms.Errors.Any())
                    .Select(err => string.Join("\n", err.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage))));
            return Error(error, "BulkCreateProcess");
        }

        if (returnPage == BulkCreateReturn.MyWorkItems)
        {
            return RedirectToAction("IndexWorkItems", "Process", new WorkItemsFilter
            {
                __ProcessId = ProcessId,
                __ProcessVersionId = ProcessVersion,
                __PageType = WorkItemsPageType.MyWorkItems
            });
        }

        indexFilterValues?.SetField("FormId", indexFormId);
        return RedirectToAction("Index", new
        {
            indexEntity?.NamespaceId,
            EntityId = indexEntity?.Id,
            FormId = indexFormId,
            Page = indexFormPageNo
        });
    }

    private void SetBulkCreateProcessViewBags(string TaskId, string ProcessId, string ProcessVersion, string Caller,
        BulkQueryType? queryType, List<string> recordsIds, string indexFormId, string associationFieldId,
        ElasticObject indexFormFilterValues, int? indexFormPageNo, int callerPage, CommonFormStructure structure,
        IdentityUser user)
    {
        ViewBag.Caller = Caller;
        ViewBag.CallerPage = callerPage;
        ViewBag.TaskId = TaskId;
        ViewBag.ProcessId = ProcessId;
        ViewBag.ProcessVersion = ProcessVersion;
        ViewBag.structure = structure;
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanSeeBpmn = CheckProcessAccess(user, ProcessId); // todo only process admins?
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ViewBag.QueryType = queryType;
        ViewBag.RecordsIds = recordsIds;
        ViewBag.IndexFormId = indexFormId;
        ViewBag.AssociationFieldId = associationFieldId;
        ViewBag.IndexFormFilterValues = controllerMethods.CodeFilterValues(indexFormFilterValues);
        ViewBag.IndexFormPageNo = indexFormPageNo ?? 1;
    }
}
