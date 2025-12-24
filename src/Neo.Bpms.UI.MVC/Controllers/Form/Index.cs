using Neo.Bpms.Application.Features;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;
using Neo.Bpms.UI.MVC.Features;
using Microsoft.Extensions.Options;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController(
    FormServiceOperation formServiceOperation,
    FormStructRoutines formStructRoutines,
    ControllerMethods controllerMethods,
    FilterManager filterController,
    IFormLogicHelper formLogicHelper,
    FormLayout formLayout,
    FormDataRoutines formDataRoutines,
    FilterConfigBackupRestore filterConfigBackupRestore,
    ServiceOperationManager serviceOperationManager,
    ISendFormCommand sendFormCommand, IPostForm postForm, IApplyFormData applyFormData,
    IOptions<CmmnSettings> cmmnSettings
    ) : BpmsController
{
    private int _recordsPerPage = 15;

    /// <summary>
    /// When you Redirect to Form/Index?[parameters] you call this Action, Type [HttpGet],
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// get Persistent Data related to the Index to initiate the requested Index Form.
    /// set a session value to set the right Item in Navigation Menu.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="Page"></param>
    /// <param name="newPage"></param>
    /// <param name="Values"></param>
    /// <param name="FilterId"></param>
    /// <param name="calendar"></param>
    /// <param name="designMode"></param>
    /// <returns></returns>
    public async Task<ActionResult> Index(CancellationToken cancellationToken,
        string NamespaceId, string EntityId, string FormSubjectId, string FormId, int? Page,
        string newPage, [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject Values,
        long? FilterId, string calendar = "shamsi", bool? designMode = false)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Index, FormSubjectId);
        if (form == null)
            return Error(Messages.FormNotFound, "Index");
        if (!CheckAccess(form, out IdentityUser user, out _))
        {
            if (!CheckAccess(user, SystemFeatureId.FormDesign))
                return Error(Messages.PageAccessDenied, "Index");
            designMode = true; // todo 
        }

        string culture = CultureHelper.GetCurrentNeutralCulture();
        PersistenceObject po = GetIndexPersistence(form);
        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(culture, NamespaceId, EntityId, form.FormSubjectId,
            FormId, po?.SortFields, form, user);
        var configuredFilters = await filterConfigBackupRestore.Configurations("Form", NamespaceId, EntityId, FormId, cancellationToken);
        (ElasticObject filterValues, ConfiguredFilter configuredFilter) = await filterController.GetConfiguredFilterValues(
            FilterId, configuredFilters, user, po, Values);
        if (structure == null)
            return Error(Messages.PageNotFound);
        int pageNo = Page ?? (po?.Page ?? 1);
        if (!string.IsNullOrEmpty(newPage) && newPage == "1")
        {
            pageNo = 1;
        }

        string sortFields = po?.SortFields;
        //structure.FilterFields = GetIndexFilterValues(Filters);
        //			if (ProjectDefinition.Project.DefaultCalendar == "shamsi")
        //			{
        //				if (filterValues != null && calendar == "shamsi")
        //					FormDataRoutines.SetRecordDateToMiladi(filterValues, form.entity);
        //			}
        IndexFormData records = designMode ?? false
            ? new IndexFormData(filterValues)
            : await GetIndexRecords(cancellationToken, form, structure, culture, filterValues, sortFields,
                pageNo,
                user);
        SetCommonIndexViewBags(pageNo, sortFields, user, structure, filterValues,
            configuredFilter, GetRecordsPerPage(records.Rows.Count));
        SetPagePackId(form);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), records) : View(records);
    }

    /// <summary>
    /// any POST Action on Form/Index Page will lead the user to this action. (such as submitting new filter or changing page and etc).
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// get Persistent Data related to the Index to initiate the requested Index Form.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="FormSubjectId"></param>
    /// <param name="FormId"></param>
    /// <param name="SortFields"></param>
    /// <param name="Page"></param>
    /// <param name="newPage"></param>
    /// <param name="alreadySelectedsJson"></param>
    /// <param name="FilterValues"></param>
    /// <param name="calendar"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Index(CancellationToken cancellationToken,
        string NamespaceId, string EntityId, string FormSubjectId, string FormId,
        string SortFields, string newPage, string alreadySelectedsJson,
        [ModelBinder(typeof(DynamicActionBinder))] ElasticObject FilterValues, string calendar = "shamsi", int Page = 1)
    {
        Check.ValidateElastic(ref FilterValues);
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Index, FormSubjectId);
        if (form == null)
            return Error(Messages.PageNotFound, "Index");
        if (!CheckAccess(form, out IdentityUser user, out _))
            return Error(Messages.PageAccessDenied, "Index");
        string culture = CultureHelper.GetCurrentNeutralCulture();
        //			if (FilterValues != null && form != null && calendar == "shamsi")
        //				FormDataRoutines.SetRecordDateToMiladi(FilterValues, form.entity);
        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(culture, NamespaceId, EntityId, form.FormSubjectId,
            FormId, SortFields, form, user);
        if (structure == null)
            return Error(Messages.PageNotFound, "Index");
        if (!string.IsNullOrEmpty(newPage) && newPage == "1")
        {
            Page = 1;
        }

        if (!form.AllowAnonymous)
            UserStatePersistence.Persist(User.Identity.Name, structure.NamespaceId, structure.EntityId, "Form", "Index",
                form.FormSubjectId,
                new PersistenceObject { Page = Page, SortFields = SortFields, FilterValues = FilterValues });
        IndexFormData records = await GetIndexRecords(cancellationToken, form, structure, culture, FilterValues, SortFields,
            Page,
            user);
        records.AlreadySelectedsJson = alreadySelectedsJson;
        SetCommonIndexViewBags(Page, SortFields, user, structure, FilterValues,
            null, GetRecordsPerPage(records.Rows.Count));
        SetPagePackId(form);
        return !string.IsNullOrEmpty(form.SpecificViewPage) ? View(form.GetSpecificViewPage(), records) : View("Index", records);
    }

    private async Task<IndexFormData> GetIndexRecords(CancellationToken cancellationToken, Form form,
        CommonFormStructure structure, string culture, ElasticObject filterValues, string sortFields, int pageNo,
        IdentityUser user)
    {
        IndexFormData records;
        try
        {
            string validSortFields = form.SortType == FormSortType.PassToProvider ? sortFields : null;
            records = string.IsNullOrEmpty(form.GetServiceOperation)
                ? await formDataRoutines.GetRecordsWithoutJoin(
                    structure, form.entity, form, culture, filterValues,
                    validSortFields, pageNo, _recordsPerPage, null,
                    GetLocalParameters(user, null),
                    null, user, true, true, cancellationToken)
                : await CallIndexGetServiceOperation(form, filterValues, validSortFields, user, pageNo,
                    _recordsPerPage);

            if (form.SortType == FormSortType.InMemory)
            {
                records.Rows = SortRecordsInMemory(records.Rows, sortFields);
            }

            FormComboData.SetCombosData(form, structure, culture, null, GetLocalParameters(user, filterValues));
            if (records.FilterValues != null)
                ComboDataRoutines.SetComboDataSelectedId(structure, records.FilterValues,
                    true);
        }
        catch (Exception e)
        {
            records = new IndexFormData(filterValues);
            ModelState.AddModelError("", e.Message);
        }

        return records;
    }

    private List<ElasticObject> SortRecordsInMemory(List<ElasticObject> records, string sortFieldsString)
    {
        if (string.IsNullOrEmpty(sortFieldsString))
            return records;
        string[] sortFieldsList = sortFieldsString.Split('#');
        IOrderedEnumerable<ElasticObject> result = null;
        for (int i = 0; i < sortFieldsList.Length; i++)
        {
            string[] sortField = sortFieldsList[i].Split(' ');
            if (sortField.Length != 2)
                continue;
            string f = sortField[0];
            string type = sortField[1];
            if (type == "ASC" && i == 0)
                result = records.OrderBy(r => r[f]);
            else if (type == "DESC" && i == 0)
                result = records.OrderByDescending(r => r[f]);
            else if (type == "ASC" && i != 0)
                result = result?.ThenBy(r => r[f]);
            else if (type == "DESC" && i != 0)
                result = result?.ThenByDescending(r => r[f]);
        }
        return result?.ToList();
    }

    private PersistenceObject GetIndexPersistence(Form form)
    {
        return UserStatePersistence.GetPersistence(User.Identity.Name, form.NamespaceId,
            form.EntityId, "Form", "Index", form.FormSubjectId);
    }

    private void SetCommonIndexViewBags(int pageNo, string sortFields, IdentityUser user,
        CommonFormStructure structure, ElasticObject filterValues,
        ConfiguredFilter configuredFilter, int recordsPerPage)
    {
        ViewBag.Page = pageNo;
        ViewBag.SortFields = sortFields;
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanPublishConfigs = user?.CheckSystemFeatureAccess(SystemFeatureId.PublishConfigs) ?? false;
        ViewBag.structure = structure;
        ViewBag.recordsPerPage = recordsPerPage;
        ViewBag.FilterValues = filterValues;
        ViewBag.FilterId = configuredFilter?.Id;
        ViewBag.FilterName = configuredFilter?.Name;
    }

    private int GetRecordsPerPage(int rowsCount)
    {
        return rowsCount > _recordsPerPage ? rowsCount : _recordsPerPage;
    }
}
