using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Reports;
using Neo.Bpms.Domain.Entities.Security.Authorization;
using static Neo.Bpms.Domain.Entities.Cmmn.UI.Report;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ReportController
{
    /// <summary>
    /// to define new config for current report.
    /// check if you are valid to view this page, otherwise you get redirected to Error page with appripriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <returns></returns>
    public ActionResult AddNewConfig(string NamespaceId, string EntityId, string ReportId,
        [ModelBinder(typeof(DynamicActionGetBinder))]
            ElasticObject re)
    {
        IdentityUser user = GetUser();
        if (!user.CheckReportAccess(NamespaceId, EntityId, ReportId))
            return Error(Messages.ReportAccessDenied, "AddNewConfig", false);
        if (!CheckAccess(user, SystemFeatureId.ReportDesign))
            return Error(Messages.ReportDesignAccessDenied, "AddNewConfig", false);
        AddNewConfigParameters result = new()
        {
            NamespaceId = NamespaceId,
            EntityId = EntityId,
            ReportId = ReportId
        };
        return View(result);
    }

    /// <summary>
    /// submit new config into report config collection.
    /// check if you are authorized to add new config into report's config collection, otheriwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="Params"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> AddNewConfig(AddNewConfigParameters Params, CancellationToken cancellationToken)
    {
        try
        {
            IdentityUser user = GetUser();
            if (!user.CheckReportAccess(Params.NamespaceId, Params.EntityId, Params.ReportId))
                return Error(Messages.ReportAccessDenied, "AddNewConfig", false);
            if (!CheckAccess(user, SystemFeatureId.ReportDesign))
                return Error(Messages.ReportDesignAccessDenied, "AddNewConfig", false);
            if (ProjectDefinition.Project.GetEntity(Params.NamespaceId, Params.EntityId) is not UiEntity entity)
                return Error(Messages.InvalidRequest, "AddNewConfig", false);
            Report report = entity.GetReport(Params.ReportId);
            if (report == null)
                return Error(Messages.InvalidRequest, "AddNewConfig", false);
            //if ((int)Params.viewType == 3)
            //{
            //	//Params.viewType = eReportViewType.WorldMap;
            //	Params.chartType = Report.ChartType.WorldMap;
            //}
            //else if ((int)Params.viewType == 5)
            //{
            //	Params.viewType = eReportViewType.IranMap;
            //	Params.chartType = Report.ChartType.IranMap;
            //}

            ConfiguredReport cr = new(report, Params.viewType,
                Guid.NewGuid().ToString(), Params.NewConfigName)
            {
                UserId = user.Id,
            };

            if (cr.ViewType == ReportViewType.Chart)
                cr.ChartType = Params.chartType;

            await reportConfigBackupRestore.Save(cr, cancellationToken);
            //return Index(NamespaceId, EntityId, ReportId, cr.Id, "", 1, null);
            ViewBag.message = Messages.NewConfigAddedSuccessfully;
            ViewBag.ConfiguredReportId = cr.ConfigId;
        }
        catch (Exception)
        {
            ViewBag.message = Messages.NewConfigAdditionFailed;
        }

        return View(Params);
    }

    /// <summary>
    /// to set the config of current report.
    /// check if you are authorize to rename config of current dashboard, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="Name"></param>
    /// <param name="IsPublic"></param>
    /// <param name="UserGroupId"></param>
    /// <param name="IsDefault"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SetConfig(string NamespaceId, string EntityId, string ReportId,
        string ConfigId, string Name, bool? IsPublic, string? UserGroupId, bool? IsDefault)
    {
        (bool result, IdentityUser user, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(NamespaceId, EntityId, ReportId, ConfigId);
        if (!result)
            return jsonResult;

        if ((IsPublic ?? false) && !CheckAccess(user, SystemFeatureId.PublishConfigs))
            return Json("خطا: شما دسترسی انتشار طراحی گزارش ندارید.");
        if (!string.IsNullOrEmpty(UserGroupId) && !user.IsAdmin && !user.CheckRole(UserGroupId))
        {
            return Json("خطا: شما دسترسی به نقش فوق را ندارید.");
        }
        if (config.IsPublic && !CheckAccess(user, SystemFeatureId.PublishConfigs))
            return Json("خطا: شما دسترسی تغییر طراحی همگانی گزارش را ندارید.");
        config.UserId = user.Id;
        config.IsPublic = IsPublic ?? false;
        config.Roles = UserGroupId != null ? [UserGroupId] : [];
        config.IsDefault = IsDefault ?? false;
        config.Name = Name;
        await reportConfigBackupRestore.Save(config);
        return Json(config.ConfigId);
    }

    [HttpPost]
    public async Task<JsonResult> DeleteConfig(string ConfigId, CancellationToken cancellationToken)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig("", "", "", ConfigId);
        if (!result)
            return jsonResult;
        await reportConfigBackupRestore.RemoveConfig(config, cancellationToken);
        if (config.Parent?.ParentConfiguredReport.SubReports != null)
        {
            config.Parent.ParentConfiguredReport.SubReports.TryRemove(ConfigId, out _);
        }

        await reportConfigBackupRestore.RemoveConfig(config, cancellationToken);
        return Json("success");
    }
    [HttpPost]
    public async Task<JsonResult> CloneConfig(string namespaceId, string entityId, string reportId, string configId, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            throw new HttpException(Messages.ReportAccessDenied);
        if (!CheckAccess(user, SystemFeatureId.ReportDesign))
            throw new HttpException(Messages.ReportDesignAccessDenied);
        GetReportConfigResult getReportConfigResult = new(configId);
        if (!await reportConfigManager.GetReportConfig(namespaceId, entityId, reportId,
            getReportConfigResult, user, cancellationToken))
            throw new HttpException(Messages.InvalidRequest);
        ConfiguredReport originalConfig = getReportConfigResult.Config;
        if (!user.CheckReportAccess(originalConfig.Report.NamespaceId, originalConfig.Report.EntityId, originalConfig.Report.Id))
            throw new HttpException(Messages.ReportAccessDenied);
        UiEntity entity = ProjectDefinition.Project.GetEntity(originalConfig.Report.NamespaceId, originalConfig.Report.EntityId) as UiEntity ?? throw new HttpException(Messages.InvalidRequest);
        _ = entity.GetReport(originalConfig.Report.Id) ?? throw new HttpException(Messages.InvalidRequest);
        if (originalConfig.Parent != null)
            throw new HttpException(Messages.InvalidRequest);
        ConfiguredReport config = originalConfig.Clone(null, null);
        foreach (ConfiguredReport.ConfiguredSubReport subReport in config.SubReports.Values)
        {
            await reportConfigBackupRestore.Save(subReport.ConfiguredReport, cancellationToken);
        }
        config.Name = $"{ViewTexts.Copy} {originalConfig.Name}";
        config.IsMeta = false;
        if (originalConfig.IsPublic && !CheckAccess(user, SystemFeatureId.PublishConfigs))
        {
            config.IsPublic = false;
            config.UserId = user.Id;
        }
        await reportConfigBackupRestore.Save(config, cancellationToken);
        return Json(config.ConfigId);
    }

    /// <summary>
    /// to change the type of sub config.
    /// </summary>
    /// <param name="ConfigId"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ChangeSubConfigType(string ConfigId)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig("", "", "", ConfigId);
        if (!result)
            return jsonResult;
        if (config.Parent?.ParentConfiguredReport == null)
            return Json("لطفا یک زیرگزارش را انتخاب کنید.");

        config.Parent.Type = config.Parent?.Type == SubReportType.DrillDown
            ? SubReportType.SubReport
            : SubReportType.DrillDown;
        await reportConfigBackupRestore.Save(config);
        return Json("Ok");
    }

    /// <summary>
    /// to rename the config of current report.
    /// check if you are authorize to rename config of current report, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> RenameConfig(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        string newName)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(NamespaceId, EntityId, ReportId, ConfigId);
        if (!result)
            return jsonResult;
        config.Name = newName;
        await reportConfigBackupRestore.Save(config);
        return Json(newName);
    }

    /// <summary>
    /// add new sub report to current report config.
    /// check if you are authorize to add new sub report to current report config, otherwise you get redirected to error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="type"></param>
    /// <param name="SubNamespaceId"></param>
    /// <param name="SubEntityId"></param>
    /// <param name="SubReportId"></param>
    /// <param name="associationName"></param>
    /// <param name="viewType"></param>
    /// <param name="chartType"></param>
    /// <param name="SubReportName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> AddSubReport(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        SubReportType type, string SubNamespaceId, string SubEntityId,
        string SubReportId, string associationName, ReportViewType viewType, ChartType? chartType,
        string dashboardConfigId, string SubReportName, CancellationToken cancellationToken)
    {
        (bool result, IdentityUser user, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(NamespaceId, EntityId, ReportId, ConfigId);
        if (!result)
            return jsonResult;
        if ((int)viewType == 3)
        {
            viewType = ReportViewType.Chart;
            chartType = ChartType.WorldMap;
        }
        else if ((int)viewType == 5)
        {
            viewType = ReportViewType.Chart;
            chartType = ChartType.IranMap;
        }

        if (string.IsNullOrEmpty(SubNamespaceId))
            SubNamespaceId = NamespaceId;
        string dashboardId = "";
        if (viewType == ReportViewType.Dashboard)
        {
            ConfiguredDashboard dashboardConfig = await dashboardConfigBackupRestore.GetConfig(dashboardConfigId, cancellationToken);
            if (dashboardConfig == null)
                return Error(Messages.InvalidRequest, "AddSubReport", false);
            SubNamespaceId = NamespaceId;
            SubEntityId = EntityId;
            SubReportId = ReportId;
            SubReportName = dashboardConfig.Name;
            dashboardId = dashboardConfig.Dashboard.Id;
        }

        if (ProjectDefinition.Project.GetEntity(SubNamespaceId, SubEntityId) is not UiEntity subEntity)
            return Error(Messages.InvalidRequest, "AddSubReport", false);
        Report newSubReport = subEntity.GetReport(SubReportId);
        ConfiguredReport subConfig = new(newSubReport, viewType, Guid.NewGuid().ToString(), SubReportName)
        {
            ChartType = chartType ?? 0,
            UserId = user.Id,
            IsPublic = config.IsPublic,
            Roles = [.. config.Roles],
            IsDefault = config.IsDefault,
            FolderId = config.FolderId,
        };
        ConfiguredReport.ConfiguredSubReport sr = config.AddSubReport(type, subConfig);
        sr.AssociationName = associationName;
        sr.DashboardId = dashboardId;
        sr.DashboardConfigId = dashboardConfigId;
        await reportConfigBackupRestore.Save(sr.ConfiguredReport, cancellationToken);
        ViewBag.message = "زیرگزارش " + subConfig.Name + " با موفقیت ثبت شد";
        PossibleSubReportSelections possibleSubReportSelections = 
            await ReportStructRoutines.GetPossibleSubReportSelections(NamespaceId, EntityId, ReportId, ConfigId,
            config, type, dashboardConfigBackupRestore);
        possibleSubReportSelections.SubReports = [.. config.SubReports.Values];
        ViewBag.SubReportType = type;
        return View(possibleSubReportSelections);
    }

    /// <summary>
    /// to add new sub report into current report config, you get redirected to this page
    /// check if you are valid to view this page, otherwise you get redirected to error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="ReportId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> AddSubReport(string NamespaceId, string EntityId, string ReportId, string ConfigId,
        SubReportType type)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(NamespaceId, EntityId, ReportId, ConfigId);
        if (!result)
            return jsonResult;

        PossibleSubReportSelections possibleSubReportSelections = await ReportStructRoutines.GetPossibleSubReportSelections(NamespaceId, EntityId,
            ReportId, ConfigId,
            config, type, dashboardConfigBackupRestore);
        possibleSubReportSelections.SubReports = [.. config.SubReports.Values];
        ViewBag.SubReportType = type;
        return View(possibleSubReportSelections);
    }

    [HttpPost]
    public async Task<JsonResult> ApplyProperties(string NamespaceId, string EntityId,
        string ReportId, string ConfigId,
        PostedRange range,
    PostedRange[] levels,
        PostedProperty[] properties, CancellationToken cancellationToken)
    {
        (bool result, IdentityUser user, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(NamespaceId, EntityId, ReportId, ConfigId);
        if (!result)
            return jsonResult;

        ReportConfigManager.SetConfigProperties(range?.GetRange(), levels?.Select(l => l?.GetRange()).ToArray(), properties, config);
        await reportConfigBackupRestore.Save(config, cancellationToken);
        return Json(new { OK = "OK" });
    }

    [HttpPost]
    public async Task<JsonResult> ApplyOrdersSettings([FromBody] ApplyOrdersSettingsModel model, CancellationToken cancellationToken)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(model.NamespaceId, model.EntityId, model.ReportId, model.ConfigId);
        if (!result)
            return jsonResult;

        if (model.SortFields != config.SortFields)
        {
            config.SortFields = model.SortFields;
            await reportConfigBackupRestore.Save(config, cancellationToken);
        }

        return Json(new { OK = "OK" });
    }

    [HttpPost]
    public async Task<JsonResult> ApplyColorsSettings([FromBody] ApplyColorsSettingsModel model, CancellationToken cancellationToken)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(model.NamespaceId, model.EntityId, model.ReportId, model.ConfigId);
        if (!result)
            return jsonResult;

        config.Formats = [];
        AddRowFormat("table-success", model.Success, config);
        AddRowFormat("table-danger", model.Danger, config);
        AddRowFormat("table-info", model.Info, config);
        AddRowFormat("table-warning", model.Warning, config);
        AddRowFormat("table-active", model.Active, config);
        await reportConfigBackupRestore.Save(config, cancellationToken);
        return Json(new { OK = "OK" });
    }

    [HttpPost]
    public async Task<JsonResult> ApplyColumns([FromBody] ColumnsBindingModel model, CancellationToken cancellationToken)
    {
        (bool result, _, ConfiguredReport config, JsonResult jsonResult) =
            await FetchReportConfig(model.NamespaceId, model.EntityId, model.ReportId, model.ConfigId);
        if (!result)
            return jsonResult;
        ReportConfigManager.SetConfigColumns(model.SelectedColumns, config);
        config.HavingCondition = model.HavingConstraint;
        config.WhereCondition = model.Constraint;
        await reportConfigBackupRestore.Save(config, cancellationToken);
        return Json(new { OK = "OK" });
    }

    private async Task<(bool result, IdentityUser user, ConfiguredReport config, JsonResult jsonResult)> 
        FetchReportConfig(string namespaceId, string entityId,
        string reportId, string configId)
    {
        JsonResult jsonResult = null;
        ConfiguredReport config = null;
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            throw new HttpException(Messages.ReportAccessDenied);
        ViewBag.user = user;
        if (!CheckAccess(user, SystemFeatureId.ReportDesign))
            throw new HttpException(Messages.ReportDesignAccessDenied);
        GetReportConfigResult getReportConfigResult = new(configId);
        if (!await reportConfigManager.GetReportConfig(namespaceId, entityId, reportId,
            getReportConfigResult, user))
            throw new HttpException(Messages.InvalidRequest);
        config = getReportConfigResult.Config;
        if (config.IsMeta)
        {
            jsonResult = Json(ViewTexts.ConfigIsNotModifiable);
            return (false, user, config, jsonResult);
        }

        if (!user.CheckReportAccess(config.Report.NamespaceId, config.Report.EntityId, config.Report.Id))
            throw new HttpException(Messages.ReportAccessDenied);
        jsonResult = null;
        return (true, user, config, jsonResult);
    }

    private static void AddRowFormat(string className, string filter, ConfiguredReport config)
    {
        if (!string.IsNullOrEmpty(filter))
            config.Formats.Add(new ConditionalFormatting
            {
                ClassName = className,
                ColumnName = "",
                Filter = filter,
                Type = ConditionalFormatting.eType.Row
            });
    }
}
public class ColumnsBindingModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public PostedOrderdColumn[] SelectedColumns { get; set; }
    public string Constraint { get; set; }
    public string HavingConstraint { get; set; }
}

public class ApplyColorsSettingsModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public string Success { get; set; }
    public string Danger { get; set; }
    public string Info { get; set; }
    public string Warning { get; set; }
    public string Active { get; set; }
}

public class ApplyOrdersSettingsModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public string SortFields { get; set; }
}
