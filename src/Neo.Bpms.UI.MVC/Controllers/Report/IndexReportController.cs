using System.Diagnostics.CodeAnalysis;
using Neo.Bpms.Domain.Features.Security;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ScheduledReport;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ReportController
{
    private const int ReportPerPageCount = 20;
    private const int ReportTimeout = 600000; // 10 minutes

    /// <summary>
    /// When you Redirect to Report/Index?[parameters] you call this Action, Type [HttpGet],
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get Persistent Data related to the Index to initiate the requested Index Form.
    /// get report Configs.
    /// set a session value to set the right Item in Navigation Menu.
    /// check if the request is for a drill down window or whole report page.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="Page"></param>
    /// <param name="newPage"></param>
    /// <param name="FilterId"></param>
    /// <param name="ParentReportIds"></param>
    /// <param name="ParentFilterValues"></param>
    /// <param name="DrillDown"></param>
    /// <param name="re"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // [AsyncTimeout(ReportTimeout)]
    public async Task<ActionResult> Index(CancellationToken cancellationToken,
        string NamespaceId, string EntityId, string ReportId,
        string ConfigId, int? Page, string newPage,
        long? FilterId, string ParentReportIds, string ParentFilterValues, int? DrillDown,
        int? calendarYear, int? calendarMonth, string calendarType,
        [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        IdentityUser user = GetUser();
        ReportId ??= Request.Query["PageId"];
        ConfiguredReport config = await CheckReportAccess(NamespaceId, EntityId, ReportId, ConfigId, user);
        Report report = config.Report;
        PersistenceObject po = GetReportPersistence(report);
        var configuredFilters = await filterConfigBackupRestore.Configurations("Report", report.NamespaceId, report.EntityId, report.Id, cancellationToken);
        (ElasticObject filters, ConfiguredFilter configuredFilter) = await filterController.GetConfiguredFilterValues(
            FilterId, configuredFilters, user, po, re );
        if (!string.IsNullOrEmpty(ParentReportIds) && !string.IsNullOrEmpty(ParentFilterValues))
            filters = controllerMethods.DecodeFilterValues(ParentFilterValues);
        string culture = CultureHelper.GetCurrentNeutralCulture() ?? "fa";
        int pageNo = Page ?? (po?.Page ?? 1);
        if (!string.IsNullOrEmpty(newPage) && newPage == "1")
            pageNo = 1;
        List<object> listIds = ParentReportIds != null ? [.. ParentReportIds.Split(',')] : null;
        ReportData result = await reportDataRoutines.GetReportData(config, true, filters,
            pageNo, po?.SortFields, null, null, listIds,
            culture, false, user, ReportPerPageCount, false, cancellationToken);
        
        // Ensure ChartType is set from config if ViewType is Chart
        // This is important when navigating from dashboard where ChartType is not posted
        if (config.ViewType == ReportViewType.Chart)
        {
            // Always use config.ChartType - it should be set in GetReportStructure, but ensure it here too
            // If config.ChartType is None or Line (default), use Column as fallback
            // Line is the default value in ConfiguredReport, so we treat it as unset
            if (config.ChartType == ChartType.None || config.ChartType == ChartType.Line)
            {
                result.Structure.ChartType = ChartType.Column;
            }
            else
            {
                result.Structure.ChartType = config.ChartType;
            }
        }
        
        await SetReportViewBag(user, ParentReportIds, po?.SortFields,
            filters, DrillDown == 1, pageNo,
            po == null, result, configuredFilter, calendarYear, calendarMonth, calendarType);
        ViewBag.ParentFilterValues = ParentFilterValues; //todo
        if (config.Parent == null)
            SetPagePackId(report);
        result.Structure.ParentReportIds = ParentReportIds;
        if (config.Parent == null)
        {
            if (result.Structure.ChartType == ChartType.WorldMap)
            {
                ViewBag.ContainerClass = "container-fluid";
            }

            return View("Index", result);
        }
        SetInIframe();
        return PartialView(config.ViewType == ReportViewType.Chart
            ? (config.ChartType == ChartType.IranMap
                ? "~/Views/Report/Map/_IranMap.cshtml"
                : config.ChartType == ChartType.WorldMap
                ? "~/Views/Report/Map/_WorldMap.cshtml"
                : "~/Views/Report/Charts/_chartView.cshtml")
            : "~/Views/Report/ListView/_listView.cshtml", result);
    }

    /// <summary>
    /// any POST Action on Report/Index Page will lead the user to this action. (such as submitting new filter or changing page and etc).
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get Persistent Data related to the Index to initiate the requested Index Form.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// set a session value to set the right Item in Navigation Menu.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="Page"></param>
    /// <param name="newPage"></param>
    /// <param name="ParentReportIds"></param>
    /// <param name="sortFields"></param>
    /// <param name="SelectedChartType"></param>
    /// <param name="FilterValues"></param>
    /// <param name="DrillDown"></param>
    /// <param name="calendar"></param>
    /// <returns></returns>
    [HttpPost]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // [AsyncTimeout(ReportTimeout)]
    public async Task<ActionResult> Index(CancellationToken cancellationToken,
        string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string newPage, string ParentReportIds, string sortFields,
        string SelectedChartType, [ModelBinder(typeof(DynamicActionBinder))]
        ElasticObject FilterValues,
        int DrillDown = 0, int Page = 1, string calendar = "shamsi",
        int? calendarYear = null, int? calendarMonth = null, string calendarType = null)
    {
        IdentityUser user = GetUser();
        InitPostReportConfigResult initPostReportConfigResult = new() { Page = Page };
        await InitPostReportConfig(user, NamespaceId, EntityId, ReportId, ConfigId, newPage, sortFields,
            FilterValues, initPostReportConfigResult);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        List<object> arr = ParentReportIds != null ? [.. ParentReportIds.Split(',')] : null;
        ReportData result = await reportDataRoutines.GetReportData(initPostReportConfigResult.config, true, FilterValues, Page,
            sortFields, null, null, arr,
            culture, false, user, ReportPerPageCount, false, cancellationToken);
        // Use posted ChartType if provided, otherwise use config's ChartType
        if (!string.IsNullOrEmpty(SelectedChartType))
        {
            result.Structure.ChartType =
                (ChartType)Enum.Parse(typeof(ChartType), SelectedChartType);
        }
        else if (initPostReportConfigResult.config.ViewType == ReportViewType.Chart)
        {
            // Use config's ChartType if not posted
            result.Structure.ChartType = initPostReportConfigResult.config.ChartType;
        }

        await SetReportViewBag(user, ParentReportIds, sortFields, FilterValues, DrillDown == 1, Page, false, result, null,
            calendarYear, calendarMonth, calendarType ?? calendar);
        return DrillDown == 1
            ? PartialView(initPostReportConfigResult.config.ViewType == ReportViewType.Chart
                ? (initPostReportConfigResult.config.ChartType == ChartType.IranMap
                    ? "~/Views/Report/Map/_IranMap.cshtml"
                    : initPostReportConfigResult.config.ChartType == ChartType.WorldMap
                    ? "~/Views/Report/Map/_WorldMap.cshtml"
                    : "~/Views/Report/Charts/_chartView.cshtml")
                : "~/Views/Report/ListView/_listView.cshtml", result)
            : View("Index", result);
    }

    public async Task<ActionResult> LoadScheduled(CancellationToken cancellationToken,
        string configId, long scheduleId)
    {
        IdentityUser user = GetUser();
        (_, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool Result) =
            await scheduledReportLoader.FetchScheduledReport(configId, scheduleId, user);
        if (!Result)
        {
            throw new Exception(errorString);
        }
        ElasticObject filterValues = new();
        foreach (ConfiguredFilterValue configuredFilterValue in configuredScheduledReport?.Values ?? [])
        {
            filterValues.SetField(configuredFilterValue.FieldId, configuredFilterValue.Value);
        }

        ViewBag.user = user;
        ViewBag.ScheduledId = configuredScheduledReport?.Id;
        ViewBag.ScheduledName = configuredScheduledReport?.Name;
        return await Index(cancellationToken, "", "", "", configId, "", "", "", "", filterValues, 0, 1,
            ProjectDefinition.Project.DefaultCalendar);
    }

    public async Task<ActionResult> Sql(CancellationToken cancellationToken,
        string NamespaceId, string EntityId, string ReportId, string ConfigId)
    {
        IdentityUser user = GetUser();
        if (!user.IsAdmin)
            throw new Exception();
        ConfiguredReport config = await CheckReportAccess(NamespaceId, EntityId, ReportId, ConfigId, user);
        Report report = config.Report;
        PersistenceObject po = GetReportPersistence(report);

        var configuredFilters = await filterConfigBackupRestore.Configurations("Report", report.NamespaceId, report.EntityId, report.Id, cancellationToken);
        (ElasticObject filters, _) = await filterController.GetConfiguredFilterValues(
                null, configuredFilters, user, po, new ElasticObject());
        string culture = CultureHelper.GetCurrentNeutralCulture() ?? "fa";
        int pageNo = 1;
        ReportData result = await reportDataRoutines.GetReportData(config, true, filters,
            pageNo, po?.SortFields, null, null, null,
            culture, false, user, ReportPerPageCount, true, cancellationToken);
        StringBuilder sb = new();
        foreach (string query in result.QueryInfo.GetQueries())
        {
            sb.AppendLine(query);
            sb.AppendLine();
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/sql", $"{ReportId}.sql");
    }


    // [AsyncTimeout(ReportTimeout)]
    public async Task<ActionResult> IframeReport(CancellationToken cancellationToken,
        string namespaceId, string entityId, string reportId,
        string configId, int? page, long? filterId, string parentReportIds, string parentFilterValues,
        int? drillDown,
        [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        SetInIframe();
        IdentityUser user = GetUser();
        ConfiguredReport config = await CheckReportAccess(namespaceId, entityId, reportId, configId, user);
        Report report = config.Report;
        PersistenceObject po = GetReportPersistence(report);

        var configuredFilters = await filterConfigBackupRestore.Configurations("Report", report.NamespaceId, report.EntityId, report.Id, cancellationToken);
        (ElasticObject filters, ConfiguredFilter configuredFilter) = await filterController.GetConfiguredFilterValues(
                filterId, configuredFilters, user, po, new ElasticObject());

        if (!string.IsNullOrEmpty(parentReportIds) && !string.IsNullOrEmpty(parentFilterValues))
            filters = controllerMethods.DecodeFilterValues(parentFilterValues);
        string culture = CultureHelper.GetCurrentNeutralCulture() ?? "fa";
        int pageNo = page ?? (po?.Page ?? 1);
        List<object> listIds = parentReportIds != null ? [.. parentReportIds.Split(',')] : null;
        const bool loadData = true;
        ReportData result = await reportDataRoutines.GetReportData(config, loadData, filters,
            pageNo, po?.SortFields, null, null, listIds,
            culture, false, user, ReportPerPageCount, false, cancellationToken);
        await SetReportViewBag(user, parentReportIds, po?.SortFields,
            filters, drillDown == 1, pageNo,
            po == null, result, configuredFilter);
        ViewBag.ParentFilterValues = parentFilterValues; //todo
        result.Structure.ParentReportIds = parentReportIds;

        return View(result);
    }

    private async Task SetReportViewBag(IdentityUser user, string parentReportIds, string sortFields,
        ElasticObject filterValues, bool isDrillDown, int page, bool persistentIsNull, ReportData result,
        ConfiguredFilter configuredFilter, int? calendarYear = null, int? calendarMonth = null, string calendarType = null)
    {
        foreach (ErrorInformation error in result.Errors)
        {
            ModelState.AddModelError(error.For, error.Text);
        }

        IIdentityUserService identityUserService = HttpContext.RequestServices.GetRequiredService<IIdentityUserService>();
        IClubRolesService clubRolesService = HttpContext.RequestServices.GetRequiredService<IClubRolesService>();
        ViewBag.user = user;
        ViewBag.UserGroups = await identityUserService.GetIdentityRoles();
        ViewBag.ClubRoles = clubRolesService.GetClubRoles();
        ViewBag.CanDesign = CheckAccess(user, SystemFeatureId.ReportDesign);
        ViewBag.CanDesignFilter = CanDesignForms(user); // todo OK?
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ViewBag.CanScheduleReports = CheckAccess(user, SystemFeatureId.ScheduledReportDesign);
        ViewBag.recordsPerPage = result.RecordsPerPage;
        ViewBag.Page = page;
        ViewBag.SortFields = sortFields;
        ViewBag.FilterValues = filterValues;
        ViewBag.ParentReportIds = parentReportIds;
        ViewBag.DrillDown = isDrillDown;
        ViewBag.PersistentIsNull = persistentIsNull;
        ViewBag.FilterId = configuredFilter?.Id;
        ViewBag.FilterName = configuredFilter?.Name;
        ViewBag.calendarYear = calendarYear;
        ViewBag.calendarMonth = calendarMonth;
        ViewBag.calendarType = calendarType;
        if (!isDrillDown && result.Structure.ChartType == ChartType.WorldMap)
        {
            ViewBag.ContainerClass = "container-fluid";
        }
    }

    private async Task<ConfiguredReport> CheckReportAccess(string namespaceId,
        string entityId, string reportId, string configId, IdentityUser user)
    {
        GetReportConfigResult getReportConfigResult = new(configId);
        if (!await reportConfigManager.GetReportConfig(namespaceId, entityId, reportId,
            getReportConfigResult, user))
            throw new Exception(Messages.ConfigNotFound);
        ConfiguredReport config = getReportConfigResult.Config;
        return !user.CheckReportAccess(config.Report.entity.NamespaceId,
            config.Report.entity.Id, config.Report.Id, config.Report)
            ? throw new UnauthorizedAccessException(Messages.ReportAccessDenied)
            : config;
    }

    private PersistenceObject GetReportPersistence(Report report)
    {
        return UserStatePersistence.GetPersistence(User.Identity.Name, report?.NamespaceId,
            report?.EntityId, "Report", "Index", report?.Id);
    }

    private static void FillFilterValuesFromPersistence(PersistenceObject ppo, ElasticObject filterValues)
    {
        foreach (KeyValuePair<string, ElasticObject> fv in ppo?.FilterValues?.Attributes ?? [])
        {
            string v = fv.Value?.ToString();
            string ofv = filterValues.GetString(fv.Key);
            if (string.IsNullOrEmpty(ofv) && !string.IsNullOrEmpty(v))
                filterValues.SetField(fv.Key, v);
        }
    }
}
