using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Rule-based widget recommender
/// </summary>
public class WidgetRecommender : IWidgetRecommender
{
    // Widget selection rules: (MetricType, UnitCategory) -> WidgetType
    private static readonly Dictionary<(MetricType, MetricUnitCategory), WidgetType> PrimaryRules = new()
    {
        // Percentage metrics -> Gauge
        { (MetricType.Gauge, MetricUnitCategory.Percentage), WidgetType.GaugeChart },
        { (MetricType.Counter, MetricUnitCategory.Percentage), WidgetType.GaugeChart },
        
        // Duration metrics -> Time Series
        { (MetricType.Histogram, MetricUnitCategory.Duration), WidgetType.TimeSeriesChart },
        { (MetricType.Gauge, MetricUnitCategory.Duration), WidgetType.TimeSeriesChart },
        
        // Rate metrics -> Sparkline
        { (MetricType.Rate, MetricUnitCategory.RatePerTime), WidgetType.SparklineChart },
        { (MetricType.Rate, MetricUnitCategory.Count), WidgetType.SparklineChart },
        
        // Status metrics -> Status Indicator
        { (MetricType.Status, MetricUnitCategory.None), WidgetType.StatusIndicator },
        { (MetricType.Status, MetricUnitCategory.Count), WidgetType.StatusIndicator },
        
        // Count metrics -> Number Card
        { (MetricType.Counter, MetricUnitCategory.Count), WidgetType.NumberCard },
        { (MetricType.Gauge, MetricUnitCategory.Count), WidgetType.NumberCard },
        
        // Currency metrics -> Number Card with special formatting
        { (MetricType.Counter, MetricUnitCategory.Currency), WidgetType.NumberCard },
        { (MetricType.Gauge, MetricUnitCategory.Currency), WidgetType.NumberCard },
        
        // Data size -> Gauge or Number
        { (MetricType.Gauge, MetricUnitCategory.DataSize), WidgetType.GaugeChart },
        { (MetricType.Counter, MetricUnitCategory.DataSize), WidgetType.NumberCard },
        
        // Histogram without specific unit -> Bar Chart
        { (MetricType.Histogram, MetricUnitCategory.None), WidgetType.HistogramChart },
    };

    // Secondary rules for specific metric name patterns
    private static readonly Dictionary<string, WidgetType> NamePatternRules = new(StringComparer.OrdinalIgnoreCase)
    {
        { "cpu", WidgetType.GaugeChart },
        { "memory", WidgetType.GaugeChart },
        { "disk", WidgetType.GaugeChart },
        { "health", WidgetType.StatusIndicator },
        { "status", WidgetType.StatusIndicator },
        { "uptime", WidgetType.NumberCard },
        { "response_time", WidgetType.TimeSeriesChart },
        { "latency", WidgetType.TimeSeriesChart },
        { "requests_per_second", WidgetType.SparklineChart },
        { "error_rate", WidgetType.GaugeChart },
        { "success_rate", WidgetType.GaugeChart },
        { "active_connections", WidgetType.NumberCard },
        { "queue_length", WidgetType.NumberCard },
        { "gc_", WidgetType.BarChart },
    };

    public WidgetRecommendation RecommendWidget(AnalyzedMetric metric)
    {
        var (widgetType, confidence, reason) = DetermineWidgetType(metric);
        var alternatives = GetAlternativeWidgets(metric, widgetType);
        var size = DetermineWidgetSize(widgetType, metric);
        var configuration = GetDefaultConfiguration(widgetType, metric);

        return new WidgetRecommendation
        {
            MetricName = metric.Name,
            RecommendedWidget = widgetType,
            Size = size,
            Confidence = confidence,
            Reason = reason,
            Configuration = configuration,
            AlternativeWidgets = alternatives
        };
    }

    public IEnumerable<WidgetRecommendation> RecommendWidgets(IEnumerable<AnalyzedMetric> metrics)
    {
        return metrics.Select(RecommendWidget);
    }

