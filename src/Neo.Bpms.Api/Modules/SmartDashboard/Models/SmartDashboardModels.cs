namespace Neo.Bpms.Api.Modules.SmartDashboard.Models;

/// <summary>
/// Represents the type of metric being analyzed
/// </summary>
public enum MetricType
{
    /// <summary>Cumulative counter (always increasing)</summary>
    Counter,
    
    /// <summary>Point-in-time measurement</summary>
    Gauge,
    
    /// <summary>Distribution of values (latency, size, etc.)</summary>
    Histogram,
    
    /// <summary>Rate of change per time unit</summary>
    Rate,
    
    /// <summary>Boolean status (up/down, healthy/unhealthy)</summary>
    Status,
    
    /// <summary>Unknown or unclassified</summary>
    Unknown
}

/// <summary>
/// Represents the unit category of a metric
/// </summary>
public enum MetricUnitCategory
{
    /// <summary>Percentage values (0-100)</summary>
    Percentage,
    
    /// <summary>Time duration (ms, s, m, h)</summary>
    Duration,
    
    /// <summary>Data size (bytes, KB, MB, GB)</summary>
    DataSize,
    
    /// <summary>Simple count</summary>
    Count,
    
    /// <summary>Currency value</summary>
    Currency,
    
    /// <summary>Rate per time unit (req/s, ops/min)</summary>
    RatePerTime,
    
    /// <summary>Temperature</summary>
    Temperature,
    
    /// <summary>No unit or unknown</summary>
    None
}

/// <summary>
/// Types of widgets that can be recommended
/// </summary>
public enum WidgetType
{
    /// <summary>Circular gauge with percentage</summary>
    GaugeChart,
    
    /// <summary>Simple number with optional trend</summary>
    NumberCard,
    
    /// <summary>Line chart over time</summary>
    TimeSeriesChart,
    
    /// <summary>Small inline chart</summary>
    SparklineChart,
    
    /// <summary>Distribution histogram</summary>
    HistogramChart,
    
    /// <summary>Green/Red status indicator</summary>
    StatusIndicator,
    
    /// <summary>Bar chart comparison</summary>
    BarChart,
    
    /// <summary>Pie/Donut chart for proportions</summary>
    PieChart,
    
    /// <summary>Data table for detailed view</summary>
    DataTable,
    
    /// <summary>Heatmap for patterns</summary>
    Heatmap
}

/// <summary>
/// Widget size in the dashboard grid
/// </summary>
public enum WidgetSize
{
    /// <summary>1x1 - Small card</summary>
    Small,
    
    /// <summary>2x1 - Medium card</summary>
    Medium,
    
    /// <summary>2x2 - Large card</summary>
    Large,
    
    /// <summary>4x2 - Extra large / Full width</summary>
    ExtraLarge
}

/// <summary>
/// Status severity levels for thresholds
/// </summary>
public enum StatusSeverity
{
    Normal,
    Warning,
    Critical
}

/// <summary>
/// Analyzed metric information
/// </summary>
public class AnalyzedMetric
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MetricType Type { get; set; }
    public MetricUnitCategory UnitCategory { get; set; }
    public string? Unit { get; set; }
    public double? CurrentValue { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public double? AvgValue { get; set; }
    public Dictionary<string, string> Tags { get; set; } = [];
    public string? Category { get; set; }
    public int Priority { get; set; } = 50; // 0-100, higher = more important
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Widget recommendation from the recommender
/// </summary>
public class WidgetRecommendation
{
    public string MetricName { get; set; } = string.Empty;
    public WidgetType RecommendedWidget { get; set; }
    public WidgetSize Size { get; set; }
    public double Confidence { get; set; } // 0-1
    public string Reason { get; set; } = string.Empty;
    public WidgetConfiguration Configuration { get; set; } = new();
    public List<WidgetType> AlternativeWidgets { get; set; } = [];
}

/// <summary>
/// Widget configuration settings
/// </summary>
public class WidgetConfiguration
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Unit { get; set; }
    public string? Format { get; set; }
    public string PrimaryColor { get; set; } = "#0EA5E9";
    public bool ShowTrend { get; set; }
    public bool ShowSparkline { get; set; }
    public int? RefreshIntervalSeconds { get; set; }
    public ThresholdConfiguration? Thresholds { get; set; }
    public ChartConfiguration? ChartConfig { get; set; }
}

