using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Dashboards;

public abstract class KpiWidgetBase<TEntity, TReport, TReportConfig> : DashboardDivWidgetDefinition<TEntity, TReport, TReportConfig>
    where TEntity : class
    where TReport : ReportDefinition
    where TReportConfig : ReportConfigDefinition
{
    protected override int? MaxRecordCount => 1;
    protected override int? HeightInPixels => 96;
    public override int Width => 3;
    protected virtual string IconClass => "fal fa-info-circle";
    protected virtual string IconId => "chart-bar";
    protected override string Icon => IconId;

    protected KpiWidgetBase()
    {
        Properties =
        [
            new ConfigWidgetProperty
                        {
                            Id = eControlPropertyId.IconClass,
                            Value = IconClass
                        }
        ];
    }
}
