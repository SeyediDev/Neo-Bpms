using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Main service for generating smart dashboards
/// </summary>
public class SmartDashboardGenerator : ISmartDashboardGenerator
{
    private readonly IMetricAnalyzer _metricAnalyzer;
    private readonly IWidgetRecommender _widgetRecommender;
    private readonly ILayoutEngine _layoutEngine;

    // Built-in templates
    private static readonly List<DashboardTemplate> BuiltInTemplates =
    [
        new DashboardTemplate
        {
            Id = "system-overview",
            Name = "نمای کلی سیستم",
            Description = "داشبورد نظارت بر منابع سیستم شامل CPU، حافظه و دیسک",
            Category = "system",
            IsBuiltIn = true,
            Slots =
            [
                new() { Id = "cpu", Label = "CPU", Position = new() { Row = 0, Column = 0, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.GaugeChart], MetricNamePattern = "cpu" },
                new() { Id = "memory", Label = "حافظه", Position = new() { Row = 0, Column = 1, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.GaugeChart], MetricNamePattern = "memory" },
                new() { Id = "requests", Label = "درخواست‌ها", Position = new() { Row = 0, Column = 2, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], MetricNamePattern = "request" },
                new() { Id = "errors", Label = "خطاها", Position = new() { Row = 0, Column = 3, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], MetricNamePattern = "error" },
                new() { Id = "response-time", Label = "زمان پاسخ", Position = new() { Row = 1, Column = 0, RowSpan = 1, ColSpan = 2 }, PreferredSize = WidgetSize.Medium, AllowedWidgetTypes = [WidgetType.TimeSeriesChart, WidgetType.SparklineChart], MetricNamePattern = "response" },
                new() { Id = "throughput", Label = "نرخ پردازش", Position = new() { Row = 1, Column = 2, RowSpan = 1, ColSpan = 2 }, PreferredSize = WidgetSize.Medium, AllowedWidgetTypes = [WidgetType.TimeSeriesChart, WidgetType.SparklineChart], MetricNamePattern = "throughput" },
            ]
        },
        new DashboardTemplate
        {
            Id = "application-health",
            Name = "سلامت برنامه",
            Description = "نظارت بر وضعیت سلامت و عملکرد برنامه",
            Category = "application",
            IsBuiltIn = true,
            Slots =
            [
                new() { Id = "health", Label = "وضعیت سلامت", Position = new() { Row = 0, Column = 0, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.StatusIndicator], MetricNamePattern = "health" },
                new() { Id = "uptime", Label = "آپتایم", Position = new() { Row = 0, Column = 1, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], MetricNamePattern = "uptime" },
                new() { Id = "gc", Label = "GC", Position = new() { Row = 0, Column = 2, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard, WidgetType.BarChart], MetricNamePattern = "gc" },
                new() { Id = "threads", Label = "تردها", Position = new() { Row = 0, Column = 3, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], MetricNamePattern = "thread" },
            ]
        },
        new DashboardTemplate
        {
            Id = "web-performance",
            Name = "عملکرد وب",
            Description = "نظارت بر عملکرد درخواست‌های HTTP و API",
            Category = "web",
            IsBuiltIn = true,
            Slots =
            [
                new() { Id = "requests-per-sec", Label = "درخواست/ثانیه", Position = new() { Row = 0, Column = 0, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard, WidgetType.SparklineChart], PreferredMetricTypes = [MetricType.Rate] },
                new() { Id = "success-rate", Label = "نرخ موفقیت", Position = new() { Row = 0, Column = 1, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.GaugeChart], MetricNamePattern = "success" },
                new() { Id = "avg-response", Label = "میانگین پاسخ", Position = new() { Row = 0, Column = 2, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], PreferredMetricTypes = [MetricType.Histogram] },
                new() { Id = "active-connections", Label = "اتصالات فعال", Position = new() { Row = 0, Column = 3, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], MetricNamePattern = "connection" },
                new() { Id = "latency-chart", Label = "نمودار تاخیر", Position = new() { Row = 1, Column = 0, RowSpan = 2, ColSpan = 4 }, PreferredSize = WidgetSize.ExtraLarge, AllowedWidgetTypes = [WidgetType.TimeSeriesChart], MetricNamePattern = "latency" },
            ]
        },
        new DashboardTemplate
        {
            Id = "minimal",
            Name = "حداقلی",
            Description = "داشبورد ساده با مهم‌ترین متریک‌ها",
            Category = "general",
            IsBuiltIn = true,
            Slots =
            [
                new() { Id = "metric1", Label = "متریک ۱", Position = new() { Row = 0, Column = 0, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.GaugeChart, WidgetType.NumberCard], PreferredMetricTypes = [MetricType.Gauge] },
                new() { Id = "metric2", Label = "متریک ۲", Position = new() { Row = 0, Column = 1, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.GaugeChart, WidgetType.NumberCard], PreferredMetricTypes = [MetricType.Gauge] },
                new() { Id = "metric3", Label = "متریک ۳", Position = new() { Row = 0, Column = 2, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], PreferredMetricTypes = [MetricType.Counter] },
                new() { Id = "metric4", Label = "متریک ۴", Position = new() { Row = 0, Column = 3, RowSpan = 1, ColSpan = 1 }, PreferredSize = WidgetSize.Small, AllowedWidgetTypes = [WidgetType.NumberCard], PreferredMetricTypes = [MetricType.Counter] },
            ]
        }
    ];

    public SmartDashboardGenerator(
        IMetricAnalyzer metricAnalyzer,
        IWidgetRecommender widgetRecommender,
        ILayoutEngine layoutEngine)
    {
        _metricAnalyzer = metricAnalyzer;
        _widgetRecommender = widgetRecommender;
        _layoutEngine = layoutEngine;
    }

