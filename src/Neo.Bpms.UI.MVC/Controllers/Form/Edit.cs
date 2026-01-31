using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions.Extensions;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Extensions;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;
namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    /// <summary>
    /// redirect here to Edit a current record or record of sub entity of selected Entity
    /// check if user is Valid to view the Edit page, otherwise the user will get redirected to Error page with appropriate message.
    /// check if this record got prevId or NextId to be able to redirect to them as well.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="ids"></param>
    /// <param name="__parentNamespaceId"></param>
    /// <param name="__parentEntityId"></param>
    /// <param name="__parentFormSubjectId"></param>
    /// <param name="__parentIds"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <param name="bSubTable"></param>
    /// <returns></returns>
    public async Task<ActionResult> Edit(string NamespaceId, string EntityId, string FormSubjectId, string FormId, string ids,
        string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId, string __subTableAssociationFieldId, string __parentIds,
        long? wid, string TaskId, string ProcessId, string Caller,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject re, 
        int callerPage = 1, bool bSubTable = false, CancellationToken cancellationToken = default)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Edit, FormSubjectId);
        if (form == null)
        {
            return Error(Messages.PageNotFound, "Edit");
        }

        if (!CheckAccess(form, out IdentityUser user, out long userGroupId))
        {
            return Error(Messages.PageAccessDenied);
        }

        if (wid != null && string.IsNullOrEmpty(ids))
        {
            ids = PostForm.GetEntityPkvFromActivityInstance(wid);
        }

        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure =
                formStructRoutines.GetEditStructure(culture, NamespaceId, EntityId, FormSubjectId, FormId, form, user);
        if (structure == null)
        {
            return Error(Messages.PageNotFound, "Edit");
        }

        LocalParameters lcs = GetLocalParameters(user, null);
        lcs.Add("NamespaceId", NamespaceId);
        lcs.Add("EntityId", EntityId);
        lcs.Add("FormId", FormId);
        lcs.Add("SubjectId", FormSubjectId);
        if (string.IsNullOrEmpty(ids) && !string.IsNullOrEmpty(form.InputRecordIdFormula))
        {
            ids = formLogicHelper.ServerEval(form.InputRecordIdFormula, lcs)?.ToString();
        }

        ElasticObject record;
        if (!string.IsNullOrEmpty(TaskId) && wid == null)
        {
            record = new ElasticObject();
        }
        else
        {
            (ElasticObject e, string s) = await GetRecord(ids, form, user, culture, structure, null, cancellationToken);
            record = e;
            ids = s;
        }
        if (string.IsNullOrEmpty(ids) && record != null)
        {
            ids = record.GetString("Ids");
        }

        if (record == null)
        {
            if (string.IsNullOrEmpty(ids))
            {
                ViewBag.RecordNotFound = true;
                record = new ElasticObject();
            }
            else
            {
                return Error(Messages.RecordNotFoundOrDifferentState, "Edit");
            }
        }

        if (!string.IsNullOrEmpty(TaskId) && wid != null)
        {
            ActivityInstanceRecordDb ai = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).FetchActivityInstanceRecord(wid.Value);
            if (ai == null || ai.Closed || ai.userTaskStateId >= UserTaskInstanceStateId.Completed)
            {
                ModelState.AddModelError("", Messages.WorkAlreadyDone);
            }

            if (ai != null)
            {
                record[RenderingForm.WorkDescription] = ai.Description;
                ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).SetWorkItemStartTime(TaskId, wid.Value, form, user, userGroupId);
            }
        }

        lcs.AddOrUpdate("q", record);
        FormComboData.SetCombosData(form, structure, culture, record, lcs);
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false);
        if (!string.IsNullOrEmpty(__parentIds) &&
            !string.IsNullOrEmpty(__parentNamespaceId) &&
            !string.IsNullOrEmpty(__parentEntityId))
        {
            LocalParameters lp = GetLocalParameters(user, null);
            Entity parentEntity = ProjectDefinition.Project.GetEntity(__parentNamespaceId, __parentEntityId);
            ComboData newComboData = ComboDataRoutines.GetRecords(form.entity, parentEntity, culture, FormDataRoutines.GetPKFilter(__parentIds, parentEntity),
                    1, "", "", lp);
            ViewBag.ParentDisplay = newComboData.Rows.Count > 0 ? newComboData.Rows[0].DisplayValue : "";
        }

        SetCommonEditViewBags(ids, __parentNamespaceId, __parentEntityId, __parentFormSubjectId, __subTableAssociationFieldId,
            __parentIds, wid, TaskId, ProcessId, "", Caller, callerPage, bSubTable,
            structure, user, form.entity);
        // todo set versionId??
        SetPagePackId(form);
        return !string.IsNullOrEmpty(form.SpecificViewPage)? View(form.GetSpecificViewPage(), record) :View(record);
    }

    /// <summary>
    /// submit changes to current record of selected Entity
    /// check if user is Valid and authorized to make changes to the record, otherwise the user will get redirected to Error page with appropriate message.
    /// check if this record got prevId or NextId to be able to redirect to them as well.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// depend on which kind of apply you choose you stay on this page or return to the parent [Edit(in case you are a sub Entity) or Index page] or redirect to Details page of current record.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="EditType"></param>
    /// <param name="record"></param>
    /// <param name="ids"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <param name="isApply"></param>
    /// <param name="bSubTable"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Edit(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string EditType,
        [ModelBinder(typeof(DynamicActionBinder))]
            ElasticObject record, string ids,
        string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId, string __subTableAssociationFieldId,
        string __parentIds,
        long? wid, string TaskId,
        string ProcessId, string Caller,
        int callerPage = 1, string isApply = "0", string bSubTable = "0", string calendar = "shamsi",
        string returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (EntityId == "Config")
        {
            foreach (KeyValuePair<string, ElasticObject> item in record.Attributes)
            {
                if (item.Key == "Value")
                    if (item.Value.InternalValue.ToString().Contains("\""))
                        item.Value.InternalValue = System.Text.Json.JsonSerializer.Serialize(item.Value.InternalValue.ToString()[1..^1]);
                    else
                        item.Value.InternalValue = System.Text.Json.JsonSerializer.Serialize(item.Value.InternalValue);
            }
        }
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Edit, FormSubjectId) 
            ?? throw new Exception("Invalid form id.");
        ElasticObject clonedRecord = record.Clone();
        PostFormData postFormData = new(CultureHelper.GetCurrentNeutralCulture(), form, clonedRecord)
        {
            EntityPkv = ids,
            WorkItemId = wid
        };
        (bool authorized, string errorMessage) = CheckPostAccess(form, out IdentityUser user, out long userGroupId);
        if (!authorized)
        {
            ModelState.AddModelError("", errorMessage);
        }
        else
        {
            if (form.BeforSaveCommand != null)
            {
                (bool result, string message) = await sendFormCommand.Send(record, form.BeforSaveCommand);
                if (!result)
                    return Error(message, "Create");
            }


            await postForm.PostEditForm(postFormData, ids, wid, TaskId, ProcessId, isApply == "1", user, userGroupId, cancellationToken);
            wid = postFormData.WorkItemId;
            ViewBag.Ids = postFormData.EntityPkv;
            if (postFormData.Errors != null)
            {
                foreach (ExceptionInfo error in postFormData.Errors)
                {
                    ModelState.AddModelError(error.ForField ?? "", error.Exception.Message);
                }
            }
        }
        if (form.AfterSaveCommand != null && postFormData.IsValid)
        {
            (bool result, string message) = await sendFormCommand.Send(record, form.AfterSaveCommand);
            if (!result)
                return Error(message, "Edit");
        }

        EditTypeId eType = (EditTypeId)Enum.Parse(typeof(EditTypeId), EditType);
        if (postFormData.IsValid)
        {
            return postFormData.RedirectToWorkItems
                ? RedirectToWorkItems(Caller, ProcessId, callerPage)
                : RedirectFromEditForm(NamespaceId, EntityId, FormSubjectId, FormId, record, ids, wid,
                    TaskId, ProcessId, Caller, bSubTable, calendar, returnUrl, eType, postFormData);
        }

        if (!string.IsNullOrEmpty(ids) && ModelState.IsValid)
        {
            (ElasticObject e, string s) = await GetRecord(ids, postFormData.Form, user, postFormData.Culture, 
                postFormData.Structure, null, cancellationToken);
            ids = s;
            record = e;
        }
        else
        {
            if (ModelState.IsValid)
            {
                //Logger.LogError("null or empty ids but model is valid!");
                ModelState.AddModelError(string.Empty, Messages.SaveWasNotSuccessful);
            }

            (ElasticObject record, string ids) r = await MergeSavedAndPosted(ids, clonedRecord, postFormData.Form, postFormData.Structure, postFormData.Culture, user, cancellationToken);
            record = r.record;
            ids = r.ids;
        }

        //FormComboData.SetCombosData(postFormData.Form, postFormData.Structure, postFormData.Culture, record,
        //    GetLocalParameters(user, record));
        //ComboDataRoutines.SetComboDataSelectedId(postFormData.Structure, record, false);
        SetCommonEditViewBags(ids, __parentNamespaceId, __parentEntityId, __parentFormSubjectId, __subTableAssociationFieldId,
            __parentIds, wid, TaskId, ProcessId, "", Caller, callerPage,
            bSubTable == "1", postFormData.Structure, user, postFormData.Form.entity);
        SetPagePackId(postFormData.Form);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), record) : View(record);
    }

    private async Task<(ElasticObject record, string ids)> MergeSavedAndPosted(string ids, ElasticObject clonedRecord,
        Form form, CommonFormStructure structure, string culture,
        IdentityUser user, CancellationToken cancellationToken = default)
    {
        ElasticObject record;
        applyFormData.ConvertPostedElasticToRecord(ref clonedRecord, structure);
        if (!string.IsNullOrEmpty(ids))
        {
            (ElasticObject record, string ids) freshRecord = await GetRecord(ids, form, user, culture, structure,
                    clonedRecord, cancellationToken);
            ids = freshRecord.ids;
            freshRecord.record.MergeValues(clonedRecord);
            record = freshRecord.record;
        }
        else
        {
            record = clonedRecord;
        }

        return (record, ids);
    }

    private ActionResult RedirectFromEditForm(string namespaceId, string entityId, string formSubjectId,
        string formId, ElasticObject record, string ids, long? wid, string taskId, string processId, string caller,
        string bSubTable, string calendar, string returnUrl, EditTypeId eType,
        PostFormData postFormData)
    {
        string prevIds = GetPrevId(postFormData.Form.entity, ids);
        string nextIds = GetNextId(postFormData.Form.entity, ids);
        switch (eType)
        {
            case EditTypeId.Save:
                return RedirectToEdit(formSubjectId, formId, wid, taskId, processId, caller, bSubTable, calendar,
                     postFormData.Structure, ids);
            case EditTypeId.SaveAndNext:
                return RedirectToEdit(formSubjectId, formId, wid, taskId, processId, caller, bSubTable, calendar,
                    postFormData.Structure, nextIds);
            case EditTypeId.SaveAndPrev:
                return RedirectToEdit(formSubjectId, formId, wid, taskId, processId, caller, bSubTable, calendar,
                    postFormData.Structure, prevIds);
            case EditTypeId.SaveAndDetails:
                return RedirectToDetail(formSubjectId, ids, wid, caller, calendar, postFormData.Structure);
            case EditTypeId.SaveAndReturn:
                {
                    return !string.IsNullOrEmpty(returnUrl)
                        ? Redirect(returnUrl)
                        : RedirectToAction("Index",
                            new
                            {
                                NamespaceId = namespaceId,
                                EntityId = entityId,
                                Caller = caller,
                                calendar
                            });
                }

            default:
                postFormData.FetchParentParams(record);
                if (!string.IsNullOrEmpty(postFormData.ParentNamespaceId) &&
                    !string.IsNullOrEmpty(postFormData.ParentEntityId) &&
                    !string.IsNullOrEmpty(postFormData.ParentIds))
                {
                    return RedirectToParent(wid, taskId, processId, caller, bSubTable, calendar, postFormData);
                }

                return RedirectToAction("Index",
                    new { NamespaceId = namespaceId, EntityId = entityId, Caller = caller, calendar });
        }
    }

    private void AddModelError(ExceptionInfos errors)
    {
        if (errors == null || errors.Count == 0)
        {
            ModelState.AddModelError("", Messages.SaveWasNotSuccessful);
        }
        else
        {
            foreach (ExceptionInfo error in errors)
            {
                ModelState.AddModelError(error.ForField, error.Exception.Message);
            }
        }
    }

    private ActionResult RedirectToParent(long? wid, string TaskId, string ProcessId, string Caller,
        string bSubTable, string calendar, PostFormData postFormData)
    {
        return RedirectToAction("Edit", new
        {
            NamespaceId = postFormData.ParentNamespaceId,
            EntityId = postFormData.ParentEntityId,
            FormSubjectId = postFormData.ParentFormSubjectId,
            SubTableAssociationFeildId = postFormData.SubTableAssociationFeildId,
            ids = postFormData.ParentIds,
            wid,
            TaskId,
            calendar,
            ProcessId,
            Caller,
            bSubTable = bSubTable != "0"
        });
    }

    private ActionResult RedirectToDetail(string FormSubjectId, string ids, long? wid, string Caller,
        string calendar, CommonFormStructure structure)
    {
        return RedirectToAction("Details", "Form", new
        {
            structure.NamespaceId,
            structure.EntityId,
            FormSubjectId,
            ids,
            wid,
            calendar,
            Caller
        });
    }

    private ActionResult RedirectToEdit(string FormSubjectId, string FormId, long? wid, string TaskId,
        string ProcessId, string Caller, string bSubTable, string calendar,
        CommonFormStructure structure, string nextIds)
    {
        return RedirectToAction("Edit", "Form", new
        {
            structure.NamespaceId,
            structure.EntityId,
            FormSubjectId,
            FormId,
            ids = nextIds,
            wid,
            TaskId,
            calendar,
            ProcessId,
            Caller,
            bSubTable = bSubTable != "0"
        });
    }

    private void SetCommonEditViewBags(string ids, string parentNamespaceId,
        string parentEntityId, string parentFormSubjectId, string subTableAssociationFeildId, string parentIds, long? wid, string taskId,
        string processId, string versionId, string caller, int callerPage,
        bool bSubTable, CommonFormStructure structure, IdentityUser user, UiEntity entity)
    {
        ViewBag.Caller = caller;
        ViewBag.CallerPage = callerPage;
        ViewBag.wid = wid;
        ViewBag.TaskId = taskId;
        ViewBag.VersionId = versionId;
        ViewBag.ProcessId = processId;
        ViewBag.structure = structure;
        ViewBag.Ids = ids;
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanSeeBpmn = user?.CheckProcessAccess(processId) ?? false; // todo only process admins?
        ViewBag.HasProcess = entity.HasProcess();
        ViewBag.CanPublishConfigs = user?.CheckSystemFeatureAccess(SystemFeatureId.PublishConfigs) ?? false;
        ViewBag.__parentNamespaceId = parentNamespaceId;
        ViewBag.__parentEntityId = parentEntityId;
        ViewBag.__parentFormSubjectId = parentFormSubjectId;
        ViewBag.__subTableAssociationFieldId = subTableAssociationFeildId;
        ViewBag.__parentIds = parentIds;
        ViewBag.bSubTable = bSubTable;
    }
}
