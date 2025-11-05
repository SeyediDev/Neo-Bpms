using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Dashboards;

public abstract class DashboardConfigDefinition : BaseModelingDefinition
{
    protected abstract string Title { get; }
    protected virtual List<string>? Roles { get; }
    protected virtual bool IsDefault { get; }

    /// <summary>
    /// Define All Dashboard Configuration
    /// </summary>
    /// <param name="dashboard">dashboard</param>
    /// <returns></returns>
    public void Define(Dashboard dashboard)
    {
        ConfiguredDashboard  dashboardConfig = new()
        {
            Roles = Roles,
            ConfigId = GetType().Name,
            Name = Title,
            IsDefault = IsDefault,
            Id = 0,
            UserId = null,
            IsPublic = true,
            Dashboard = dashboard,
            FolderId = null,
            Divs = [],
            Widgets = [],
            IsMeta = true
        };
        dashboard.MetaConfigures ??= [];
        dashboard.MetaConfigures.Add(dashboardConfig.ConfigId, dashboardConfig);
        foreach (var configDefinition in ExtractSubsInstances<DashboardDivDefinition>())
        {
            configDefinition?.Define(dashboardConfig, null);
        }
        foreach (var configDefinition in ExtractSubsInstances<DashboardWidgetDefinition>())
        {
            configDefinition?.Define(dashboardConfig, null);
        }
        foreach (var configDefinition in ExtractSubsInstances<DashboardDivWidgetDefinition>())
        {
            configDefinition?.Define(dashboardConfig, null);
        }
    }
}
