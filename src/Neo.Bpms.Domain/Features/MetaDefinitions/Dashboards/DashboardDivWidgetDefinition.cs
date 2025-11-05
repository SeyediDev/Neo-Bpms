using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Dashboards;

public abstract class DashboardDivWidgetDefinition
    : DashboardWidgetDefinitionBase
{
}
public abstract class DashboardDivWidgetDefinition<TEntity, TReport, TReportConfig>
    : DashboardDivWidgetDefinition, IDashboardDivDefinition
    where TEntity : class
    where TReport : ReportDefinition
    where TReportConfig : ReportConfigDefinition
{
    public abstract string Title { get; }
    /// <summary>
    /// Div Width
    /// </summary>
    public virtual int Width { get; } = 6;
    /// <summary>
    /// Is Row
    /// </summary>
    public virtual bool IsRow { get; } = false;

    public override ConfigWidget Define(ConfiguredDashboard dashboardConfig, ConfigDiv parentDiv)
    {
        ConfigDiv div = ((IDashboardDivDefinition)this).AddDiv(dashboardConfig, parentDiv);
        return AddWidget<TEntity, TReport, TReportConfig>(dashboardConfig, div);
    }
}
