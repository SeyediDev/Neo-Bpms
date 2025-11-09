using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class DashboardStructRoutines(
    ReportConfigBackupRestore reportConfigBackupRestore,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore,
    DashboardDataRoutines dashboardDataRoutines,
    DashboardConfigManager dashboardConfigManager,
    FormStructRoutines formStructRoutines,
    IFormLogicHelper formLogicHelper,
    ReportConfigManager reportConfigManager,
    ReportFilterName reportFilterName)
{
    public async Task<DashboardData> GetDashboardData(
        ConfiguredDashboard config, string configId, bool loadData, ElasticObject filterValues,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        string parentReportIds, ElasticObject parentFilters,
        string culture, bool forPrint, IdentityUser user, List<ConfiguredDashboard> dashboardConfigs, CancellationToken cancellationToken)
    {
        DashboardData result = new()
        {
            Structure = await GetDashboardConfigViewModel(config, configId, culture, user, dashboardConfigs, cancellationToken)
        };
        result.Structure.FilterValues = filterValues;
        LocalParameters lp = new(user) { { "userId", user?.Id } };
        if (loadData)
        {
            await dashboardDataRoutines.GetDashboardData(result, config,
                parentReportConfig, subReport,
                parentReportIds, parentFilters,
                culture, forPrint, user, reportConfigBackupRestore);
        }

        FormComboData.SetCombosData(config.Dashboard, result.Structure, culture, null, lp);
        if (filterValues != null)
            ComboDataRoutines.SetComboDataSelectedId(result.Structure, filterValues, true, config.Dashboard.Entity);
        return result;
    }


    private async Task<CommonFormStructure> GetCommonStructure(string culture,
        DashboardStructure structure, Dashboard dashboard, IdentityUser user,
        string tableName, List<ConfiguredDashboard> dashboardConfigs, CancellationToken cancellationToken)
    {
        UiEntity entity = dashboard?.Entity;
        if (entity == null) return null;
        structure.NamespaceId = entity.model.Id;
        structure.EntityId = entity.Id;
        structure.Name = dashboard.Name;
        structure.Form_ReportId = dashboard.Id;
        if (dashboard.formFields != null)
        {
            foreach (FormField field in dashboard.formFields)
            {
                structure.hasMandatoryFilter = true;
                InputFieldDefinition iff = formStructRoutines.NewInputFieldDefinition(structure, field, culture, true);
                if (field.GetProperties() == null) continue;
                foreach (FormProperty prop in field.GetProperties())
                {
                    iff.AddProperty(prop.id, prop.value);
                }
            }
        }

        formLogicHelper.AddFormLogic(structure, dashboard.UiRules, tableName);
        if (entity.KeyFields != null)
        {
            foreach (EntityField field in entity.KeyFields)
                structure.KeyFields.Add(field.Id);
        }

        EntityItem entityItem = new()
        {
            ItemType = "Dashboard",
            NamespaceId = dashboard.NamespaceId,
            EntityId = entity.Id,
            ItemId = dashboard.Id
        };

        structure.Configs = dashboardConfigs;

        var configuredFilters = await filterConfigBackupRestore.Configurations(entityItem, cancellationToken);
        structure.ConfiguredFilters = configuredFilters?.Where(cfg =>
                cfg.CheckAccess(user)).ToList();
        var configuredFolders = await folderConfigBackupRestore.Configurations(entityItem, cancellationToken);
        structure.ConfiguredFolders = configuredFolders?.Where(cfg =>
                cfg.CheckAccess(user)).ToList();

        structure.UserGroups = [.. user.Roles.Values.Where(role =>dashboard.CheckRole(role.Code))];
        return structure;
    }

    public async Task<DashboardConfigViewModel> GetDashboardConfigViewModel(ConfiguredDashboard config, string configId,
        string culture, IdentityUser user, List<ConfiguredDashboard> dashboardConfigs, CancellationToken cancellationToken)
    {
        DashboardConfigViewModel structure = new() { Reports = [] };
        Dashboard dashboard = config.Dashboard;
        structure.ConfigId = configId;
        structure.ActiveConfig = config;
        if (await GetCommonStructure(culture, structure, dashboard, user, null, dashboardConfigs, cancellationToken) == null)
            return null;
        foreach (Dashboard.DashboardReport dashboardReport in config.Dashboard.Reports)
        {
            UiEntity dashboardEntity = dashboard.Entity.NamespaceId == dashboardReport.NamespaceId &&
                                   dashboard.Entity.Id == dashboardReport.EntityId
                ? dashboard.Entity
                : (UiEntity)ProjectDefinition.Project.GetEntity(dashboardReport.NamespaceId, dashboardReport.EntityId);
            Report report = dashboardEntity?.GetReport(dashboardReport.ReportId);
            if (report == null) continue;
            List<DashboardConfigViewModel.ReportViewModel.ConfigViewModel> reportConfigViewModels = [];
            List<ConfiguredReport> reportConfigs = await reportConfigBackupRestore.Configurations(report, cancellationToken);
            foreach (ConfiguredReport reportConfig in reportConfigs)
            {
                if (reportConfig.ViewType is ReportViewType.Chart or ReportViewType.GroupByList or ReportViewType.List)
                {
                    reportConfigViewModels.Add(new DashboardConfigViewModel.ReportViewModel.ConfigViewModel
                    {
                        Id = reportConfig.ConfigId,//TODO
                        Name = reportConfig.Name,
                        viewType = reportConfig.ViewType,
                        ChartType = reportConfig.ChartType,
                    });
                }
            }
            structure.Reports.Add(new DashboardConfigViewModel.ReportViewModel
            {
                Id = report.Id,
                NamespaceId = report.entity.model.Id,
                EntityId = report.entity.Id,
                Name = report.Name,
                Configs = reportConfigViewModels
            });
        }
        await Task.CompletedTask;
        return structure;
    }

    public async Task<string> GetFilterNames(DashboardConfigViewModel structure,
        string parentReportId, string parentReportConfigId,
        string parentReportIds, ElasticObject parentFilterValues, IdentityUser user, string culture)
    {
        string configId = structure.ConfigId;
        GetDashboardConfigResult cfgResult = new(configId);
        if (!await dashboardConfigManager.GetDashboardConfig(structure.NamespaceId, structure.EntityId, structure.Form_ReportId,
            cfgResult, user, CancellationToken.None))
            return null;
        configId = cfgResult.ConfigId;
        ConfiguredDashboard config = cfgResult.Config;

        ConfiguredReport parentConfig = null;
        if (!string.IsNullOrEmpty(parentReportId) &&
            !string.IsNullOrEmpty(parentReportConfigId))
        {
            GetReportConfigResult getParentReportConfigResult = new(parentReportConfigId);
            if (!await reportConfigManager.GetReportConfig(structure.NamespaceId,
                structure.EntityId, parentReportId,
                getParentReportConfigResult, user))
                return null;
        }

        ConfiguredReport.ConfiguredSubReport subReport = parentConfig?.SubReports?.Values.FirstOrDefault(s => s.DashboardConfigId == configId);
        return reportFilterName.GetFilterNames(structure.NamespaceId, structure.EntityId, structure.FilterValues,
            parentReportIds, structure.CombosData, parentFilterValues, parentConfig,
            structure.Fields, subReport, culture);
    }
}
