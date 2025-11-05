using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;
using static Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;
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
