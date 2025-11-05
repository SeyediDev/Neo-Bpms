using Neo.Bpms.Infrastructure.Features.Bpms.Engine;

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
    public async Task<ActionResult> CommandForm(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string __parentNamespaceId, string __parentEntityId, string __parentFormSubjectId, string __subTableAssociationFieldId, string __parentIds,
        string workItemFormId, long? wid, string TaskId, string ProcessId, string Caller,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject record, int callerPage = 1, CancellationToken cancellationToken=default)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.CommandForm, FormSubjectId);
        Form.eFormType formType = form?.FormType ?? Form.eFormType.CommandForm;
        if (formType != Form.eFormType.CommandForm)
        {
            return Error("Invalid Form Type");
        }
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
        ViewBag.structure = structure;
        return View(record);
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
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CommandForm(string NamespaceId, string EntityId, string FormSubjectId, string FormId,
         [ModelBinder(typeof(DynamicActionBinder))] ElasticObject record)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.CommandForm, FormSubjectId) ?? throw new Exception("Invalid form id.");
        if (form?.FormType != Form.eFormType.CommandForm)
        {
            return Error("Invalid Form Type");
        }

        (bool authorized, string errorMessage) = CheckPostAccess(form, out IdentityUser user, out _);
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
        }


        if (form.AfterSaveCommand != null)
        {
            (bool result, string message) = await sendFormCommand.Send(record, form.AfterSaveCommand);
            if (!result)
                return Error(message, "Create");
        }

        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetCreateStructure(culture, NamespaceId, EntityId, FormSubjectId, FormId, form,
            user);
        if (structure == null)
            return Error(Messages.PageNotFound, "Create");
        structure.FormType = Form.eFormType.CommandForm;
        LocalParameters lp = GetLocalParameters(user, null);
        FormComboData.SetCombosData(form, structure, culture, record, lp);
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false, form.Entity);
        ViewBag.structure = structure;
        return View(record);
    }
}
