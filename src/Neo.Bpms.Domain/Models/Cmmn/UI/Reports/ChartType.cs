namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public partial class Report
{
    public enum ChartType
    {
        None = 0,
        Area,
        Bar,
        Column,
        Line,
        Pie,
        Stock,
        Spline,
        Areaspline,
        Scatter,
        Polar,
        Angular,
        Range,
        WorldMap,
        Treemap,
        Gauge,
        MetricBox,
        IranMap,

        BpmnDiagram
    }
}
