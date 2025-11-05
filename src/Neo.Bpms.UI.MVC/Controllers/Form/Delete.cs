using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    /// <summary>
    /// to delete current record of selected Entity
    /// check if user is Valid and authorized to view delete page of current record, otherwise the user will get redirected to Error page with appropriate message.
    /// check if this record got prevId or NextId to be able to redirect to them as well.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ids"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="__parentNamespaceId"></param>
    /// <param name="__parentEntityId"></param>
    /// <param name="__parentFormSubjectId"></param>
    /// <param name="__parentIds"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <returns></returns>
    public async Task<ActionResult> Delete(string NamespaceId, string EntityId, string ids, string FormSubjectId, string FormId,
        string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId,
        string __subTableAssociationFieldId, string __parentIds,
        long? wid, string TaskId, string ProcessId, [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re, CancellationToken cancellationToken)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Delete, FormSubjectId);
        if (form == null)
        {
            return Error(Messages.FormNotFound, "Delete");
        }

        if (!CheckAccess(form, out IdentityUser user, out _))
        {
            return Error(Messages.PageAccessDenied, "Delete");
        }

        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetDeleteStructure(culture, NamespaceId, EntityId, FormSubjectId, FormId,
                form,
                user);
        if (structure == null)
        {
            return Error(Messages.PageNotFound);
        }

        (ElasticObject record, string ids) r = await GetRecord(ids, form, user, culture, structure, null, cancellationToken);
        ElasticObject record = r.record;
        _ = r.ids;
        if (record == null)
        {
            return Error(Messages.RecordNotFoundOrDifferentState);
            //ViewBag.RecordNotFound = true;
            //record = new ElasticObject();                
        }

        FormComboData.SetCombosData(form, structure, culture, record, GetLocalParameters(user, record));
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false, form.entity);
        //          var prevIds = GetPrevId(form.entity, ids);
        //			var nextIds = GetNextId(form.entity, ids);
        //          ViewBag.HasPrev = !string.IsNullOrEmpty(prevIds);
        //		    ViewBag.HasNext = !string.IsNullOrEmpty(nextIds);
        SetCommonDeleteViewBags(__parentNamespaceId, __parentEntityId, __parentFormSubjectId,
            __subTableAssociationFieldId,
            __parentIds, structure, user);
        SetPagePackId(form);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), record) : View(record);
    }

    /// <summary>
    /// submit to delete a record from Entity
    /// check if user is Valid and authorized to submit delete page to remove current record, otherwise the user will get redirected to Error page with appropriate message.
    /// check if this record got prevId or NextId to be able to redirect to them as well.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// depend on which kind of apply you choose you stay on this page or return to the parent [Edit(in case you are a sub Entity) or Index page] or got to next or prev record;
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="ids"></param>
    /// <param name="DeleteType"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <param name="isApply"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Delete(string NamespaceId, string EntityId, string FormSubjectId, string FormId, string ids,
        string DeleteType, string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId,
        string __subTableAssociationFieldId,
        string __parentIds, long? wid, string TaskId, string ProcessId,
        [ModelBinder(typeof(DynamicActionBinder))]
        ElasticObject re, string isApply = "0", string calendar = "shamsi",
        string returnUrl = null, CancellationToken cancellationToken = default)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Delete, FormSubjectId);
        if (form == null)
        {
            return Error(Messages.FormNotFound, "Delete");
        }

        if (!CheckAccess(form, out IdentityUser user, out long userGroupId))
        {
            return Error(Messages.PageAccessDenied, "Delete");
        }

        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateStructure(culture, form.NamespaceId, form.EntityId,
                form.FormSubjectId, form.Id, form, user);
        (ElasticObject record, string ids) r = await GetRecord(ids, form, user, culture, structure, null, cancellationToken);
        if (form.BeforSaveCommand != null)
        {
            (bool result, string message) = await sendFormCommand.Send(r.record, form.BeforSaveCommand);
            if (!result)
                return Error(message, "Create");
        }
        PostFormData postResult = new(culture, form, r.record) { structure = structure };
        await postForm.PostDeleteForm(postResult, wid, user, ids, userGroupId, cancellationToken);
        if (!postResult.IsValid)
        {
            foreach (ExceptionInfo error in postResult.errors)
            {
                ModelState.AddModelError(error.ForField, error.Exception.Message);
            }
        }
        if (form.AfterSaveCommand != null && postResult.IsValid)
        {
            (bool result, string message) = await sendFormCommand.Send(r.record, form.AfterSaveCommand);
            if (!result)
                return Error(message, "Delete");
        }
        eDeleteType deleteType = (eDeleteType)Enum.Parse(typeof(eDeleteType), DeleteType);
        if (ModelState.IsValid)
        {
            string prevIds = GetPrevId(postResult.form.entity, ids);
            string nextIds = GetNextId(postResult.form.entity, ids);
            //				ViewBag.HasPrev = !string.IsNullOrEmpty(prevIds);
            //				ViewBag.HasNext = !string.IsNullOrEmpty(nextIds);
            if (ModelState.IsValid)
            {
                switch (deleteType)
                {
                    case eDeleteType.DelAndNext:
                        return RedirectToAction("Delete", "Form", new { NamespaceId, EntityId, ids = nextIds, calendar });
                    case eDeleteType.DelAndPrev:
                        return RedirectToAction("Delete", "Form", new { NamespaceId, EntityId, ids = prevIds, calendar });
                    case eDeleteType.DelAndReturn:
                        {
                            return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", new { NamespaceId, EntityId });
                        }

                    default:
                        return !string.IsNullOrEmpty(__parentNamespaceId) && !string.IsNullOrEmpty(__parentEntityId) &&
                            !string.IsNullOrEmpty(__parentIds)
                            ? RedirectToAction("Edit", new
                            {
                                NamespaceId = __parentNamespaceId,
                                EntityId = __parentEntityId,
                                calendar,
                                FormSubjectId = __parentFormSubjectId,
                                SubTableAssociationFeildId = __subTableAssociationFieldId,
                                ids = __parentIds
                            })
                            : (ActionResult)RedirectToAction("Index", new { NamespaceId, EntityId, calendar });
                }
            }
        }
        ElasticObject record = (await GetRecord(ids, postResult.form, user, postResult.Culture, postResult.structure, null, cancellationToken)).record;
        FormComboData.SetCombosData(postResult.form, postResult.structure, postResult.Culture, record, GetLocalParameters(user, record));
        ComboDataRoutines.SetComboDataSelectedId(postResult.structure, record, false, postResult.form.entity);
        SetCommonDeleteViewBags(__parentNamespaceId, __parentEntityId, __parentFormSubjectId, __subTableAssociationFieldId,
            __parentIds, postResult.structure, user);
        SetPagePackId(postResult.form);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), record) : View(record);
    }

    private void SetCommonDeleteViewBags(string __parentNamespaceId, string __parentEntityId,
        string __parentFormSubjectId, string __subTableAssociationFieldId,
        string __parentIds, CommonFormStructure structure, IdentityUser user)
    {
        ViewBag.structure = structure;
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.__parentNamespaceId = __parentNamespaceId;
        ViewBag.__parentEntityId = __parentEntityId;
        ViewBag.__parentFormSubjectId = __parentFormSubjectId;
        ViewBag.__subTableAssociationFieldId = __subTableAssociationFieldId;
        ViewBag.__parentIds = __parentIds;
    }
}
