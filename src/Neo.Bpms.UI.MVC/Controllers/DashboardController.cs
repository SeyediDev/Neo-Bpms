using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.UI.MVC.Controllers;

public class DashboardController(DashboardConfigBackupRestore dashboardConfigBackupRestore, 
    ReportConfigBackupRestore reportConfigBackupRestore,
    DashboardConfigManager dashboardConfigManager,
    DashboardStructRoutines dashboardStructRoutines,
    ReportConfigManager reportConfigManager,
    ControllerMethods controllerMethods,
    FilterManager filterController,
    FilterConfigBackupRestore filterConfigBackupRestore
    ) : BpmsController
{
    /// <summary>
    /// When you Redirect to Dashboard/Index?[parameters] you call this Action, Type [HttpGet],
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get Persistent Data related to the Index to initiate the requested Index Form.
    /// get dashboard Configs.
    /// set a session value to set the right Item in Navigation Menu.
    /// check if the request is for a drill down window or whole dashboard page.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="DashboardId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="ParentReportId"></param>
    /// <param name="ParentReportConfigId"></param>
    /// <param name="ParentReportIds"></param>
    /// <param name="ParentFilterValues"></param>
    /// <param name="re"></param>
    /// <returns></returns>
    public async Task<ActionResult> Index(string NamespaceId, string EntityId,
        string DashboardId, string ConfigId,
        long? FilterId, string ParentReportId, string ParentReportConfigId,
        string ParentReportIds, string ParentFilterValues,
        [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re, CancellationToken cancellationToken=default)
    {
        IdentityUser user = GetUser();
        GetDashboardConfigResult cfgResult = new(ConfigId);

        if (!await dashboardConfigManager.GetDashboardConfig(NamespaceId, EntityId, DashboardId,
            cfgResult, user, cancellationToken))
            return Error(Messages.InvalidRequest, "Index");
        ConfigId = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;
        Dashboard dashboard = config.Dashboard;
        UiEntity entity = dashboard.Entity;
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            return Error("شما به این داشبورد دسترسی ندارید.", "Index");
        PersistenceObject po = UserStatePersistence.GetPersistence(User.Identity.Name, entity.NamespaceId, entity.Id, "Dashboard",
        "Index", dashboard.Id);
        var configuredFilters = await filterConfigBackupRestore.Configurations("Dashboard", NamespaceId, EntityId, DashboardId, cancellationToken);
        (ElasticObject filterValues, _) = await filterController.GetConfiguredFilterValues(FilterId, configuredFilters, user, po, re);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        ViewBag.FilterValues = filterValues;
        ViewBag.CanDesign = CheckAccess(user, SystemFeatureId.DashboardDesign);
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ConfiguredReport parentReportConfig = null;
        if (!string.IsNullOrEmpty(ParentReportConfigId))
        {
            GetReportConfigResult getParentReportConfigResult = new(ParentReportConfigId);
            if (!await reportConfigManager.GetReportConfig(entity.NamespaceId, entity.Id, ParentReportId,
                 getParentReportConfigResult, user, cancellationToken))
                return Error("طراحی مافوق موجود نیست!", "Index");
        }

        ElasticObject parentFilters = new();
        if (!string.IsNullOrEmpty(ParentReportIds) &&
             !string.IsNullOrEmpty(ParentFilterValues))
            parentFilters = controllerMethods.DecodeFilterValues(ParentFilterValues);
        ConfiguredReport.ConfiguredSubReport subReport =
             parentReportConfig?.SubReports?.Values.FirstOrDefault(s => s.DashboardConfigId == config.ConfigId);
        GetReportConfigResult getReportConfigResult = new(ConfigId);
        DashboardData result = await dashboardStructRoutines.GetDashboardData(config, ConfigId, true,
             filterValues, parentReportConfig, subReport, ParentReportIds, parentFilters,
             culture, false, user, cfgResult.Configs, cancellationToken);
        FormComboData.SetCombosData(dashboard, result.Structure, culture, null, GetLocalParameters(user, null));
        if (filterValues != null)
            ComboDataRoutines.SetComboDataSelectedId(result.Structure, filterValues, true, entity);
        result.Structure.NamespaceId = entity.NamespaceId;
        result.Structure.EntityId = entity.Id;
        result.Structure.Form_ReportId = dashboard.Id;
        result.Structure.Name = string.IsNullOrEmpty(config.Name) ? dashboard.Name : config.Name;
        result.Structure.ConfigId = ConfigId;
        SetPagePackId(dashboard);
        //+ "_" + ConfigId
        ViewBag.ParentReportIds = ParentReportIds;
        ViewBag.ParentReportId = ParentReportId;
        ViewBag.ParentReportConfigId = ParentReportConfigId;
        ViewBag.ParentFilterValues = ParentFilterValues;
        ViewBag.ContainerClass = "container-fluid";
        ViewBag.PersistentIsNull = po == null;
        return View(result);
    }

    /// <summary>
    /// any POST Action on Dashboard/Index Page will lead the user to this action. (such as submitting new filter or changing page and etc).
    /// check if user is Valid, otherwise the user will get redirected to Error page with appropriate message.
    /// get Persistent Data related to the Index to initiate the requested Index Form.
    /// get the Value for select DOM element.
    /// define the selectedId for select DOM element
    /// set a session value to set the right Item in Navigation Menu.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="DashboardId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="ParentFilterValues"></param>
    /// <param name="ParentReportId"></param>
    /// <param name="ParentReportIds"></param>
    /// <param name="ParentReportConfigId"></param>
    /// <param name="FilterValues"></param>
    /// <param name="calendar"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Index(string NamespaceId, string EntityId,
         string DashboardId, string ConfigId,
         string ParentReportId, string ParentReportConfigId,
         string ParentReportIds, string ParentFilterValues,
         [ModelBinder(typeof(DynamicActionBinder))]
         ElasticObject FilterValues,
         string calendar = "shamsi", CancellationToken cancellationToken=default)
    {
        IdentityUser user = GetUser();
        GetDashboardConfigResult cfgResult = new(ConfigId);
        if (!await dashboardConfigManager.GetDashboardConfig(NamespaceId, EntityId, DashboardId,
            cfgResult, user, cancellationToken))
            return Error(Messages.InvalidRequest, "Index");
        ConfigId = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;

        Dashboard dashboard = config.Dashboard;
        UiEntity entity = dashboard.Entity;
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            return Error("شما به این داشبورد دسترسی ندارید.", "Index");
        UserStatePersistence.Persist(User.Identity.Name, entity.NamespaceId, entity.Id, "Dashboard", "Index",
             dashboard.Id,
             new PersistenceObject { FilterValues = FilterValues });

        //            if (FilterValues != null && calendar == "shamsi")
        //                FormDataRoutines.SetRecordDateToMiladi(FilterValues, entity);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        ConfiguredReport parentReportConfig = null;
        if (!string.IsNullOrEmpty(ParentReportConfigId))
        {
            GetReportConfigResult getParentReportConfigResult = new(ParentReportConfigId);
            if (!await reportConfigManager.GetReportConfig(entity.NamespaceId, entity.Id, ParentReportId,
                getParentReportConfigResult, user, cancellationToken))
                return Error("طراحی مافوق موجود نیست!", "Index");
        }

        ElasticObject parentFilters = new();
        if (!string.IsNullOrEmpty(ParentReportIds) &&
             !string.IsNullOrEmpty(ParentFilterValues))
            parentFilters = controllerMethods.DecodeFilterValues(ParentFilterValues);
        ConfiguredReport.ConfiguredSubReport subReport =
             parentReportConfig?.SubReports?.Values.FirstOrDefault(s => s.DashboardConfigId == config.ConfigId);
        DashboardData result = await dashboardStructRoutines.GetDashboardData(config, ConfigId, true,
             FilterValues, parentReportConfig, subReport, ParentReportIds, parentFilters,
             culture, false, user, cfgResult.Configs, cancellationToken);
        FormComboData.SetCombosData(dashboard, result.Structure, culture, null, GetLocalParameters(user, null));
        if (FilterValues != null)
            ComboDataRoutines.SetComboDataSelectedId(result.Structure, FilterValues, true, entity);
        calendar ??= ProjectDefinition.Project.DefaultCalendar;
        SetPagePackId(dashboard);
        result.Structure.NamespaceId = entity.NamespaceId;
        result.Structure.EntityId = entity.Id;
        result.Structure.Form_ReportId = dashboard.Id;
        result.Structure.Name = string.IsNullOrEmpty(config.Name) ? dashboard.Name : config.Name;
        result.Structure.ConfigId = ConfigId;
        ViewBag.FilterValues = FilterValues;
        ViewBag.CanDesign = CheckAccess(user, SystemFeatureId.DashboardDesign);
        ViewBag.CanPublishConfigs = CheckAccess(user, SystemFeatureId.PublishConfigs);
        ViewBag.ParentReportIds = ParentReportIds;
        ViewBag.ParentReportId = ParentReportId;
        ViewBag.ParentReportConfigId = ParentReportConfigId;
        ViewBag.ParentFilterValues = ParentFilterValues;
        ViewBag.ContainerClass = "container-fluid";
        ViewBag.PersistentIsNull = false;
        return View(result);
    }

    [HttpGet]
    public async Task<JsonResult> ApiConfig(string namespaceId, string entityId, string dashboardId, string dashboardConfigId, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();
        UiEntity entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId) as UiEntity
            ?? throw new HttpException(400, "درخواست نامعتبر.");
        Dashboard dashboard = entity.GetDashboard(dashboardId) ?? throw new HttpException(400, "درخواست نامعتبر.");
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            throw new HttpException(403, "شما به این داشبورد دسترسی ندارید.");

        GetDashboardConfigResult cfgResult = new(dashboardConfigId);
        await dashboardConfigManager.GetDashboardConfig(namespaceId, entityId, dashboardId,
             cfgResult, user, cancellationToken);
        dashboardConfigId = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;


        DashboardConfigViewModel dashboardConfigViewModel = await dashboardStructRoutines.GetDashboardConfigViewModel(config,
             dashboardConfigId,
             CultureHelper.GetCurrentNeutralCulture(), user, cfgResult.Configs, cancellationToken);
        return Json(dashboardConfigViewModel);
    }

    [HttpPost]
    public async Task<ActionResult> ApiSaveConfigAs(string namespaceId, string entityId,
         string dashboardId, List<ConfigWidget> widgets,
         List<ConfigDiv> divs, string dashboardConfigName, string dashboardConfigId = null, CancellationToken cancellationToken=default)
    {
        try
        {
            IdentityUser user = GetUser();
            UiEntity entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId) as UiEntity
                ?? throw new HttpException(400, "درخواست نامعتبر.");
            Dashboard dashboard = entity.GetDashboard(dashboardId) ?? throw new HttpException(400, "درخواست نامعتبر.");
            if (!AccessServices.CheckDashboardAccess(user, dashboard))
                throw new HttpException(403, "شما به این داشبورد دسترسی ندارید.");
            if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
                return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
            string guid = dashboardConfigId ?? Guid.NewGuid().ToString();
            //todo what happens when dashboardConfigId has already been saved
            ConfiguredDashboard configuredDashboard = new(dashboard, guid, dashboardConfigName)
            {
                Divs = divs,
                Widgets = widgets
            };
            await dashboardConfigBackupRestore.Save(configuredDashboard, cancellationToken);
            return Json(new { ConfiguredDashboardId = guid });
        }
        catch (Exception)
        {
            throw new HttpException(500, "درخواست شما با خطا مواجه شد.");
        }
    }

    /// <summary>
    /// to add new config to current dashboard.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> AddNewConfig([FromBody] AddNewConfigModel model, CancellationToken cancellationToken)
    {
        string ConfigId = "";
        try
        {
            IdentityUser user = GetUser();
            if (ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) is not UiEntity entity)
                return Json("موجودیت ناشناخته " + model.NamespaceId + ":" + model.EntityId);
            Dashboard dashboard = entity.GetDashboard(model.DashboardId);
            if (dashboard == null)
                return Json("داشبورد ناشناخته " + model.NamespaceId + ":" + model.EntityId + ":" + model.DashboardId);
            if (!AccessServices.CheckDashboardAccess(user, dashboard))
                return Json("خطا: شما به این داشبورد دسترسی ندارید.");
            if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
                return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
            ConfiguredDashboard cr = new(dashboard, Guid.NewGuid().ToString(), model.ConfigName)
            {
                //به دلیل بررسی دسترسی برای همه کانفیگ ها
                //و عدم پیاده سازی دسترسی برای کانفیگ داشبورد
                //به طور پیش فرض همه عمومی هستند تا پیاده سازی تکمیل شود.
                IsPublic = model.IsPublic,
                UserId = model.UserId,
                Roles = model.RoleId!=null ? [model.RoleId]: [],
                FolderId = model.FolderId,
                IsDefault = model.IsDefault,
            };
            ViewBag.message = Messages.NewConfigAddedSuccessfully;
            await dashboardConfigBackupRestore.Save(cr, cancellationToken);
            ConfigId = cr.ConfigId;
        }
        catch (Exception)
        {
            ViewBag.message = Messages.NewConfigAdditionFailed;
        }

        return Json(ConfigId);
    }

    /// <summary>
    /// to rename the config of current dashboard.
    /// check if you are authorize to rename config of current dashboard, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> RenameConfig([FromBody] RenameConfigModel model, CancellationToken cancellationToken) //, string oldName
    {
        IdentityUser user = GetUser();
        //if (!User.Identity.IsAuthenticated)
        //	return RedirectToAction("Login", "Account", HttpContext.Request.Url);
        if (ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) is not UiEntity entity)
            return Json("موجودیت ناشناخته " + model.NamespaceId + ":" + model.EntityId);
        Dashboard dashboard = entity.GetDashboard(model.DashboardId);
        if (dashboard == null)
            return Json("داشبورد ناشناخته " + model.NamespaceId + ":" + model.EntityId + ":" + model.DashboardId);
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            return Json("خطا: شما به این داشبورد دسترسی ندارید.");
        if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
            return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
        string configId = model.ConfigId;
        GetDashboardConfigResult cfgResult = new(configId);
        await dashboardConfigManager.GetDashboardConfig(model.NamespaceId, model.EntityId, model.DashboardId,
             cfgResult, user, cancellationToken);
        _ = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;

        config.Name = model.NewName;
        await dashboardConfigBackupRestore.Save(config, cancellationToken);
        return Json(model.NewName);
    }

    /// <summary>
    /// to set the config of current dashboard.
    /// check if you are authorize to rename config of current dashboard, otherwise you get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="NamespaceId"></param>
    /// <param name="EntityId"></param>
    /// <param name="DashboardId"></param>
    /// <param name="ConfigId"></param>
    /// <param name="Name"></param>
    /// <param name="IsPublic"></param>
    /// <param name="UserGroupId"></param>
    /// <param name="FolderId"></param>
    /// <param name="IsDefault"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SetConfig(string NamespaceId, string EntityId, string DashboardId,
         string ConfigId, string Name,
         bool? IsPublic, string? UserGroupId, long? FolderId, bool? IsDefault, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();
        if (ProjectDefinition.Project.GetEntity(NamespaceId, EntityId) is not UiEntity entity)
            return Json("موجودیت ناشناخته " + NamespaceId + ":" + EntityId);
        Dashboard dashboard = entity.GetDashboard(DashboardId);
        if (dashboard == null)
            return Json("داشبورد ناشناخته " + NamespaceId + ":" + EntityId + ":" + DashboardId);
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            return Json("خطا: شما به این داشبورد دسترسی ندارید.");
        if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
            return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
        GetDashboardConfigResult cfgResult = new(ConfigId);
        if( !await dashboardConfigManager.GetDashboardConfig(NamespaceId, EntityId, DashboardId,
            cfgResult, user, cancellationToken))
            return Json("خطا: چنین طراحی وجود ندارد");
        ConfigId = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;
        if ((IsPublic ?? false) && !CheckAccess(user, SystemFeatureId.PublishConfigs))
            return Json("خطا: شما دسترسی انتشار طراحی ندارید.");
        if (!string.IsNullOrEmpty(UserGroupId) && !user.IsAdmin && !user.CheckRole(UserGroupId))
        {
            return Json("خطا: شما دسترسی به رول فوق را ندارید.");
        }
        config.UserId = user.Id;
        config.IsPublic = IsPublic ?? false;
        config.Roles = UserGroupId!=null ? [UserGroupId] : [];
        config.FolderId = FolderId ?? 0;
        config.IsDefault = IsDefault ?? false;
        config.Name = Name;
        await dashboardConfigBackupRestore.Save(config, cancellationToken);
        return Json(config.ConfigId);
    }

    [HttpPost]
    public async Task<JsonResult> DeleteConfig([FromBody] DeleteDashboardConfigModel model, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();
        if (ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) is not UiEntity entity)
            return Json("موجودیت ناشناخته " + model.NamespaceId + ":" + model.EntityId);
        Dashboard dashboard = entity.GetDashboard(model.DashboardId);
        if (dashboard == null)
            return Json("داشبورد ناشناخته " + model.NamespaceId + ":" + model.EntityId + ":" + model.DashboardId);
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            return Json("خطا: شما به این داشبورد دسترسی ندارید.");
        if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
            return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
        await dashboardConfigBackupRestore.RemoveConfig(model.NamespaceId, model.EntityId, model.DashboardId, model.ConfigId, cancellationToken);
        return Json("success");
    }

    [HttpPost]
    public async Task<JsonResult> AddNewWidget([FromBody] AddNewWidgetModel model, CancellationToken cancellationToken)
    {
        AddWidgetResponse res = new();
        try
        {
            IdentityUser user = GetUser();
            if (ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) is not UiEntity entity)
                return Json("خطا:موجودیت ناشناخته " + model.NamespaceId + ":" + model.EntityId);
            Dashboard dashboard = entity.GetDashboard(model.DashboardId);
            if (dashboard == null)
                return Json("خطا:داشبورد ناشناخته " + model.NamespaceId + ":" + model.EntityId + ":" + model.DashboardId);
            if (!AccessServices.CheckDashboardAccess(user, dashboard))
                return Json("خطا: شما به این داشبورد دسترسی ندارید.");
            if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
                return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
            string configId = model.ConfigId;
            GetDashboardConfigResult cfgResult = new(configId);
            if (!await dashboardConfigManager.GetDashboardConfig(model.NamespaceId, model.EntityId, model.DashboardId,
                cfgResult, user, cancellationToken))
                return Json("خطا:درخواست نامعتبر");
            configId = cfgResult.ConfigId;
            ConfiguredDashboard config = cfgResult.Config;
            string reportConfigId = model.ReportConfigId;
            GetReportConfigResult getReportConfigResult = new(reportConfigId);
            if (!await reportConfigManager.GetReportConfig(model.ReportNamespaceId, model.ReportEntityId, model.ReportId,
                      getReportConfigResult, user, cancellationToken))
                return Json("خطا:درخواست نامعتبر");
            ConfiguredReport configuredReport = getReportConfigResult.Config;
            ConfigWidget widget = new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportNamespaceId = configuredReport.Report.entity.NamespaceId,
                ReportEntityId = configuredReport.Report.entity.Id,
                ReportId = configuredReport.Report.Id,
                ReportConfigId = reportConfigId,
            };
            config.Widgets.Add(widget);
            ConfigDiv div = new()
            {
                Id = Guid.NewGuid().ToString(),
                IsRow = false,
                WidgetId = widget.Id,
                Widget = widget,
                Width = 12
                //todo changeback first 12 to 6
            };
            config.Divs.Add(div);
            ViewBag.message = Messages.NewConfigAddedSuccessfully;
            await dashboardConfigBackupRestore.Save(config, cancellationToken);
            res.Div = div;
            res.Widget = widget;
        }
        catch (Exception)
        {
            ViewBag.message = "خطا:ثبت طراحی جدید برای گزارش با خطا مواجه شد.";
        }

        return Json(res);
    }

    [HttpPost]
    public async Task<JsonResult> RemoveWidget([FromBody] RemoveWidgetModel removeWidgetModel, CancellationToken cancellationToken)
    {
        try
        {
            IdentityUser user = GetUser();
            if (ProjectDefinition.Project.GetEntity(removeWidgetModel.NamespaceId, removeWidgetModel.EntityId) is not UiEntity entity)
                return Json("خطا:موجودیت ناشناخته " + removeWidgetModel.NamespaceId + ":" + removeWidgetModel.EntityId);
            Dashboard dashboard = entity.GetDashboard(removeWidgetModel.DashboardId);
            if (dashboard == null)
                return Json("خطا:داشبورد ناشناخته " + removeWidgetModel.NamespaceId + ":" + removeWidgetModel.EntityId + ":" + removeWidgetModel.DashboardId);
            if (!AccessServices.CheckDashboardAccess(user, dashboard))
                return Json("خطا: شما به این داشبورد دسترسی ندارید.");
            if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
                return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
            string configId = removeWidgetModel.ConfigId;
            GetDashboardConfigResult cfgResult = new(configId);
            if (!await dashboardConfigManager.GetDashboardConfig(removeWidgetModel.NamespaceId, removeWidgetModel.EntityId, removeWidgetModel.DashboardId,
                cfgResult, user, cancellationToken))
                return Json("خطا:درخواست نامعتبر");
            configId = cfgResult.ConfigId;
            ConfiguredDashboard config = cfgResult.Config;
            
            config.Divs.RemoveAt(config.Divs.FindIndex(d => d.WidgetId == removeWidgetModel.WidgetId));
            config.Widgets.RemoveAt(config.Widgets.FindIndex(w => w.Id == removeWidgetModel.WidgetId));
            ViewBag.message = Messages.NewConfigAddedSuccessfully;
            await dashboardConfigBackupRestore.Save(config, cancellationToken);
        }
        catch (Exception)
        {
            ViewBag.message = "خطا:ثبت طراحی جدید برای گزارش با خطا مواجه شد.";
        }

        return Json("OK");
    }

    [HttpPost]
    public async Task<JsonResult> SubmitWidgetSettings([FromBody] SubmitWidgetSettingsModel model, CancellationToken cancellationToken)
    {
        try
        {
            IdentityUser user = GetUser();
            if (ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) is not UiEntity entity)
                return Json("خطا:موجودیت ناشناخته " + model.NamespaceId + ":" + model.EntityId);
            Dashboard dashboard = entity.GetDashboard(model.DashboardId);
            if (dashboard == null)
                return Json("خطا:داشبورد ناشناخته " + model.NamespaceId + ":" + model.EntityId + ":" + model.DashboardId);
            if (!AccessServices.CheckDashboardAccess(user, dashboard))
                return Json("خطا: شما به این داشبورد دسترسی ندارید.");
            if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
                return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
            string configId = model.ConfigId;
            GetDashboardConfigResult cfgResult = new(configId);
            if (!await dashboardConfigManager.GetDashboardConfig(model.NamespaceId, model.EntityId, model.DashboardId,
                cfgResult, user, cancellationToken))
                return Json("خطا:درخواست نامعتبر");
            configId = cfgResult.ConfigId;
            ConfiguredDashboard config = cfgResult.Config;

            ConfigWidget widget = config.Widgets.FirstOrDefault(w => w.Id == model.WidgetId);
            if (widget == null)
                return Json("خطا:درخواست نامعتبر");
            if (!string.IsNullOrEmpty(model.RecordsCount))
                widget.SetProperty(eControlPropertyId.MaxRecordCount, model.RecordsCount);
            if (!string.IsNullOrEmpty(model.WidgetHeight))
                widget.SetProperty(eControlPropertyId.HeightInPixels, model.WidgetHeight);
            Report widgetReport = ProjectDefinition.Project
                 .GetUiEntity(widget.ReportNamespaceId, widget.ReportEntityId)
                 ?.GetReport(widget.ReportId);
            ConfiguredReport reportConfig = await reportConfigBackupRestore.GetConfig(widgetReport, widget.ReportConfigId,cancellationToken );
            if (reportConfig == null)
                return Json("خطا:درخواست نامعتبر");
            if (!string.IsNullOrEmpty(model.ChartType))
            {
                Enum.TryParse(model.ChartType, out Report.ChartType eChartType);
                reportConfig.ChartType = eChartType;
            }

            await reportConfigBackupRestore.Save(reportConfig, cancellationToken);
            config.Divs.Find(d => d.WidgetId == model.WidgetId).Width = model.WidgetWidth;
            await dashboardConfigBackupRestore.Save(config, cancellationToken);
        }
        catch (Exception e)
        {
            ViewBag.message = "خطا:ثبت طراحی جدید برای گزارش با خطا مواجه شد." + e;
            return Json("Error");
        }

        return Json("OK");
    }

    [HttpPost]
    public async Task<JsonResult> SubmitWidgetsOrder([FromBody] SubmitWidgetOrderModel model, CancellationToken cancellationToken)
    {
        try
        {
            IdentityUser user = GetUser();
            if (ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) is not UiEntity entity)
                return Json("خطا:موجودیت ناشناخته " + model.NamespaceId + ":" + model.EntityId);
            Dashboard dashboard = entity.GetDashboard(model.DashboardId);
            if (dashboard == null)
                return Json("خطا:داشبورد ناشناخته " + model.NamespaceId + ":" + model.EntityId + ":" + model.DashboardId);
            if (!AccessServices.CheckDashboardAccess(user, dashboard))
                return Json("خطا: شما به این داشبورد دسترسی ندارید.");
            if (!CheckAccess(user, SystemFeatureId.DashboardDesign))
                return Json("خطا: شما دسترسی طراحی داشبورد ندارید.");
            string configId = model.ConfigId;
            GetDashboardConfigResult cfgResult = new(configId);
            if (!await dashboardConfigManager.GetDashboardConfig(model.NamespaceId, model.EntityId, model.DashboardId,
                cfgResult, user, cancellationToken))
                return Json("خطا:درخواست نامعتبر");
            configId = cfgResult.ConfigId;
            ConfiguredDashboard config = cfgResult.Config;

            //todo irad dare. felan:
            List<ConfigDiv> divs = config.Divs;
            config.Divs = [];
            foreach (string widgetId in model.WidgetIds)
            {
                ConfigDiv div = divs.FirstOrDefault(d => d.WidgetId == widgetId);
                if (div == null) continue;
                config.Divs.Add(div);
                divs.RemoveAt(divs.FindIndex(d => d.WidgetId == widgetId));
            }

            config.Divs.AddRange(divs);
            await dashboardConfigBackupRestore.Save(config, cancellationToken);
        }
        catch (Exception e)
        {
            ViewBag.message = "خطا:ثبت طراحی جدید برای گزارش با خطا مواجه شد." + e;
        }

        return Json("OK");
    }

    private ActionResult Error(string error, string actionName = null, bool showHeader = true,
         ElasticObject re = null)
    {
        ViewBag.preventHeader = !showHeader;

        if (re != null)
            error += " data : " + re;

        return View("~/Views/Error/Error.cshtml", new FormErrorViewModel { ErrorMessage = error });
    }

    private void SetPagePackId(Dashboard dashboard)
    {
        SetPagePackId(dashboard.PagePackId);
    }

    /// <summary>
    /// Refresh widget data without page reload
    /// </summary>
    [HttpPost]
    public async Task<JsonResult> RefreshWidget([FromBody] RefreshWidgetModel model, CancellationToken cancellationToken)
    {
        try
        {
            IdentityUser user = GetUser();
            
            GetDashboardConfigResult cfgResult = new(model.ConfigId);
            if (!await dashboardConfigManager.GetDashboardConfig(model.NamespaceId, model.EntityId, model.DashboardId,
                cfgResult, user, cancellationToken))
                return Json(new { success = false, error = "کانفیگ داشبورد یافت نشد" });

            ConfiguredDashboard config = cfgResult.Config;
            ConfiguredDashboard.ConfigWidget widget = config.Widgets?.FirstOrDefault(w => w.Id == model.WidgetId);
            
            if (widget == null)
                return Json(new { success = false, error = "ویجت یافت نشد" });

            // Get the widget's report configuration
            UiEntity entity = ProjectDefinition.Project.GetEntity(widget.ReportNamespaceId, widget.ReportEntityId) as UiEntity;
            Report report = entity?.GetReport(widget.ReportId);
            if (report == null)
                return Json(new { success = false, error = "گزارش یافت نشد" });

            ConfiguredReport reportConfig = await reportConfigBackupRestore.GetConfig(report, widget.ReportConfigId, cancellationToken);
            if (reportConfig == null)
                return Json(new { success = false, error = "کانفیگ گزارش یافت نشد" });

            // Get filter values from persistence
            PersistenceObject po = UserStatePersistence.GetPersistence(User.Identity.Name, 
                entity.NamespaceId, entity.Id, "Dashboard", "Index", model.DashboardId);
            ElasticObject filterValues = po?.FilterValues;

            int maxRecord = DashboardDataRoutines.GetWidgetMaxRecord(widget, reportConfig.ViewType);
            string culture = CultureHelper.GetCurrentNeutralCulture();

            // Get report data
            ReportDataRoutines reportDataRoutines = HttpContext.RequestServices.GetRequiredService<ReportDataRoutines>();
            ReportData reportResult = await reportDataRoutines.GetReportData(reportConfig, true, filterValues, 1, null, null, null, null, culture, false, user, maxRecord, cancellationToken: cancellationToken);

            // For now, just return success - full HTML rendering would need a different approach
            // Client-side can trigger a full page refresh if needed
            return Json(new { 
                success = true, 
                message = "داده‌های ویجت با موفقیت به‌روز شد. صفحه را refresh کنید تا تغییرات را ببینید.",
                recordCount = reportResult.RecordCount
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }
}

// ===== DASHBOARD MODELS =====

public class DeleteDashboardConfigModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
}

public class AddNewConfigModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigName { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? UserId { get; set; }
    public string? RoleId { get; set; }
    public long? FolderId { get; set; }
    public bool IsDefault { get; set; }
}

public class RenameConfigModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
    public string NewName { get; set; }
}

public class AddNewWidgetModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
    public string ReportNamespaceId { get; set; }
    public string ReportEntityId { get; set; }
    public string ReportId { get; set; }
    public string ReportConfigId { get; set; }
}

public class RemoveWidgetModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
    public string WidgetId { get; set; }
}

public class RefreshWidgetModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
    public string WidgetId { get; set; }
}

public class SubmitWidgetSettingsModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
    public string WidgetId { get; set; }
    public string ChartType { get; set; }
    public int WidgetWidth { get; set; }
    public string WidgetHeight { get; set; }
    public string RecordsCount { get; set; }
}

public class SubmitWidgetOrderModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string ConfigId { get; set; }
    public string[] WidgetIds { get; set; }
}