/// <summary>
/// Threshold configuration for status coloring
/// </summary>
public class ThresholdConfiguration
{
    public double? WarningThreshold { get; set; }
    public double? CriticalThreshold { get; set; }
    public bool InvertThresholds { get; set; } // true = lower is worse
    public string NormalColor { get; set; } = "#10B981"; // Green
    public string WarningColor { get; set; } = "#F59E0B"; // Yellow
    public string CriticalColor { get; set; } = "#EF4444"; // Red
}

/// <summary>
/// Chart-specific configuration
/// </summary>
public class ChartConfiguration
{
    public string TimeRange { get; set; } = "1h";
    public int DataPoints { get; set; } = 60;
    public bool ShowGrid { get; set; } = true;
    public bool ShowLegend { get; set; }
    public bool Stacked { get; set; }
    public string? YAxisLabel { get; set; }
}

/// <summary>
/// Position in dashboard grid
/// </summary>
public class GridPosition
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int RowSpan { get; set; } = 1;
    public int ColSpan { get; set; } = 1;
}

/// <summary>
/// Complete widget definition with position
/// </summary>
public class DashboardWidget
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string MetricName { get; set; } = string.Empty;
    public WidgetType WidgetType { get; set; }
    public WidgetSize Size { get; set; }
    public GridPosition Position { get; set; } = new();
    public WidgetConfiguration Configuration { get; set; } = new();
}

/// <summary>
/// Generated dashboard layout
/// </summary>
public class GeneratedDashboard
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int GridColumns { get; set; } = 4;
    public List<DashboardWidget> Widgets { get; set; } = [];
    public List<WidgetGroup> Groups { get; set; } = [];
    public DashboardMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Group of related widgets
/// </summary>
public class WidgetGroup
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public List<string> WidgetIds { get; set; } = [];
    public int Order { get; set; }
}

/// <summary>
/// Dashboard metadata
/// </summary>
public class DashboardMetadata
{
    public int TotalMetrics { get; set; }
    public int TotalWidgets { get; set; }
    public string GenerationStrategy { get; set; } = "smart";
    public double ConfidenceScore { get; set; }
    public List<string> Warnings { get; set; } = [];
}

/// <summary>
/// Dashboard template definition
/// </summary>
public class DashboardTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = "general";
    public bool IsBuiltIn { get; set; }
    public List<TemplateWidgetSlot> Slots { get; set; } = [];
}

/// <summary>
/// Template widget slot - defines expected widget placement
/// </summary>
public class TemplateWidgetSlot
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public GridPosition Position { get; set; } = new();
    public WidgetSize PreferredSize { get; set; }
    public List<WidgetType> AllowedWidgetTypes { get; set; } = [];
    public List<MetricType> PreferredMetricTypes { get; set; } = [];
    public string? MetricNamePattern { get; set; }
}

/// <summary>
/// User's saved dashboard configuration
/// </summary>
public class UserDashboardConfig
{
    public int UserId { get; set; }
    public string DashboardId { get; set; } = string.Empty;
    public string? TemplateId { get; set; }
    public List<DashboardWidget> CustomWidgets { get; set; } = [];
    public Dictionary<string, object> Preferences { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request to generate a smart dashboard
/// </summary>
public class GenerateDashboardRequest
{
    public string? TemplateId { get; set; }
    public string? Name { get; set; }
    public List<string>? MetricFilters { get; set; }
    public string? Category { get; set; }
    public int MaxWidgets { get; set; } = 12;
    public bool IncludeSystemMetrics { get; set; } = true;
    public bool IncludeApplicationMetrics { get; set; } = true;
}

