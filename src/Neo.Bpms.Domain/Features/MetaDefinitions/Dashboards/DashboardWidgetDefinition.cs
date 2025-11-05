using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Dashboards;
public abstract class DashboardWidgetDefinition
    : DashboardWidgetDefinitionBase
{
}
public abstract class DashboardWidgetDefinition<TEntity, TReport, TReportConfig>
    : DashboardWidgetDefinition
    where TEntity : class
    where TReport : ReportDefinition
    where TReportConfig : ReportConfigDefinition
{
    public override ConfigWidget Define(ConfiguredDashboard dashboardConfig, ConfigDiv div)
    {
        ArgumentNullException.ThrowIfNull(div);
        return AddWidget<TEntity, TReport, TReportConfig>(dashboardConfig, div);
    }
}
