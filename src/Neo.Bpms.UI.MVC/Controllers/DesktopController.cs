using Neo.Bpms.Domain.Entities.Cmmn.Common;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.UI.MVC.Features;
using Microsoft.Extensions.Options;
namespace Neo.Bpms.UI.MVC.Controllers;

public class DesktopController(
    DashboardStructRoutines dashboardStructRoutines,
    DashboardConfigManager dashboardConfigManager,
    FormStructRoutines formStructRoutines,
    ControllerMethods controllerMethods, 
    ReportConfigManager reportConfigManager,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FilterManager filterController, IOptions<CmmnSettings> cmmnSettings) : ControllerBaseMVC
{
    public async Task<ActionResult> Index(string desktopId, CancellationToken cancellationToken)
    {
        if (User.Identity is null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
            //throw new UnauthenticatedUserException();
        }
        IdentityUser user = GetUser();
        ViewBag.ContainerClass = "container-fluid";

        Form desktopForm = GetDesktopForm(user, desktopId, null, Form.eFormType.Dashboard);
        if(desktopForm?.FormType == Form.eFormType.Dashboard )
        {
            (DashboardData Result, string ErrorText) r = await GetDashboardStructureAndData(user, desktopForm.NamespaceId, desktopForm.EntityId, desktopForm.Id, cancellationToken);
            if (r.Result == null)
            {
                ViewBag.preventHeader = false;
                return Error(r.ErrorText );
            }
            return View("~/Views/Dashboard/Index.cshtml", r.Result );
            //RedirectToAction("Index", "Dashboard", new { structure.NamespaceId, structure.EntityId, DashboardId = structure.Form_ReportId });
        }
        desktopForm ??= GetDesktopForm(user, desktopId, null, Form.eFormType.CustomPage);
        CommonFormStructure structure = desktopForm == null
                    ? null
                    : formStructRoutines.GetCommonFormStructure(CultureHelper.GetCurrentNeutralCulture(),
                    desktopForm.NamespaceId, desktopForm.EntityId, desktopForm.Id, Form.eFormType.CustomPage, null, desktopForm, user);
        return View("~/Views/Desktop/Index.cshtml", structure);
    }

    private Form GetDesktopForm(IdentityUser user, string formId, string namespaceId, Form.eFormType formtype)
    {
        bool Condition(Form f)
        {
            return f.FormType == formtype && CheckAccess(user, f);
        }

        UiEntity desktopEntity = ProjectDefinition.Project.GetEntityByEntityId(nameof(HomePageEntity), namespaceId) as UiEntity;
        var forms = desktopEntity?.getForms();
        var dashboards = desktopEntity?.GetDashboards();
        Form desktopForm = forms?.FirstOrDefault(Condition)
                          ?? forms?.FirstOrDefault(f => f.Id == "Default")
                          ?? desktopEntity?.GetDashboard(formId)
                          ?? desktopEntity?.GetDashboard("HomePageDashboard")
                          ?? dashboards?.Values.FirstOrDefault(Condition)
                          ?? dashboards?.Values.FirstOrDefault(f => f.Id == "Default");
        return desktopForm;
    }
    
    private async Task<(DashboardData? Result, string ErrorText)> GetDashboardStructureAndData(
        IdentityUser user, string namespaceId, string entityId, string dashboardId, CancellationToken cancellationToken)
    {
        string ConfigId = "";
        long? FilterId = null;
        string ParentReportConfigId = "";
        string ParentReportId = "";
        string ParentReportIds = "";
        string ParentFilterValues = "";
        ElasticObject re = new();

        GetDashboardConfigResult cfgResult = new(ConfigId);
        if (!await dashboardConfigManager.GetDashboardConfig(namespaceId, entityId, dashboardId,
            cfgResult, user, cancellationToken))
            return (null, "چنین طراحی موجود نیست");
        ConfigId = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;

        Dashboard dashboard = config.Dashboard;
        UiEntity entity = dashboard.Entity;
        if (!AccessServices.CheckDashboardAccess(user, dashboard))
            return (null, "شما به این داشبورد دسترسی ندارید.");
        PersistenceObject po = UserStatePersistence.GetPersistence(User.Identity.Name, entity.NamespaceId, entity.Id, "Dashboard",
             "Index", dashboard.Id);
        var configuredFilters = await filterConfigBackupRestore.Configurations("Dashboard", namespaceId, entityId, dashboardId, cancellationToken);
        (ElasticObject filterValues , _) = await filterController.GetConfiguredFilterValues(FilterId, configuredFilters, user, po, re);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        ViewBag.FilterValues = filterValues;
        ViewBag.CanDesign = cmmnSettings.Value.ShowFormDesign;
        ViewBag.CanPublishConfigs = false;
        ConfiguredReport parentReportConfig = null;
        if (!string.IsNullOrEmpty(ParentReportConfigId))
        {
            GetReportConfigResult getParentReportConfigResult = new(ParentReportConfigId);
            if (!await reportConfigManager.GetReportConfig(entity.NamespaceId, entity.Id, ParentReportId, getParentReportConfigResult, user, cancellationToken))
                return (null, "طراحی مافوق موجود نیست!");
        }

        ElasticObject parentFilters = new();
        if (!string.IsNullOrEmpty(ParentReportIds) &&
             !string.IsNullOrEmpty(ParentFilterValues))
            parentFilters = controllerMethods.DecodeFilterValues(ParentFilterValues);
        ConfiguredReport.ConfiguredSubReport subReport =
             parentReportConfig?.SubReports?.Values.FirstOrDefault(s => s.DashboardConfigId == config.ConfigId);
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
        SetPagePackId(dashboardId);
        //+ "_" + ConfigId
        ViewBag.ParentReportIds = ParentReportIds;
        ViewBag.ParentReportId = ParentReportId;
        ViewBag.ParentReportConfigId = ParentReportConfigId;
        ViewBag.ParentFilterValues = ParentFilterValues;
        ViewBag.ContainerClass = "container-fluid";
        ViewBag.PersistentIsNull = po == null;
        return (result, null);
    }
    internal static LocalParameters GetLocalParameters(IdentityUser user, ElasticObject record)
    {
        LocalParameters lp = new() { { "user", user }, { "userId", user?.Id } };
        if (record != null)
            lp.Add("q", record);
        return lp;
    }
}
