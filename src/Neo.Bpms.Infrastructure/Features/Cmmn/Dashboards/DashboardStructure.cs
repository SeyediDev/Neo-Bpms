using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class DashboardStructure : CommonFormStructure
{
    public ConfiguredDashboard ActiveConfig { get; set; }
    public List<ConfiguredDashboard> Configs { get; set; } = [];
}
