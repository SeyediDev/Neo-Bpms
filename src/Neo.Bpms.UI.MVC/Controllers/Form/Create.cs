using Neo.Bpms.Infrastructure.Features.Bpms.Engine;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    /// <summary>
    /// to insert new record into the related Entity.
    /// check if user is Valid and authorized to get redirected to create page to insert new record to Entity, otherwise the user will get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="__parentNamespaceId"></param>
    /// <param name="__parentEntityId"></param>
    /// <param name="__parentFormSubjectId"></param>
    /// <param name="__subTableAssociationFieldId"></param>
    /// <param name="__parentIds"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <returns></returns>
    public async Task<ActionResult> Create(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId, string __subTableAssociationFieldId, string __parentIds,
        string workItemFormId, long? wid, string TaskId, string ProcessId, string Caller,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject record, int callerPage = 1, CancellationToken cancellationToken=default)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Create, FormSubjectId);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.Create;
        if (form == null)
            return Error(Messages.PageNotFound);
        if (!CheckAccess(form, out IdentityUser user, out long userGroupId))
            return Error(Messages.PageAccessDenied);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateStructure(culture, NamespaceId, EntityId, FormSubjectId, FormId, form,
            user);
        if (structure == null)
            return Error(Messages.PageNotFound, "Create");
        structure.FormType = formType;
        SetCommonCreateViewBags(__parentNamespaceId, __parentEntityId, __parentFormSubjectId, __subTableAssociationFieldId,
            __parentIds, workItemFormId, wid, TaskId, ProcessId, Caller, callerPage, user, structure);
        LocalParameters lp = GetLocalParameters(user, null);
        if (!string.IsNullOrEmpty(__parentIds))
        {
            Entity parentEntity = ProjectDefinition.Project.GetEntity(__parentNamespaceId, __parentEntityId);
            ComboData newComboData = ComboDataRoutines.GetRecords(form.entity, parentEntity, true, culture,
                 FormDataRoutines.GetPKFilter(__parentIds, parentEntity), 1, "", "", lp);
            ViewBag.ParentDisplay = newComboData.Rows.Count > 0 ? newComboData.Rows[0].DisplayValue : "";
        }
        SetPagePackId(form);
        UiEntity entity = form.entity;
        if (wid != null)
        {
            string ids = PostForm.GetEntityPkvFromActivityInstance(wid);
            (ElasticObject record, string ids) r = await GetRecord(ids, form, user, culture, structure, null, cancellationToken);
            record = r.record;
            if (record == null)
            {
                return Error(Messages.RecordNotFound, "ProcessCreate");
            }
            ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).SetWorkItemStartTime(TaskId, wid.Value, form, user, userGroupId);
        }
        FormComboData.SetCombosData(form, structure, culture, record, lp);
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false, entity);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), record) : View(record);
    }

    /// <summary>
    /// to submit new record into the related Entity.
    /// check if user is Valid and authorized to submit new record into Entity, otherwise the user will get redirected to Error page with appropriate message.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// depend on kind of apply you can do "Add and New" or "Add and Return to List" operation.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="record"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <param name="isApply"></param>
    /// <param name="addAgain"></param>
    /// <returns></returns>
    [HttpPost]
    //[ValidateAntiForgeryToken(Salt = "Neo.Web.Form.Create")]
    public async Task<ActionResult> Create(string NamespaceId, string EntityId, string FormSubjectId, string FormId, string CreateType,
         [ModelBinder(typeof(DynamicActionBinder))] ElasticObject record, string workItemFormId, long? wid,
         string TaskId, string ProcessId, string Caller, int callerPage = 1,
         string isApply = "0", bool addAgain = false, string calendar = "shamsi", string returnUrl = null)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Create, FormSubjectId) ?? throw new Exception("Invalid form id.");
        eCreateType createType = CreateType.ToEnum(eCreateType.SaveAndReturn);
        if (createType == eCreateType.SaveAndReturn) addAgain = false;

        PostFormData postFormData = new(CultureHelper.GetCurrentNeutralCulture(), form, record);
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
            await postForm.PostCreateForm(postFormData, wid, TaskId, ProcessId, user, createType, userGroupId);
            if (postFormData.errors != null)
            {
                foreach (ExceptionInfo error in postFormData.errors)
                {
                    ModelState.AddModelError(error.ForField ?? "", error.Exception.Message);
                }
            }
        }


        postFormData.FetchParentParams(record);
        if (form.AfterSaveCommand != null && postFormData.IsValid)
        {
            (bool result, string message) = await sendFormCommand.Send(record, form.AfterSaveCommand);
            if (!result)
                return Error(message, "Create");
        }
        if (postFormData.IsValid)
        {
            if (postFormData.RedirectToWorkItems)
                return RedirectToWorkItems(Caller, ProcessId, callerPage);
            if (postFormData.structure.FormType != Form.eFormType.ProcessCreate)
            {
                if (addAgain)
                {
                    SetCommonCreateViewBags(postFormData.parentNamespaceId, postFormData.parentEntityId,
                        postFormData.parentFormSubjectId, postFormData.subTableAssociationFeildId, postFormData.parentIds, workItemFormId, wid, TaskId,
                        ProcessId, Caller, callerPage, user, postFormData.structure);
                    ComboDataRoutines.SetComboDataSelectedId(postFormData.structure, record, false, postFormData.form.entity);
                    FormComboData.SetCombosData(postFormData.form, postFormData.structure, postFormData.Culture, null, GetLocalParameters(user, record));
                    return View(new ElasticObject());
                }

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return !string.IsNullOrEmpty(postFormData.parentNamespaceId) && !string.IsNullOrEmpty(postFormData.parentEntityId) &&
                     !string.IsNullOrEmpty(postFormData.parentIds)
                    ? RedirectToAction("Edit", "Form", new
                    {
                        NamespaceId = postFormData.parentNamespaceId,
                        EntityId = postFormData.parentEntityId,
                        FormSubjectId = postFormData.parentFormSubjectId,
                        SubTableAssociationFeildId = postFormData.subTableAssociationFeildId,
                        ids = postFormData.parentIds,
                        FormId = workItemFormId,
                        TaskId,
                        calendar,
                        ProcessId,
                        wid,
                        Caller
                    })
                    : (ActionResult)RedirectToAction("Index", new { NamespaceId, EntityId, calendar });
            }
        }
        SetCommonCreateViewBags(postFormData.parentNamespaceId, postFormData.parentEntityId, postFormData.parentFormSubjectId,
            postFormData.subTableAssociationFeildId, postFormData.parentIds, workItemFormId, wid, TaskId,
            ProcessId, Caller, callerPage, user, postFormData.structure);
        SetPagePackId(postFormData.form);
        ViewBag.structure = postFormData.structure;
        FormComboData.SetCombosData(postFormData.form, postFormData.structure, postFormData.Culture, null, GetLocalParameters(user, record));
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), record) : View(record);
    }

    private void SetCommonCreateViewBags(string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId,
         string __subTableAssociationFieldId, string __parentIds, string workItemFormId, long? wid, string TaskId, string ProcessId, string Caller,
         int callerPage, IdentityUser user, CommonFormStructure structure)
    {
        ViewBag.Caller = Caller;
        ViewBag.CallerPage = callerPage;
        ViewBag.wid = wid;
        ViewBag.TaskId = TaskId;
        ViewBag.ProcessId = ProcessId;
        ViewBag.workItemFormId = workItemFormId;
        ViewBag.structure = structure;
        ViewBag.__parentNamespaceId = __parentNamespaceId;
        ViewBag.__parentEntityId = __parentEntityId;
        ViewBag.__parentFormSubjectId = __parentFormSubjectId;
        ViewBag.__subTableAssociationFieldId = __subTableAssociationFieldId;
        ViewBag.__parentIds = __parentIds;
        var canDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanDesign = canDesign;
        ViewBag.CanPublishConfigs = user?.CheckSystemFeatureAccess(SystemFeatureId.PublishConfigs) ?? false;
    }
}
