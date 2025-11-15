using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

namespace Neo.Bpms.UI.MVC.Models.Dashboard
{
    public class DashboardWidgetRenderModel
    {
        public DashboardData DashboardData { get; set; }

        public ConfiguredDashboard.ConfigDiv DivConfig { get; set; }

        public ConfiguredDashboard.ConfigWidget Widget { get; set; }

        public bool CanDesign { get; set; }

        public ReportData ReportData { get; set; }

        public int WidgetHeight { get; set; }

        public int MaxRecord { get; set; }
    }
}
