namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    /// <summary>
    /// getting redirect to view a record Details you call this Action
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// check if this record got prevId or NextId to be able to redirect to them as well.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="ids"></param>
    /// <param name="wid"></param>
    /// <param name="TaskId"></param>
    /// <param name="ProcessId"></param>
    /// <param name="isApply"></param>
    /// <returns></returns>
    public async Task<ActionResult> Details(string NamespaceId, string EntityId, string FormSubjectId, string FormId, string ids,
        long? wid, string TaskId, string ProcessId,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject re, string isApply = "0", CancellationToken cancellationToken = default)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Detail, FormSubjectId);
        if (form == null)
            return Error(Messages.FormNotFound, "Detail");
        if (!CheckAccess(form, out IdentityUser user, out _))
            return Error(Messages.PageAccessDenied, "Details");
        string culture = CultureHelper.GetCurrentNeutralCulture();
        CommonFormStructure structure = formStructRoutines.GetDetailsStructure(culture, NamespaceId, EntityId, FormSubjectId, FormId,
            form, user);
        if (structure == null)
        {
            return Error(Messages.PageNotFound, "Details");
        }
        (ElasticObject record, string ids) r = await GetRecord(ids, form, user, culture, structure, null, cancellationToken);
        ElasticObject record = r.record;
        ids = r.ids;
        if (record == null)
        {
            //ViewBag.RecordNotFound = true;
            //record = new ElasticObject();
            return Error(Messages.RecordNotFoundOrDifferentState, "Details");
        }
        FormComboData.SetCombosData(form, structure, culture, record, GetLocalParameters(user, record));
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false);

        //		    ViewBag.prevIds = GetPrevId(form.entity, ids);
        //		    ViewBag.nextIds = GetNextId(form.entity, ids);
        //		    ViewBag.HasPrev = !string.IsNullOrEmpty(ViewBag.prevIds);
        //		    ViewBag.HasNext = !string.IsNullOrEmpty(ViewBag.nextIds);
        SetDetailsViewBags(ids, user, structure, form);
        SetPagePackId(form);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), record) : View(record);
    }

    private void SetDetailsViewBags(string ids, IdentityUser user, CommonFormStructure structure, Form form)
    {
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanPublishConfigs = user?.CheckSystemFeatureAccess(SystemFeatureId.PublishConfigs) ?? false;
        ViewBag.structure = structure;
        ViewBag.Ids = ids;
    }
}