    private (WidgetType Type, double Confidence, string Reason) DetermineWidgetType(AnalyzedMetric metric)
    {
        // First, check name pattern rules (highest confidence)
        foreach (var pattern in NamePatternRules)
        {
            if (metric.Name.Contains(pattern.Key, StringComparison.OrdinalIgnoreCase))
            {
                return (pattern.Value, 0.95, $"مطابقت با الگوی نام: {pattern.Key}");
            }
        }

        // Check primary type+unit rules
        var key = (metric.Type, metric.UnitCategory);
        if (PrimaryRules.TryGetValue(key, out var widgetType))
        {
            return (widgetType, 0.85, $"بر اساس نوع متریک ({metric.Type}) و واحد ({metric.UnitCategory})");
        }

        // Fallback based on type only
        var fallbackType = metric.Type switch
        {
            MetricType.Gauge => WidgetType.GaugeChart,
            MetricType.Counter => WidgetType.NumberCard,
            MetricType.Histogram => WidgetType.TimeSeriesChart,
            MetricType.Rate => WidgetType.SparklineChart,
            MetricType.Status => WidgetType.StatusIndicator,
            _ => WidgetType.NumberCard
        };

        return (fallbackType, 0.6, "پیش‌فرض بر اساس نوع متریک");
    }

    private List<WidgetType> GetAlternativeWidgets(AnalyzedMetric metric, WidgetType primary)
    {
        var alternatives = new List<WidgetType>();

        // Always add NumberCard as alternative (works for everything)
        if (primary != WidgetType.NumberCard)
            alternatives.Add(WidgetType.NumberCard);

        // Type-specific alternatives
        switch (metric.Type)
        {
            case MetricType.Gauge:
                if (primary != WidgetType.GaugeChart) alternatives.Add(WidgetType.GaugeChart);
                if (primary != WidgetType.TimeSeriesChart) alternatives.Add(WidgetType.TimeSeriesChart);
                break;
            
            case MetricType.Counter:
                if (primary != WidgetType.SparklineChart) alternatives.Add(WidgetType.SparklineChart);
                if (primary != WidgetType.BarChart) alternatives.Add(WidgetType.BarChart);
                break;
            
            case MetricType.Histogram:
                if (primary != WidgetType.HistogramChart) alternatives.Add(WidgetType.HistogramChart);
                if (primary != WidgetType.BarChart) alternatives.Add(WidgetType.BarChart);
                break;
            
            case MetricType.Rate:
                if (primary != WidgetType.TimeSeriesChart) alternatives.Add(WidgetType.TimeSeriesChart);
                if (primary != WidgetType.GaugeChart) alternatives.Add(WidgetType.GaugeChart);
                break;
        }

        return alternatives.Take(3).ToList();
    }

    private WidgetSize DetermineWidgetSize(WidgetType widgetType, AnalyzedMetric metric)
    {
        // Size based on widget type
        return widgetType switch
        {
            WidgetType.GaugeChart => WidgetSize.Small,
            WidgetType.NumberCard => WidgetSize.Small,
            WidgetType.StatusIndicator => WidgetSize.Small,
            WidgetType.SparklineChart => WidgetSize.Medium,
            WidgetType.TimeSeriesChart => metric.Priority > 70 ? WidgetSize.Large : WidgetSize.Medium,
            WidgetType.HistogramChart => WidgetSize.Medium,
            WidgetType.BarChart => WidgetSize.Medium,
            WidgetType.PieChart => WidgetSize.Medium,
            WidgetType.DataTable => WidgetSize.Large,
            WidgetType.Heatmap => WidgetSize.ExtraLarge,
            _ => WidgetSize.Small
        };
    }

