using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Dashboards;

public abstract class DashboardWidgetDefinitionBase
    : BaseModelingDefinition
{
    protected virtual int? HeightInPixels { get; } = 200;
    protected virtual int? MaxRecordCount { get; } = 10;
    protected virtual string Icon => "chart-bar";

    /// <summary>
    /// Widget Properties
    /// </summary>
    public List<ConfigWidgetProperty> Properties { get; set; } = [];
    public abstract ConfigWidget Define(ConfiguredDashboard dashboardConfig, ConfigDiv div);
    protected ConfigWidget AddWidget<TEntity, TReport, TReportConfig>(ConfiguredDashboard dashboardConfig, ConfigDiv div)
        where TEntity : class
        where TReport : ReportDefinition
        where TReportConfig : ReportConfigDefinition
    {
        string reportNamespaceId = EntityDefinition.GetNamespaceName<TEntity>();
        string reportEntityId = EntityDefinition.GetEntityName<TEntity>();
        string reportId = typeof(TReport).Name;
        string reportConfigId = typeof(TReportConfig).Name;

        ConfigWidget widget = new()
        {
            Id = GetType().Name,
            Properties = Properties,
            ReportNamespaceId = reportNamespaceId,
            ReportEntityId = reportEntityId,
            ReportId = reportId,
            ReportConfigId = reportConfigId,
            Icon = Icon
        };
        div.Widget = widget;
        div.WidgetId = widget.Id;
        dashboardConfig.Widgets.Add(widget);
        if(HeightInPixels!=null)
        {
            widget.AddProperty(eControlPropertyId.HeightInPixels, HeightInPixels.ToString());
        }
        if(MaxRecordCount!=null)
        {
            widget.AddProperty(eControlPropertyId.MaxRecordCount, MaxRecordCount.ToString());
        }

        dashboardConfig.Dashboard.Reports ??= [];
        var report = dashboardConfig.Dashboard.Reports.FirstOrDefault(
            x => x.NamespaceId == reportNamespaceId &&
               x.EntityId == reportEntityId &&
               x.ReportId == reportId);
        if (report == null)
        {
            report = new()
            {
                NamespaceId = reportNamespaceId,
                EntityId = reportEntityId,
                ReportId = reportId
            };
            dashboardConfig.Dashboard.Reports.Add(report);
        }

        return widget;
    }
}