    public async Task<GeneratedDashboard> GenerateDashboardAsync(
        GenerateDashboardRequest request, 
        CancellationToken cancellationToken = default)
    {
        return await GenerateDashboardInternalAsync(request, cancellationToken);
    }

    public async Task<GeneratedDashboard> PreviewDashboardAsync(
        GenerateDashboardRequest request, 
        CancellationToken cancellationToken = default)
    {
        return await GenerateDashboardInternalAsync(request, cancellationToken);
    }

    private async Task<GeneratedDashboard> GenerateDashboardInternalAsync(
        GenerateDashboardRequest request,
        CancellationToken cancellationToken)
    {
        // Step 1: Analyze all metrics
        var allMetrics = await _metricAnalyzer.AnalyzeAllMetricsAsync(cancellationToken);
        var metricsList = allMetrics.ToList();

        // Step 2: Filter metrics based on request
        var filteredMetrics = FilterMetrics(metricsList, request);

        // Step 3: Limit number of metrics
        var limitedMetrics = filteredMetrics
            .OrderByDescending(m => m.Priority)
            .Take(request.MaxWidgets)
            .ToList();

        // Step 4: Get widget recommendations
        var recommendations = _widgetRecommender.RecommendWidgets(limitedMetrics).ToList();

        // Step 5: Arrange widgets
        List<DashboardWidget> widgets;
        
        if (!string.IsNullOrEmpty(request.TemplateId))
        {
            var template = GetTemplate(request.TemplateId);
            if (template != null)
            {
                widgets = _layoutEngine.ApplyTemplate(template, recommendations, limitedMetrics);
            }
            else
            {
                widgets = _layoutEngine.ArrangeWidgets(recommendations);
            }
        }
        else
        {
            widgets = _layoutEngine.ArrangeWidgets(recommendations);
        }

        // Step 6: Group widgets
        var groups = _layoutEngine.GroupWidgets(widgets, limitedMetrics);

        // Step 7: Calculate confidence score
        var avgConfidence = recommendations.Any() 
            ? recommendations.Average(r => r.Confidence) 
            : 0;

        // Step 8: Build dashboard
        var dashboard = new GeneratedDashboard
        {
            Name = request.Name ?? "داشبورد هوشمند",
            Description = $"تولید شده در {DateTime.UtcNow:yyyy/MM/dd HH:mm}",
            GeneratedAt = DateTime.UtcNow,
            GridColumns = 4,
            Widgets = widgets,
            Groups = groups,
            Metadata = new DashboardMetadata
            {
                TotalMetrics = metricsList.Count,
                TotalWidgets = widgets.Count,
                GenerationStrategy = string.IsNullOrEmpty(request.TemplateId) ? "smart" : $"template:{request.TemplateId}",
                ConfidenceScore = avgConfidence,
                Warnings = GenerateWarnings(metricsList, widgets, request)
            }
        };

        return dashboard;
    }

    private IEnumerable<AnalyzedMetric> FilterMetrics(List<AnalyzedMetric> metrics, GenerateDashboardRequest request)
    {
        var filtered = metrics.AsEnumerable();

        // Filter by name patterns
        if (request.MetricFilters != null && request.MetricFilters.Any())
        {
            filtered = filtered.Where(m => 
                request.MetricFilters.Any(f => 
                    m.Name.Contains(f, StringComparison.OrdinalIgnoreCase)));
        }

        // Filter by category
        if (!string.IsNullOrEmpty(request.Category))
        {
            filtered = filtered.Where(m => 
                m.Category?.Equals(request.Category, StringComparison.OrdinalIgnoreCase) == true);
        }

        // Filter system metrics
        if (!request.IncludeSystemMetrics)
        {
            filtered = filtered.Where(m => 
                !m.Name.StartsWith("system.", StringComparison.OrdinalIgnoreCase) &&
                !m.Name.StartsWith("process.", StringComparison.OrdinalIgnoreCase));
        }

        // Filter application metrics
        if (!request.IncludeApplicationMetrics)
        {
            filtered = filtered.Where(m => 
                !m.Name.StartsWith("app.", StringComparison.OrdinalIgnoreCase) &&
                !m.Name.StartsWith("application.", StringComparison.OrdinalIgnoreCase));
        }

        return filtered;
    }

    private List<string> GenerateWarnings(
        List<AnalyzedMetric> allMetrics, 
        List<DashboardWidget> widgets, 
        GenerateDashboardRequest request)
    {
        var warnings = new List<string>();

        if (allMetrics.Count == 0)
        {
            warnings.Add("هیچ متریکی یافت نشد. مطمئن شوید که OpenTelemetry به درستی پیکربندی شده است.");
        }
        else if (widgets.Count == 0)
        {
            warnings.Add("هیچ ویجتی ایجاد نشد. فیلترهای انتخاب شده را بررسی کنید.");
        }
        else if (widgets.Count < request.MaxWidgets && allMetrics.Count > widgets.Count)
        {
            warnings.Add($"برخی متریک‌ها ({allMetrics.Count - widgets.Count}) به دلیل فیلترها نادیده گرفته شدند.");
        }

        if (!request.IncludeSystemMetrics)
        {
            warnings.Add("متریک‌های سیستم غیرفعال است.");
        }

        if (!request.IncludeApplicationMetrics)
        {
            warnings.Add("متریک‌های برنامه غیرفعال است.");
        }

        return warnings;
    }

    public IEnumerable<DashboardTemplate> GetTemplates()
    {
        return BuiltInTemplates;
    }

    public DashboardTemplate? GetTemplate(string templateId)
    {
        return BuiltInTemplates.FirstOrDefault(t => 
            t.Id.Equals(templateId, StringComparison.OrdinalIgnoreCase));
    }
}