    public WidgetConfiguration GetDefaultConfiguration(WidgetType widgetType, AnalyzedMetric metric)
    {
        var config = new WidgetConfiguration
        {
            Title = metric.DisplayName,
            Unit = metric.Unit,
            ShowTrend = widgetType == WidgetType.NumberCard,
            ShowSparkline = widgetType == WidgetType.NumberCard && metric.Type == MetricType.Counter,
            RefreshIntervalSeconds = GetDefaultRefreshInterval(metric),
            Thresholds = GetDefaultThresholds(metric),
        };

        // Format based on unit category
        config.Format = metric.UnitCategory switch
        {
            MetricUnitCategory.Percentage => "0.0",
            MetricUnitCategory.Duration => "0.00",
            MetricUnitCategory.Currency => "#,##0",
            MetricUnitCategory.DataSize => "auto",
            MetricUnitCategory.Count => "#,##0",
            _ => "0.##"
        };

        // Color based on category
        config.PrimaryColor = GetColorForCategory(metric.Category);

        // Chart configuration for chart widgets
        if (widgetType is WidgetType.TimeSeriesChart or WidgetType.SparklineChart 
            or WidgetType.BarChart or WidgetType.HistogramChart)
        {
            config.ChartConfig = new ChartConfiguration
            {
                TimeRange = metric.Priority > 70 ? "15m" : "1h",
                DataPoints = widgetType == WidgetType.SparklineChart ? 20 : 60,
                ShowGrid = widgetType != WidgetType.SparklineChart,
                YAxisLabel = metric.Unit
            };
        }

        return config;
    }

    public ThresholdConfiguration? GetDefaultThresholds(AnalyzedMetric metric)
    {
        // Only for percentage or known threshold-able metrics
        if (metric.UnitCategory != MetricUnitCategory.Percentage)
        {
            // Check for error/success rate patterns
            var lowerName = metric.Name.ToLowerInvariant();
            if (!lowerName.Contains("rate") && !lowerName.Contains("error") && !lowerName.Contains("success"))
                return null;
        }

        var thresholds = new ThresholdConfiguration();
        var lowerName = metric.Name.ToLowerInvariant();

        // CPU/Memory - high is bad
        if (lowerName.Contains("cpu") || lowerName.Contains("memory") || lowerName.Contains("disk"))
        {
            thresholds.WarningThreshold = 70;
            thresholds.CriticalThreshold = 90;
            thresholds.InvertThresholds = false;
        }
        // Error rate - any is bad
        else if (lowerName.Contains("error"))
        {
            thresholds.WarningThreshold = 1;
            thresholds.CriticalThreshold = 5;
            thresholds.InvertThresholds = false;
        }
        // Success rate - low is bad
        else if (lowerName.Contains("success"))
        {
            thresholds.WarningThreshold = 99;
            thresholds.CriticalThreshold = 95;
            thresholds.InvertThresholds = true;
        }
        // Generic percentage
        else
        {
            thresholds.WarningThreshold = 80;
            thresholds.CriticalThreshold = 95;
            thresholds.InvertThresholds = false;
        }

        return thresholds;
    }

    private int GetDefaultRefreshInterval(AnalyzedMetric metric)
    {
        // High priority metrics refresh faster
        if (metric.Priority > 80) return 5;
        if (metric.Priority > 60) return 10;
        if (metric.Priority > 40) return 30;
        return 60;
    }

    private string GetColorForCategory(string? category)
    {
        return category switch
        {
            "سیستم" => "#0EA5E9", // Sky blue
            "شبکه" => "#8B5CF6", // Purple
            "وب" => "#10B981", // Emerald
            "پایگاه داده" => "#F59E0B", // Amber
            "کش" => "#EC4899", // Pink
            "صف" => "#6366F1", // Indigo
            "GC" => "#14B8A6", // Teal
            "تردها" => "#F97316", // Orange
            "استخر" => "#84CC16", // Lime
            "اتصالات" => "#06B6D4", // Cyan
            "نشست‌ها" => "#A855F7", // Violet
            "کاربران" => "#22C55E", // Green
            "احراز هویت" => "#EF4444", // Red
            "کسب‌وکار" => "#3B82F6", // Blue
            _ => "#64748B" // Slate
        };
    }
}

