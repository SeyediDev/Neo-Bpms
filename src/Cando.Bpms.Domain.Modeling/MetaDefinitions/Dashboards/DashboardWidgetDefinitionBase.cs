using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;
using static Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;

public abstract class DashboardWidgetDefinitionBase
    : BaseModelingDefinition
{
    protected virtual int? HeightInPixels { get; set; } = 200;
    protected virtual int? MaxRecordCount { get; set; } = 10;

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
            ReportConfigId = reportConfigId
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
