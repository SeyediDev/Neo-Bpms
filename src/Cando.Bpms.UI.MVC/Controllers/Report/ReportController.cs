using Neo.Bpms.Domain.Entities.Cmmn;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Security.Authorization;
using Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ReportController(
    DashboardConfigBackupRestore dashboardConfigBackupRestore, 
    ReportConfigBackupRestore reportConfigBackupRestore,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FilterManager filterController,
    ControllerMethods controllerMethods,
    ReportDataRoutines reportDataRoutines,
    ReportStructRoutines reportStructRoutines,
    ReportConfigManager reportConfigManager,
    ScheduledReportLoader scheduledReportLoader
    ) : BpmsController
{
    public class InitPostReportConfigResult
    {
        public ConfiguredReport config {  get; set; }
        public int Page {  get; set; }
    }
    private async Task<IdentityUser> InitPostReportConfig(IdentityUser user,
        string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string newPage, string sortFields, ElasticObject FilterValues, InitPostReportConfigResult initPostReportConfigResult)
    {
        initPostReportConfigResult.config = await CheckReportAccess(NamespaceId, EntityId, ReportId, ConfigId, user);
        Report report = initPostReportConfigResult.config.Report;
        if (initPostReportConfigResult.config.Parent == null)
            SetPagePackId(report);
        if (initPostReportConfigResult.config.Parent?.ParentConfiguredReport != null)
        {
            //todo goto parent recursively
            PersistenceObject ppo = GetReportPersistence(initPostReportConfigResult.config.Parent?.ParentConfiguredReport.Report);
            FillFilterValuesFromPersistence(ppo, FilterValues);
            ppo = GetReportPersistence(report);
            FillFilterValuesFromPersistence(ppo, FilterValues);
        }

        UserStatePersistence.Persist(User.Identity.Name, report.NamespaceId, report.EntityId,
            "Report", "Index", report.Id,
            new PersistenceObject { Page = initPostReportConfigResult.Page, SortFields = sortFields, FilterValues = FilterValues });
        if (!string.IsNullOrEmpty(newPage) && newPage == "1")
            initPostReportConfigResult.Page = 1;
        return user;
    }

    private void SetPagePackId(Report report)
    {
        SetPagePackId(report.PagePackId);
    }

    /// <summary>
    /// Edit filter of aa column
    /// check if you are valid to visit this page otherwise you get redirected to Error page with a appropriate message.
    /// get current filters list and show them to user.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="ColumnName"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> filterEditor(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string ColumnName, [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            return Error(Messages.ReportAccessDenied, "filterEditor", false);
        ViewBag.user = user;
        if (!CheckAccess(user, SystemFeatureId.ReportDesign))
            return Error(Messages.ReportDesignAccessDenied, "filterEditor", false);
        GetReportConfigResult getReportConfigResult = new(ConfigId);
        if (!await reportConfigManager.GetReportConfig(NamespaceId, EntityId, ReportId, getReportConfigResult, user))
            return Error(Messages.InvalidRequest, "filterEditor", false);
        ConfiguredReport config = getReportConfigResult.Config;
        if (!user.CheckReportAccess(config.Report.NamespaceId, config.Report.EntityId, config.Report.Id,
            config.Report))
            return Error(Messages.ReportAccessDenied, "filterEditor", false);
        ConfiguredReport.SelectedField field = config.GetSelectedFieldByName(ColumnName);
        if (field == null)
            return Error(Messages.InvalidRequest, "filterEditor", false);
        ReportColumnFilterSettings filterSettings =
            ReportStructRoutines.GetColumnFilterSettings(NamespaceId, EntityId, ReportId, ConfigId, field);
        filterSettings.ColumnName = ColumnName;
        return View(filterSettings);
    }

    /// <summary>
    /// submit new and Edited filters to the column of current report.
    /// check if you are authorize to make changes to filters list of current report, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="FilterSettings"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> filterEditor(ReportColumnFilterSettings FilterSettings)
    {
        try
        {
            IdentityUser user = GetUser(User);
            if (user == null || !User.Identity.IsAuthenticated)
                return Error(Messages.ReportAccessDenied, "filterEditor", false);
            ViewBag.user = user;
            if (!CheckAccess(user, SystemFeatureId.ReportDesign))
                return Error(Messages.ReportDesignAccessDenied, "filterEditor", false);
            if (string.IsNullOrEmpty(FilterSettings.ColumnName))
                return Error("درخواست نامعتبر.", "filterEditor", false);
            string configId = FilterSettings.ConfigId;
            GetReportConfigResult getReportConfigResult=new(configId);
            if (!await reportConfigManager.GetReportConfig(FilterSettings.NamespaceId, FilterSettings.EntityId,
                FilterSettings.ReportId, getReportConfigResult, user))
                return Error(Messages.InvalidRequest, "filterEditor", false);
            ConfiguredReport config = getReportConfigResult.Config;
            if (!user.CheckReportAccess(config.Report.NamespaceId, config.Report.EntityId, config.Report.Id,
                config.Report))
                return Error(Messages.ReportAccessDenied, "filterEditor", false);
            ConfiguredReport.SelectedField field = config.GetSelectedFieldByName(FilterSettings.ColumnName);
            if (field == null)
                return Error(Messages.InvalidRequest, "filterEditor", false);
            ReportStructRoutines.SetColumnFilterSetting(FilterSettings, field);
            await reportConfigBackupRestore.Save(config);
            ViewBag.message = "ثبت فیلتر جدید با موفقیت انجام شد.";
        }
        catch (Exception)
        {
            ViewBag.message = "ثبت فیلتر جدید برای ستون با خطا مواجه شد.";
        }

        return View(FilterSettings);
    }

    /// <summary>
    /// show the list of formats for a column or row of current report
    /// check if you are valid to visit this page.
    /// get column or row format list to show user.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="ColumnName"></param>
    /// <param name="IsColumn"></param>
    /// <returns></returns>
    public async Task<ActionResult> formatEditor(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string ColumnName, bool IsColumn,
        [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            return Error(Messages.ReportAccessDenied, "formatEditor", false);
        ViewBag.user = user;
        if (!user.CheckReportAccess(NamespaceId, EntityId, ReportId))
            return Error(Messages.ReportAccessDenied, "formatEditor", false);
        if (!CheckAccess(user, SystemFeatureId.ReportDesign))
            return Error(Messages.ReportDesignAccessDenied, "formatEditor", false);
        GetReportConfigResult getReportConfigResult = new(ConfigId);
        if (!await reportConfigManager.GetReportConfig(NamespaceId, EntityId, ReportId, getReportConfigResult, user))
            return Error(Messages.InvalidRequest, "formatEditor", false);
        ConfiguredReport config = getReportConfigResult.Config;
        if (string.IsNullOrWhiteSpace(ColumnName))
        {
            ColumnName = null;
        }
        else
        {
            ConfiguredReport.SelectedField field = config.GetSelectedFieldByName(ColumnName);
            if (field == null)
                return Error(Messages.InvalidRequest, "formatEditor", false);
            //	var field = config.Fields[ColumnName];
        }

        ReportConditionalFormattingSettings formatSetting = ReportStructRoutines.GetFormatSetting(NamespaceId, EntityId, ReportId, ConfigId,
            ColumnName, IsColumn, config);
        return View(formatSetting);
    }

    /// <summary>
    /// submit formats new or edited list of current report.
    /// check if the user is authorized to submit changes to format editor window of current report's column or row.
    /// 
    /// </summary>
    /// <param name="FormatSetting"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> formatEditor(ReportConditionalFormattingSettings FormatSetting)
    {
        try
        {
            IdentityUser user = GetUser(User);
            if (user == null || !User.Identity.IsAuthenticated)
                return Error(Messages.ReportAccessDenied, "formatEditor", false);
            ViewBag.user = user;
            if (!user.CheckReportAccess(FormatSetting.NamespaceId, FormatSetting.EntityId, FormatSetting.ReportId))
                return Error(Messages.ReportAccessDenied, "formatEditor", false);
            if (!CheckAccess(user, SystemFeatureId.ReportDesign))
                return Error(Messages.ReportDesignAccessDenied, "formatEditor", false);
            string configId = FormatSetting.ConfigId;
            GetReportConfigResult getReportConfigResult = new(configId);
            if (!await reportConfigManager.GetReportConfig(FormatSetting.NamespaceId, FormatSetting.EntityId,
                FormatSetting.ReportId, getReportConfigResult, user))
                return Error(Messages.InvalidRequest, "formatEditor", false);
            ConfiguredReport config = getReportConfigResult.Config;
            if (!string.IsNullOrWhiteSpace(FormatSetting.ColumnName))
            {
                ConfiguredReport.SelectedField field = config.GetSelectedFieldByName(FormatSetting.ColumnName);
                if (field == null)
                    return Error(Messages.InvalidRequest, "formatEditor", false);
            }

            ReportStructRoutines.SetFormatSetting(FormatSetting, config);
            await reportConfigBackupRestore.Save(config);
            ViewBag.message = "ثبت قالب نمایشی جدید با موفقیت انجام شد.";
        }
        catch (Exception)
        {
            ViewBag.message = "ثبت قالب نمایشی جدید برای ستون با خطا مواجه شد.";
        }

        return View(FormatSetting);
    }

    /// <summary>
    /// show the visual attributes of current column header.
    /// check if you are valid to view these attributes, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="ColumnName"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> displayEditor(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string ColumnName, [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            return Error(Messages.ReportAccessDenied, "displayEditor", false);
        ViewBag.user = user;
        if (!CheckAccess(user, SystemFeatureId.ReportDesign))
            return Error(Messages.ReportDesignAccessDenied, "displayEditor", false);
        GetReportConfigResult getReportConfigResult = new(ConfigId);
        if (!await reportConfigManager.GetReportConfig(NamespaceId, EntityId, ReportId, getReportConfigResult, user))
            return Error("درخواست نامعتبر . چنین طراحی وجود ندارد", "displayEditor", false);
        ConfiguredReport config = getReportConfigResult.Config;
        if (!user.CheckReportAccess(config.Report.NamespaceId, config.Report.EntityId, config.Report.Id))
            return Error(Messages.ReportAccessDenied, "displayEditor", false);
        Report report = config.Report;
        UiEntity entity = report.entity;
        ConfiguredReport.SelectedField field = config.GetSelectedFieldByName(ColumnName);
        if (field == null)
            return Error(Messages.InvalidRequest, "displayEditor", false);
        ReportColumnDisplayEditor currentSetting = ReportStructRoutines.GetColumnProperties(field);
        if (string.IsNullOrEmpty(currentSetting.aliasValue))
        {
            Domain.Entities.Cmmn.Fields.EntityField efld = entity.GetField(ColumnName);
            if (efld != null)
                currentSetting.aliasValue = efld.Name;
        }

        currentSetting.NamespaceId = NamespaceId;
        currentSetting.EntityId = EntityId;
        currentSetting.ReportId = ReportId;
        currentSetting.ConfigId = ConfigId;
        currentSetting.ColumnName = ColumnName;
        //currentSetting.ParentReportFilters
        return View(currentSetting);
    }

    /// <summary>
    /// submit new Edited values for column visual attributes.
    /// check if you are authorized to make these changes to column visual attributes, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="EditorValue"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> displayEditor(ReportColumnDisplayEditor EditorValue)
    {
        try
        {
            IdentityUser user = GetUser(User);
            if (user == null || !User.Identity.IsAuthenticated)
                return Error(Messages.ReportAccessDenied, "displayEditor", false);
            ViewBag.user = user;
            if (!CheckAccess(user, SystemFeatureId.ReportDesign))
                return Error(Messages.ReportDesignAccessDenied, "displayEditor", false);
            string configId = EditorValue.ConfigId;
            GetReportConfigResult getReportConfigResult = new(configId);
            if (!await reportConfigManager.GetReportConfig(EditorValue.NamespaceId, EditorValue.EntityId,
                EditorValue.ReportId, getReportConfigResult, user))
                return Error(Messages.InvalidRequest, "displayEditor", false);
            ConfiguredReport config = getReportConfigResult.Config;
            if (!user.CheckReportAccess(config.Report.NamespaceId, config.Report.EntityId, config.Report.Id))
                return Error(Messages.ReportAccessDenied, "displayEditor", false);
            ConfiguredReport.SelectedField field = config.GetSelectedFieldByName(EditorValue.ColumnName);
            if (field == null)
                return Error(Messages.InvalidRequest, "displayEditor", false);
            ReportStructRoutines.SetColumnProperties(EditorValue.aliasValue, EditorValue.widthValue,
                EditorValue.alignValue, EditorValue.vAlignValue, EditorValue.dirValue, field);
            await reportConfigBackupRestore.Save(config);
            ViewBag.message = "طراحی نمایشی جدید با موفقیت ثبت شد.";
        }
        catch (Exception)
        {
            ViewBag.message = "ثبت طراحی نمایشی جدید با خطا مواجه شد.";
        }

        return View(EditorValue);
    }

    /// <summary>
    /// to view the Print Preview of current report Config and Data.
    /// check if you are valid to view this page.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="ParentReportIds"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> PrintPreview(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string ParentReportIds, [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            return Error(Messages.ReportAccessDenied, "PrintPreview");
        ViewBag.user = user;
        GetReportConfigResult getReportConfigResult = new(ConfigId);
        if (!await reportConfigManager.GetReportConfig(NamespaceId, EntityId, ReportId, getReportConfigResult, user))
            return Error("درخواست نامعتبر. چنین طراحی موجود نیست", "PrintPreview");
        ConfiguredReport config = getReportConfigResult.Config;
        if (!user.CheckReportAccess(config.Report.NamespaceId, config.Report.EntityId, config.Report.Id,
            config.Report))
            return Error(Messages.ReportAccessDenied, "PrintPreview");
        Report report = config.Report;
        UiEntity entity = report.entity;
        PersistenceObject po = UserStatePersistence.GetPersistence(User.Identity.Name, entity.NamespaceId, entity.Id, "Report",
            "Index", ReportId);
        ElasticObject filterValues = po?.FilterValues;
        string culture = CultureHelper.GetCurrentNeutralCulture();
        List<object> arr = ParentReportIds != null ? [.. ParentReportIds.Split(',')] : null;
        ReportData result = await reportDataRoutines.GetReportData(config, true, filterValues, 0,
            po?.SortFields, null, null, arr, culture, true, user);
        ViewBag.recordsPerPage = result.recordsPerPage;
        ViewBag.SortFields = po?.SortFields;
        ViewBag.FilterValues = filterValues;
        ViewBag.ParentReportIds = ParentReportIds;
        if (config.Parent == null)
            SetPagePackId(report);
        result.structure.ParentReportIds = ParentReportIds;
        result.structure.FilterValues = filterValues;
        return View(result);
    }

    private ActionResult Error(string error, string actionName = null, bool notInIframe = true,
        ElasticObject re = null)
    {
        SetInIframe(!notInIframe);
        if (re != null)
            error += " data : " + re;
        return View("~/Views/Error/Error.cshtml",
            new FormErrorViewModel
            {
                ErrorMessage = error
            });
    }

    /// <summary>
    /// get Group by name from list for report of Chart type.
    /// </summary>
    /// <param name="structure"></param>
    /// <returns></returns>
    public static string GetGroupByNames(ReportStructure structure)
    {
        return string.Join(",", GetGroupBys(structure).Select(sc => sc.Alias));
    }
    
    /// <summary>
    /// get Group by name from list for report of Chart type.
    /// </summary>
    /// <param name="structure"></param>
    /// <returns></returns>
    public static List<ColumnFieldDefinition> GetGroupBys(ReportStructure structure)
    {
        return structure.SelectedColumns.Where(
                    sc => sc.aggrType == eAggregationFunctions.GroupByItem).ToList();
    }

    private static string GetFieldName(ReportStructure structure, string fieldId)
    {
        Domain.Entities.Cmmn.Entities.Entity entity = ProjectDefinition.Project.GetEntity(structure.NamespaceId, structure.EntityId);
        Domain.Entities.Cmmn.Fields.EntityField fld = entity?.GetField(fieldId);
        return fld != null ? fld.Name : fieldId;
    }
}
