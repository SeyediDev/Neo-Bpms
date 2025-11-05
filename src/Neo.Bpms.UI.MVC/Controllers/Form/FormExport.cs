namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    public async Task<ActionResult> ExportToExcel(string NamespaceId, string EntityId,
        string FormSubjectId, string FormId, string SortFields, string newPage,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject FilterValues, string calendar, CancellationToken cancellationToken)
    {
        Check.ValidateElastic(ref FilterValues);

        IdentityUser user = GetUser();
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Index, FormSubjectId);
        if (!CheckAccess(user, form))
            return Error(Messages.PageAccessDenied, "Index");
        string culture = CultureHelper.GetCurrentNeutralCulture();
        if (form == null)
            return Error(Messages.PageNotFound, "Index");

        UserStatePersistence.Persist(User.Identity.Name, form.NamespaceId, form.EntityId, "Form", "Index",
            FormSubjectId,
            new PersistenceObject { SortFields = SortFields, FilterValues = FilterValues });

        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(culture, form.NamespaceId, form.EntityId,
                    form.FormSubjectId, form.Id, SortFields, form, user);
        TakeIndexFormQuery takeConfigQuery = new(structure,cancellationToken, form, SortFields, FilterValues, user, culture);
        ReportToExcelGenerator reportViewGenerator = new(cancellationToken,
            takeConfigQuery.Structure, takeConfigQuery.Structure.ColumnInfos, culture, calendar);
        MemoryStream stream = (MemoryStream) await FetchReportViewGeneratorContent(takeConfigQuery, reportViewGenerator, 100000);
        return GetFileStreamResult(EntityId, ".xlsx", stream,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
    private async Task<object> FetchReportViewGeneratorContent(TakeIndexFormQuery takeIndexFormQuery,
        ReportViewGenerator reportViewGenerator, int recordsCount)
    {
        reportViewGenerator.Init();
        reportViewGenerator.GenerateHeader();
        ElasticObject totalRecord = takeIndexFormQuery.FetchTotalRecord();
        if (totalRecord != null)
            reportViewGenerator.GenerateTotalRow(totalRecord);
        await takeIndexFormQuery.TakeQueryPageByPage(formDataRoutines, recordsCount, reportViewGenerator.GenerateRows);
        reportViewGenerator.GenerateFooter();
        object content = reportViewGenerator.Content;
        reportViewGenerator.Release();
        return content;
    }
}
