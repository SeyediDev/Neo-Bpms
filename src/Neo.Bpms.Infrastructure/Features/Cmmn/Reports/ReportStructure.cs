using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportStructure : CommonFormStructure
{
    public ReportViewType ReportViewType { get; set; }
    public ChartType ChartType { get; set; }
    public GroupByViewType GroupByViewType { get; set; }

    public IList<ReportColumnFilter> SelectedFilters { get; set; }

    //		public IList<ReportSelectedGroupBy> ReportSelectedGroupBys{ get; set; }
    public IList<ReportOrderInfo> OrderInfos { get; set; } = [];

    public string HavingConstraint { get; set; }
    public string Constraint { get; set; }
    public ValueRange ValueRange { get; set; }
    public List<ValueRange> Levels { get; set; }

    public IList<ColumnFieldDefinition> SelectedColumns { get; set; } = [];
    public IEnumerable<ColumnFieldDefinition> Columns => SelectedColumns.Where(c => c.aggrType != eAggregationFunctions.GroupByItem);
    public IList<PostedProperty> Properties { get; set; }

    public bool HasAnyDrillDown =>
        SubReports != null && SubReports.Any(sr => sr.Type == Report.SubReportType.DrillDown);

    public List<ConfiguredReport.ConfiguredSubReport> SubReports { get; set; }
        = [];

    public List<ConfiguredReport> Configs { get; set; } = [];

    public List<ConfigTreeItem> ScheduledReportsTree
    {
        get
        {
            List<ConfigTreeItem> result =
            [
                new() 
                {
                    id = "thisConfig",
                    parent = "#",
                    text = CulturalTexts.ThisConfigSchedules,
                    type = "default"
                },
                new() 
                {
                    id = "otherConfig",
                    parent = "#",
                    text = CulturalTexts.OtherConfigsSchedules,
                    type = "default"
                },
                .. Configs.SelectMany(c => c.ScheduledReports)
                    .OrderBy(f => f.ReportConfigId.ToString() + f.Name)
                    .Select(c => new ConfigTreeItem
                    {
                        id = c.Id.ToString(),
                        data = new
                        {
                            configId = c.ReportConfigId?.ToString() ?? ""
                        },
                        parent = c.ReportConfigId == ConfigId ? "thisConfig" : "otherConfig",
                        text = c.Name,
                        type = "ScheduledReport"
                    }),
            ];
            return result;
        }
    }

    public string GetReportProperty(long id)
    {
        PostedProperty property = Properties?.FirstOrDefault(p => p.PropertyId == id);
        return property?.Value ?? GetDefaultPropertyValue(id);
    }

    private string GetDefaultPropertyValue(long id)
    {
        return id switch
        {
            (long)ReportConfigProperty.BackgroundColor => "#17a2b8",//If it got changed, tetametric.js default value should also change
            (long)ReportConfigProperty.TextColor => "#ffffff",//If it got changed, tetametric.js default value should also change
            _ => "",
        };
    }

    public string[] GetReportPropertyList(long id)
    {
        string[] property = Properties?.Where(p => p.PropertyId == id).Select(p2 => p2.Value).ToArray();
        return property;
    }

    /// <summary>
    /// Normalizes ChartType for tree type naming - same logic as ReportStructRoutines.GetChartType
    /// </summary>
    private static ChartType GetChartType(ChartType chartType)
    {
        return chartType switch
        {
            ChartType.IranMap or ChartType.WorldMap or ChartType.Treemap => ChartType.Treemap,
            ChartType.BpmnDiagram => ChartType.BpmnDiagram,
            ChartType.MetricBox => ChartType.MetricBox,
            ChartType.Gauge => ChartType.Gauge,
            _ => ChartType.Column
        };
    }
}
