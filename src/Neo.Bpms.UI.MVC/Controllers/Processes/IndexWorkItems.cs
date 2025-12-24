using Neo.Bpms.Domain.Model.BPMN.Processes;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using static Neo.Bpms.Domain.Model.UI.Forms.Form;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ProcessController
{
    [HttpGet]
    public async Task<ActionResult> IndexWorkItems(WorkItemsFilter processFilter, CancellationToken cancellationToken)
    {
        IdentityUser user = CheckWorkItemsAccess(processFilter.PageType, processFilter.ProcessId);
        processFilter.SetDefaultValues();
        Process process = (ProjectDefinition.Project.GetBpmnDefinition(processFilter.ProcessId,
             processFilter.ProcessVersionId)?.Process) ?? throw new Exception("چنین فرآیندی یافت نشد.");
        Form form = FindAppropriateForm(process);
        if (form == null || !CheckAccess(user, form))
        {
            return RedirectToAction(processFilter.PageType.ToString(), processFilter);
        }

        PersistenceObject po = UserStatePersistence.GetPersistence(User.Identity.Name,
             process.EntityNamespaceId, process.EntityId, "Form", "Index", form.FormSubjectId);
        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(CultureHelper.GetCurrentNeutralCulture(),
        process.EntityNamespaceId, process.EntityId, form.FormSubjectId, form.Id, po?.SortFields, form, user);
        var configuredFilters = await filterConfigBackupRestore.Configurations("Form", form.NamespaceId, form.EntityId, form.Id, cancellationToken);
        (ElasticObject filterValues, _) = await filterController.GetConfiguredFilterValues(null, configuredFilters, user, po, new ElasticObject());
        if (structure == null)
            return Error(Messages.PageNotFound);

        if (processFilter.PageType == WorkItemsPageType.MyWorkItems)
            processFilter.UserId = user.Id;

        string culture = CultureHelper.GetCurrentNeutralCulture();

        EntityFilterInfo entityFilter = new(structure, form, filterValues, po?.SortFields);
        List<WorkItemViewModel> workItemsList = workItemManager.GetWorkItems(out long recordCount, RecordsPerPage,
             (WorkItemsQueryType)processFilter.PageType, user, culture, processFilter, entityFilter);
        WorkItemsViewModel result = new(workItemsList, processFilter, filterValues, recordCount);
        FormComboData.SetCombosData(form, structure, CultureHelper.GetCurrentNeutralCulture(), null, GetLocalParameters(user, null));
        if (result.EntityFilter != null)
            ComboDataRoutines.SetComboDataSelectedId(structure, result.EntityFilter, true);

        SetViewBags(user, po?.SortFields, structure, processFilter, process);

        return View(result);
    }

    [HttpPost]
    public async Task<ActionResult> IndexWorkItems(WorkItemsFilter processFilter, string indexFormId,
         string SortFields, string newPage, string alreadySelectedsJson,
         [ModelBinder(typeof(DynamicActionBinder))] ElasticObject filterValues, int Page = 1)
    {
        IdentityUser user = CheckWorkItemsAccess(processFilter.PageType, processFilter.ProcessId);

        Process process = (ProjectDefinition.Project.GetBpmnDefinition(processFilter.ProcessId,
             processFilter.ProcessVersionId)?.Process) ?? throw new Exception("چنین فرآیندی یافت نشد.");
        Form form = ((ProjectDefinition.Project.GetEntity(process.EntityNamespaceId,
             process.EntityId) as UiEntity)?.getForms()?.FirstOrDefault(f => f.Id == indexFormId)) ?? throw new Exception("چنین فرمی یافت نشد.");
        if (!CheckAccess(user, form))
            throw new Exception("شما به این فرم دسترسی ندارید.");
        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(CultureHelper.GetCurrentNeutralCulture(),
             process.EntityNamespaceId, process.EntityId, form.FormSubjectId, form.Id, SortFields, form, user)
            ?? throw new Exception(Messages.PageNotFound);
        if (processFilter.PageType == WorkItemsPageType.MyWorkItems)
            processFilter.UserId = user.Id;
        if (!string.IsNullOrEmpty(newPage) && newPage == "1") // todo what?
        {
            Page = 1;
        }
        string culture = CultureHelper.GetCurrentNeutralCulture();

        UserStatePersistence.Persist(User.Identity.Name, structure.NamespaceId, structure.EntityId, "Form", "Index",
                 form.FormSubjectId,
                 new PersistenceObject { Page = Page, SortFields = SortFields, FilterValues = filterValues });
        EntityFilterInfo entityFilter = new(structure, form, filterValues, SortFields);

        DoFilterConversions(processFilter);
        List<WorkItemViewModel> workItemsList = workItemManager.GetWorkItems(out long recordCount, RecordsPerPage,
             (WorkItemsQueryType)processFilter.PageType, user, culture, processFilter, entityFilter);
        WorkItemsViewModel result =
                 new(workItemsList, processFilter, filterValues, recordCount)
                 {
                     AlreadySelectedsJson = alreadySelectedsJson
                 };
        FormComboData.SetCombosData(form, structure, CultureHelper.GetCurrentNeutralCulture(), null, GetLocalParameters(user, null));
        if (result.EntityFilter != null)
            ComboDataRoutines.SetComboDataSelectedId(structure, result.EntityFilter, true);

        SetViewBags(user, SortFields, structure, processFilter, process);

        return View(result);
    }

    private static Form FindAppropriateForm(Process process)
    {
        List<Form> indexForms = (ProjectDefinition.Project.GetEntity(process.EntityNamespaceId,
             process.EntityId) as UiEntity)?.getForms()?.Where(f => f.FormType == eFormType.Index).ToList();
        Form result =
            indexForms?.FirstOrDefault(ef => ef.Id == process.Id) ??
            indexForms?.FirstOrDefault(ef => ef.FormSubjectId == process.Id) ??
            indexForms?.FirstOrDefault(ef => ef.FormSubjectId == $"{process.Id}Cartable") ??
            indexForms?.FirstOrDefault(ef => ef.FormSubjectId == "Cartable");
        return result;
    }
    private void SetViewBags(IdentityUser user, string sortFields, CommonFormStructure structure,
         WorkItemsFilter processFilter, Process process)
    {
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.SortFields = sortFields;
        ViewBag.structure = structure;
        ViewBag.recordsPerPage = RecordsPerPage;
        ViewBag.Page = processFilter.Page;

        ViewBag.ProcessName = process.Name;
        if (processFilter.PageType == WorkItemsPageType.MyWorkItems)
            FillProcessList(process);
        SetActivitiesListViewBag(process);
    }
}
