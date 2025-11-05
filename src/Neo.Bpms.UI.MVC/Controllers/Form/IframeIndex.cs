#pragma warning disable 1573

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    [HttpGet]
    public async Task<ActionResult> IframeIndex(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string FormId, int? Page, long? FilterId, [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject parameters, bool withoutTopBar = true)
    {
        SetInIframe();
        (Form form, IdentityUser user, CommonFormStructure structure, PersistenceObject po) formAndThings = await GetFormAndThings(NamespaceId, EntityId, FormId);
        IdentityUser user = formAndThings.user;
        Form form = formAndThings.form;
        CommonFormStructure structure = formAndThings.structure;
        PersistenceObject po = formAndThings.po;

        if (form.FormType != Form.eFormType.Index)
        {
            throw new HttpException(400, "Invalid form type");
        }

        var configuredFilters = await filterConfigBackupRestore.Configurations("Form", NamespaceId, EntityId, FormId, cancellationToken);
        (ElasticObject filterValues, ConfiguredFilter configuredFilter ) = await filterController.GetConfiguredFilterValues(
            FilterId, configuredFilters, user, po, parameters);
        if (structure == null)
            return Error(Messages.PageNotFound);
        int pageNo = Page ?? (po?.Page ?? 1);

        IndexFormData records = await GetIndexRecords(cancellationToken, form, structure, CultureHelper.GetCurrentNeutralCulture(),
            filterValues, po?.SortFields, pageNo, user);

        SetCommonIndexViewBags(pageNo, po?.SortFields, user, structure, filterValues,
            configuredFilter, GetRecordsPerPage(records.Rows.Count));
        WithoutTopBarViewBag(withoutTopBar);
        return View(records);
    }


    [HttpPost]
    public async Task<ActionResult> IframeIndex(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string FormId, string SortFields, string alreadySelectedsJson,
        [ModelBinder(typeof(DynamicActionBinder))] ElasticObject FilterValues, int Page = 1, bool withoutTopBar = true)
    {
        SetInIframe();
        (Form form, IdentityUser user, CommonFormStructure structure, PersistenceObject po) formAndThings = await GetFormAndThings(NamespaceId, EntityId, FormId, null, null, SortFields);
        IdentityUser user = formAndThings.user;
        Form form = formAndThings.form;
        CommonFormStructure structure = formAndThings.structure;

        if (form.FormType != Form.eFormType.Index)
        {
            throw new HttpException(400, "Invalid form type");
        }

        if (structure == null)
            return Error(Messages.PageNotFound);
        UserStatePersistence.Persist(User.Identity.Name, structure.NamespaceId, structure.EntityId, "Form", "Index",
                form.FormSubjectId,
                new PersistenceObject { Page = Page, SortFields = SortFields, FilterValues = FilterValues });

        IndexFormData records = await GetIndexRecords(cancellationToken, form, structure, CultureHelper.GetCurrentNeutralCulture(),
               FilterValues, SortFields, Page, user);
        records.AlreadySelectedsJson = alreadySelectedsJson;
        SetCommonIndexViewBags(Page, SortFields, user, structure, FilterValues,
            null, GetRecordsPerPage(records.Rows.Count));
        WithoutTopBarViewBag(withoutTopBar);

        return View(records);
    }

    private void WithoutTopBarViewBag(bool withoutTopBar)
    {
        ViewBag.WithoutTopBar = withoutTopBar;
    }
}
