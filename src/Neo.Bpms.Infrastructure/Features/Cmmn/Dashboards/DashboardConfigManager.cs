using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class GetDashboardConfigResult(string configId)
{
    public string ConfigId { get; set; } = configId;
    public ConfiguredDashboard Config { get; set; }
    public List<ConfiguredDashboard> Configs { get; set; }
}

public class DashboardConfigManager(DashboardConfigBackupRestore dashboardConfigBackupRestore, 
    FilterConfigBackupRestore filterConfigBackupRestore, 
    FolderConfigBackupRestore folderConfigBackupRestore)
{
    public async Task<bool> GetDashboardConfig(string namespaceId, string entityId,
        string dashboardId, GetDashboardConfigResult result,
        IdentityUser user, CancellationToken cancellationToken)
    {
        result.Config = null;
        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Dashboard dashboard = uiEntity?.GetDashboard(dashboardId);
        if (dashboard == null)
            return false;
        if (result.Configs == null)
        {
            var configs = await dashboardConfigBackupRestore.Configurations(dashboard, cancellationToken);
            result.Configs = [.. configs.Where(x => x.CheckAccess(user))];
        }
        if (!string.IsNullOrEmpty(result.ConfigId))
        {
            result.Config = await dashboardConfigBackupRestore.GetConfig(dashboard, result.ConfigId, cancellationToken);
            if (result.Config != null)
            {
                if (!CheckAccess(result.Config, user))
                {
                    result.Config = null;
                }
            }
            return result.Config != null;
        }
        if (dashboard.MetaConfigures?.Count is > 0)
        {
            result.Config = dashboard.MetaConfigures.Values.FirstOrDefault(x => x.IsDefault && CheckAccess(x, user)) ??
                dashboard.MetaConfigures.Values.FirstOrDefault(x => !x.IsDefault && CheckAccess(x, user));
            result.ConfigId = result.Config?.ConfigId;
            if (result.Config != null)
            {
                return true;
            }
        }

        result.Config = DashboardConfigBackupRestore.GetDefaultItem(result.Configs, user)
            ?? result.Configs.FirstOrDefault(x=>x.ConfigId== "default");
        if (result.Config != null)
        {
            result.ConfigId = result.Config.ConfigId;
            return true;
        }
        foreach (ConfiguredDashboard configuration in result.Configs.Where(rc => !rc.IsDefault))
        {
            result.Config = configuration;
            result.ConfigId = result.Config.ConfigId;
            return true;
        }
        result.ConfigId = "default";
        result.Config = new ConfiguredDashboard(dashboard, result.ConfigId, dashboard.Name);
        await dashboardConfigBackupRestore.Save(result.Config, cancellationToken);
        return true;
    }

    public async Task DeleteDashboardConfigs(string namespaceId, string entityId, string dashboardId,
        CancellationToken cancellationToken)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Dashboard dashboard = entity?.GetDashboard(dashboardId);
        if (dashboard == null) return;
        List<ConfiguredDashboard> configurations = await dashboardConfigBackupRestore.Configurations(dashboard, cancellationToken);
        foreach (ConfiguredDashboard dashboardConfiguration in configurations)
        {
            await dashboardConfigBackupRestore.RemoveConfig(dashboardConfiguration, cancellationToken);
        }
        await filterConfigBackupRestore.RemoveConfigs(namespaceId, entityId, null, null, dashboardId, null, cancellationToken);
        await folderConfigBackupRestore.RemoveConfigs(namespaceId, entityId, null, null, dashboardId, null, cancellationToken);
    }
    
    private static bool CheckAccess(ConfiguredDashboard config, IdentityUser user)
    {
        return config.CheckAccess(user);
    }
}
