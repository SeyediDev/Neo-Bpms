namespace Neo.Bpms.UI.MVC.Models.Dashboard
{
    public class DashboardDivRenderModel(DashboardData dashboardData, ConfiguredDashboard.ConfigDiv divConfig, bool canDesign)
    {
        public DashboardData DashboardData { get; } = dashboardData ?? throw new ArgumentNullException(nameof(dashboardData));

        public ConfiguredDashboard.ConfigDiv DivConfig { get; } = divConfig ?? throw new ArgumentNullException(nameof(divConfig));

        public bool CanDesign { get; } = canDesign;
    }
}
