using System;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

namespace Neo.Bpms.UI.MVC.Models.Dashboard
{
    public class DashboardDivRenderModel
    {
        public DashboardDivRenderModel(DashboardData dashboardData, ConfiguredDashboard.ConfigDiv divConfig, bool canDesign)
        {
            DashboardData = dashboardData ?? throw new ArgumentNullException(nameof(dashboardData));
            DivConfig = divConfig ?? throw new ArgumentNullException(nameof(divConfig));
            CanDesign = canDesign;
        }

        public DashboardData DashboardData { get; }

        public ConfiguredDashboard.ConfigDiv DivConfig { get; }

        public bool CanDesign { get; }
    }
}
