using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Repository.Entities;
using Neo.Domain.Entities.Common;
using static Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public class DashboardConfigBackupRestore(IBpmsSubjectSettingRepository repository)
    : ItemConfigBackupRestore<ConfiguredDashboard>
{
    protected override string SubjectTitle => $"{ConfigType.Dashboard}Config";
    private static string SubjectId(ConfiguredDashboard cr) => SubjectId(cr.Dashboard);
    private static string SubjectId(Dashboard dashboard) => SubjectId(dashboard.NamespaceId, dashboard.EntityId, dashboard.Id);
    private static string SubjectId(string namespaceId, string entityId, string dashboardId) => $"{namespaceId}.{entityId}.{dashboardId}";

    public async Task Save(ConfiguredDashboard config, CancellationToken cancellationToken = default)
    {
        if (config.IsMeta) return;
        config.ConfigId ??= Guid.NewGuid().ToString();
        await repository.SaveAsync(SubjectTitle, SubjectId(config), config.ConfigId, config, cancellationToken);
    }

    public async Task RemoveConfig(ConfiguredDashboard config, CancellationToken cancellationToken = default)
    {
        if (config.IsMeta) return;
        if (config.Id > 0)
            await repository.RemoveAsync(config.Id, cancellationToken);
        else
            await repository.RemoveAsync(SubjectTitle, SubjectId(config), config.ConfigId, cancellationToken);
    }

    public async Task RemoveConfig(string namespaceId, string entityId, string dashboardId, string configId, CancellationToken cancellationToken = default)
    {
        await repository.RemoveAsync(SubjectTitle, SubjectId(namespaceId, entityId, dashboardId), configId, cancellationToken);
    }

    public async Task<ConfiguredDashboard> GetConfig(string configId, CancellationToken cancellationToken = default)
    {
        return await repository.GetByKeyAsync<ConfiguredDashboard>(SubjectTitle, configId, Extract, cancellationToken);
    }

    public async Task<ConfiguredDashboard> GetConfig(Dashboard dashboard, string configId, CancellationToken cancellationToken = default)
    {
        return GetMetaConfig(dashboard, configId) ??
            await repository.GetAsync<ConfiguredDashboard>(
            SubjectTitle, SubjectId(dashboard), configId, (config, setting) => Extract(config, dashboard, setting.Key), cancellationToken);
    }

    public async Task<List<ConfiguredDashboard>> Configurations(Dashboard dashboard, CancellationToken cancellationToken = default)
    {
        List<ConfiguredDashboard> configs = dashboard.MetaConfigures?.Values.ToList() ?? [];
        configs.AddRange(await repository.GetAllConfigsAsync<ConfiguredDashboard>(
            SubjectTitle, SubjectId(dashboard), (config, setting) => Extract(config, dashboard, setting.Key), cancellationToken));
        return configs;
    }

    private static ConfiguredDashboard Extract(ConfiguredDashboard config, SubjectSetting subjectSetting)
    {
        var split = subjectSetting.SubjectId.Split(".");
        var dashboard = ProjectDefinition.Project.GetUiEntity(split[0], split[1])?.GetDashboard(split[2]);
        return Extract(config, dashboard, subjectSetting.Key);
    }

    private static ConfiguredDashboard Extract(ConfiguredDashboard config, Dashboard dashboard, string configId)
    {
        config.ConfigId = configId;
        config.Dashboard = dashboard;
        foreach (var item in config?.Divs ?? [])
        {
            NormalizeDiv(config, item);
        }
        return config;
    }

    private static void NormalizeDiv(ConfiguredDashboard config, ConfigDiv div)
    {
        div.Widget = config.Widgets?.FirstOrDefault(x => x.Id == div.WidgetId);
        foreach (var item in div?.Children ?? [])
        {
            item.Parent = div;
            NormalizeDiv(config, item);
        }
    }
    private static ConfiguredDashboard GetMetaConfig(Dashboard dashboard, string configId)
    {
        return dashboard.MetaConfigures?.TryGetValue(configId, out var config) ?? false && config != null
            ? config : null;
    }
}
