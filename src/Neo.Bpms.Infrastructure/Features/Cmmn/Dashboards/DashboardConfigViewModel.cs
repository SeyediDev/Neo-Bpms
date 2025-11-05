using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class DashboardConfigViewModel : DashboardStructure
{
    public List<ReportViewModel> Reports { get; set; }
    public class ReportViewModel
    {
        public string Id { get; set; }
        public string NamespaceId { get; set; }
        public string EntityId { get; set; }
        public string Name { get; set; }
        public List<ConfigViewModel> Configs { get; set; }
        public class ConfigViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public ReportViewType viewType { get; set; }
            public Report.ChartType ChartType { get; set; }
        }
    }
}
