namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ReportController
{
    private static int _excelMaxRecordsCount;

    private static int ExcelMaxRecordsCount
    {
        get
        {
            if (_excelMaxRecordsCount == 0)
            {
                _excelMaxRecordsCount = int.TryParse(DependencyInjectionHolder.Instance.Configuration["ReportExcelExportMaxRecords"], out int max) ? max : 1000000;
            }

            return _excelMaxRecordsCount;
        }
    }

    // [AsyncTimeout(3000000)]
    public async Task<ActionResult> ExportToExcel(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string ReportId, string ConfigId,
        string newPage, string ParentReportIds, string sortFields,
        [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject FilterValues, int Page = 1, string calendar = "shamsi")
    {
        IdentityUser user = GetUser();
        InitPostReportConfigResult initPostReportConfigResult = new() { Page = Page };
        await InitPostReportConfig(user, NamespaceId, EntityId, ReportId, ConfigId, newPage, 
            sortFields, FilterValues, initPostReportConfigResult);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        (ReportStructure structure, _) = await reportStructRoutines.GetReportStructure(
            initPostReportConfigResult.config, null, culture, user);
        TakeConfigQuery takeConfigQuery = new(cancellationToken, initPostReportConfigResult.config, 
            FilterValues, user, culture, structure, reportDataRoutines);
        ReportToExcelGenerator reportViewGenerator = new(cancellationToken,
            takeConfigQuery.Structure, [.. takeConfigQuery.Structure.Columns], culture, calendar);
        MemoryStream stream =
            (MemoryStream) await FetchReportViewGeneratorContent(takeConfigQuery, reportViewGenerator, ExcelMaxRecordsCount);
        return GetFileStreamResult(EntityId, ".xlsx", stream,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    // [AsyncTimeout(3000000)]
    public async Task<ActionResult> ExportToHtml(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string ReportId, string ConfigId,
        string newPage, string ParentReportIds, string sortFields,
        [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject FilterValues,
        int Page = 1, string calendar = "shamsi")
    {
        IdentityUser user = GetUser();
        InitPostReportConfigResult initPostReportConfigResult = new() { Page = Page };
        await InitPostReportConfig(user, NamespaceId, EntityId, ReportId, ConfigId, newPage, sortFields, FilterValues, initPostReportConfigResult);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        (ReportStructure structure, _) = await reportStructRoutines.GetReportStructure(
            initPostReportConfigResult.config, null, culture, user);
        TakeConfigQuery takeConfigQuery = new(cancellationToken, initPostReportConfigResult.config, 
            FilterValues, user, culture, structure, reportDataRoutines);
        ReportToHtmlGenerator reportViewGenerator = new(cancellationToken,
            takeConfigQuery.Structure, [.. takeConfigQuery.Structure.Columns], culture, calendar);
        object content = await FetchReportViewGeneratorContent(takeConfigQuery, reportViewGenerator, ExcelMaxRecordsCount);
        return CreateTextFile(EntityId, ".html", content?.ToString());
    }

    // [AsyncTimeout(3000000)]
    public async Task<ActionResult> ExportToCSV(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string ReportId, string ConfigId,
        string newPage, string ParentReportIds, string sortFields,
        [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject FilterValues, int Page = 1, string calendar = "shamsi")
    {
        IdentityUser user = GetUser();
        InitPostReportConfigResult initPostReportConfigResult = new() { Page = Page };
        await InitPostReportConfig(user, NamespaceId, EntityId, ReportId, ConfigId, newPage, sortFields, FilterValues, initPostReportConfigResult);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        (ReportStructure structure, _) = await reportStructRoutines.GetReportStructure(
            initPostReportConfigResult.config, null, culture, user);
        TakeConfigQuery takeConfigQuery = new(cancellationToken, initPostReportConfigResult.config, 
            FilterValues, user, culture, structure, reportDataRoutines);
        ReportToCSVGenerator reportViewGenerator = new(cancellationToken,
            takeConfigQuery.Structure, [.. takeConfigQuery.Structure.Columns], culture, calendar);
        object content = await FetchReportViewGeneratorContent(takeConfigQuery, reportViewGenerator, ExcelMaxRecordsCount);
        return CreateTextFile(EntityId, ".csv", content?.ToString());
    }

    private static async Task<object> FetchReportViewGeneratorContent(TakeConfigQuery takeConfigQuery,
        ReportViewGenerator reportViewGenerator, int recordsCount)
    {
        reportViewGenerator.Init();
        reportViewGenerator.GenerateHeader();
        ElasticObject totalRecord = takeConfigQuery.FetchTotalRecord();
        if (totalRecord != null)
            reportViewGenerator.GenerateTotalRow(totalRecord);
        await takeConfigQuery.TakeQueryPageByPage(recordsCount, reportViewGenerator.GenerateRows);
        reportViewGenerator.GenerateFooter();
        object content = reportViewGenerator.Content;
        reportViewGenerator.Release();
        return content;
    }

    private ActionResult CreateTextFile(string fileName, string extension, string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        return File(bytes, "text/plain; charset=utf-32", fileName + extension);
    }
}
