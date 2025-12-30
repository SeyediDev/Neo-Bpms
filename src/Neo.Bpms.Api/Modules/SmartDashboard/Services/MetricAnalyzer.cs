using System.Text.RegularExpressions;
using Neo.Bpms.Api.Modules.Monitoring.Services;
using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Analyzes metrics to determine their type, unit, and characteristics
/// </summary>
public class MetricAnalyzer : IMetricAnalyzer
{
    private readonly IMetricsStore _metricsStore;

    // Patterns for metric type detection
    private static readonly Dictionary<string, MetricType> TypePatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        // Counter patterns
        { "total", MetricType.Counter },
        { "count", MetricType.Counter },
        { "requests", MetricType.Counter },
        { "errors", MetricType.Counter },
        { "failures", MetricType.Counter },
        { "successes", MetricType.Counter },
        { "bytes_sent", MetricType.Counter },
        { "bytes_received", MetricType.Counter },
        
        // Gauge patterns
        { "usage", MetricType.Gauge },
        { "percent", MetricType.Gauge },
        { "current", MetricType.Gauge },
        { "active", MetricType.Gauge },
        { "connections", MetricType.Gauge },
        { "threads", MetricType.Gauge },
        { "queue", MetricType.Gauge },
        { "pool", MetricType.Gauge },
        { "memory", MetricType.Gauge },
        { "cpu", MetricType.Gauge },
        
        // Histogram patterns
        { "duration", MetricType.Histogram },
        { "latency", MetricType.Histogram },
        { "time", MetricType.Histogram },
        { "response_time", MetricType.Histogram },
        { "size", MetricType.Histogram },
        
        // Rate patterns
        { "rate", MetricType.Rate },
        { "per_second", MetricType.Rate },
        { "per_minute", MetricType.Rate },
        { "throughput", MetricType.Rate },
        
        // Status patterns
        { "status", MetricType.Status },
        { "health", MetricType.Status },
        { "up", MetricType.Status },
        { "available", MetricType.Status },
    };

    // Patterns for unit category detection
    private static readonly Dictionary<string, MetricUnitCategory> UnitPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        { "%", MetricUnitCategory.Percentage },
        { "percent", MetricUnitCategory.Percentage },
        { "ms", MetricUnitCategory.Duration },
        { "s", MetricUnitCategory.Duration },
        { "seconds", MetricUnitCategory.Duration },
        { "minutes", MetricUnitCategory.Duration },
        { "hours", MetricUnitCategory.Duration },
        { "bytes", MetricUnitCategory.DataSize },
        { "kb", MetricUnitCategory.DataSize },
        { "mb", MetricUnitCategory.DataSize },
        { "gb", MetricUnitCategory.DataSize },
        { "irr", MetricUnitCategory.Currency },
        { "usd", MetricUnitCategory.Currency },
        { "eur", MetricUnitCategory.Currency },
        { "req/s", MetricUnitCategory.RatePerTime },
        { "ops/s", MetricUnitCategory.RatePerTime },
        { "/s", MetricUnitCategory.RatePerTime },
        { "/min", MetricUnitCategory.RatePerTime },
    };

    // High priority metric patterns
    private static readonly HashSet<string> HighPriorityPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        "cpu", "memory", "error", "failure", "health", "status",
        "response_time", "latency", "requests", "active"
    };

    // Category patterns for grouping
    private static readonly Dictionary<string, string> CategoryPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        { "cpu", "سیستم" },
        { "memory", "سیستم" },
        { "disk", "سیستم" },
        { "network", "شبکه" },
        { "http", "وب" },
        { "request", "وب" },
        { "response", "وب" },
        { "database", "پایگاه داده" },
        { "db", "پایگاه داده" },
        { "sql", "پایگاه داده" },
        { "cache", "کش" },
        { "redis", "کش" },
        { "queue", "صف" },
        { "message", "صف" },
        { "gc", "GC" },
        { "thread", "تردها" },
        { "pool", "استخر" },
        { "connection", "اتصالات" },
        { "session", "نشست‌ها" },
        { "user", "کاربران" },
        { "auth", "احراز هویت" },
        { "business", "کسب‌وکار" },
    };

    public MetricAnalyzer(IMetricsStore metricsStore)
    {
        _metricsStore = metricsStore;
    }

    public Task<IEnumerable<AnalyzedMetric>> AnalyzeAllMetricsAsync(CancellationToken cancellationToken = default)
    {
        var metricDefinitions = _metricsStore.GetMetricDefinitions();
        var analyzedMetrics = new List<AnalyzedMetric>();

        foreach (var metricDef in metricDefinitions)
        {
            // Get latest value from time series
            var timeSeries = _metricsStore.GetTimeSeries(metricDef.Name, DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow);
            var latestValue = timeSeries.DataPoints.LastOrDefault()?.Value ?? 0;
            
            var analyzed = AnalyzeMetricInternal(metricDef.Name, metricDef.Unit, latestValue);
            if (analyzed != null)
            {
                analyzedMetrics.Add(analyzed);
            }
        }

        // Sort by priority descending
        IEnumerable<AnalyzedMetric> result = analyzedMetrics.OrderByDescending(m => m.Priority);
        return Task.FromResult(result);
    }

    public Task<AnalyzedMetric?> AnalyzeMetricAsync(string metricName, CancellationToken cancellationToken = default)
    {
        var metricDef = _metricsStore.GetMetricDefinition(metricName);
        
        if (metricDef == null)
            return Task.FromResult<AnalyzedMetric?>(null);

        // Get latest value from time series
        var timeSeries = _metricsStore.GetTimeSeries(metricDef.Name, DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow);
        var latestValue = timeSeries.DataPoints.LastOrDefault()?.Value ?? 0;

        return Task.FromResult<AnalyzedMetric?>(AnalyzeMetricInternal(metricDef.Name, metricDef.Unit, latestValue));
    }

    private AnalyzedMetric AnalyzeMetricInternal(string name, string? unit, double value)
    {
        var type = DetectMetricType(name, unit, new[] { value });
        var unitCategory = DetectUnitCategory(unit);
        var category = CategorizeMetric(name);
        var priority = CalculatePriority(name, type, unitCategory);

        return new AnalyzedMetric
        {
            Name = name,
            DisplayName = GenerateDisplayName(name),
            Type = type,
            UnitCategory = unitCategory,
            Unit = unit,
            CurrentValue = value,
            Category = category,
            Priority = priority,
            LastUpdated = DateTime.UtcNow
        };
    }

    public MetricType DetectMetricType(string metricName, string? unit, IEnumerable<double>? values)
    {
        var normalizedName = metricName.ToLowerInvariant();

        // Check against known patterns
        foreach (var pattern in TypePatterns)
        {
            if (normalizedName.Contains(pattern.Key))
            {
                return pattern.Value;
            }
        }

        // Check unit for hints
        if (!string.IsNullOrEmpty(unit))
        {
            var normalizedUnit = unit.ToLowerInvariant();
            
            if (normalizedUnit.Contains("%") || normalizedUnit.Contains("percent"))
                return MetricType.Gauge;
            
            if (normalizedUnit.Contains("ms") || normalizedUnit.Contains("seconds"))
                return MetricType.Histogram;
            
            if (normalizedUnit.Contains("/s") || normalizedUnit.Contains("per"))
                return MetricType.Rate;
        }

        // Analyze values if available
        if (values != null && values.Any())
        {
            var valueList = values.ToList();
            
            // If all values are 0 or 1, might be status
            if (valueList.All(v => v == 0 || v == 1))
                return MetricType.Status;
            
            // If values only increase, likely counter
            if (valueList.Count > 1)
            {
                var isMonotonic = true;
                for (int i = 1; i < valueList.Count; i++)
                {
                    if (valueList[i] < valueList[i - 1])
                    {
                        isMonotonic = false;
                        break;
                    }
                }
                if (isMonotonic)
                    return MetricType.Counter;
            }
        }

        return MetricType.Gauge; // Default to gauge
    }

    public MetricUnitCategory DetectUnitCategory(string? unit)
    {
        if (string.IsNullOrEmpty(unit))
            return MetricUnitCategory.None;

        var normalizedUnit = unit.ToLowerInvariant().Trim();

        foreach (var pattern in UnitPatterns)
        {
            if (normalizedUnit.Contains(pattern.Key))
            {
                return pattern.Value;
            }
        }

        // Check for count-like patterns
        if (Regex.IsMatch(normalizedUnit, @"^\d*$") || normalizedUnit == "count" || normalizedUnit == "items")
            return MetricUnitCategory.Count;

        return MetricUnitCategory.None;
    }

    public string GenerateDisplayName(string metricName)
    {
        // Replace common separators with spaces
        var displayName = Regex.Replace(metricName, @"[._\-]", " ");
        
        // Split camelCase
        displayName = Regex.Replace(displayName, @"([a-z])([A-Z])", "$1 $2");
        
        // Capitalize each word
        displayName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(displayName.ToLower());
        
        // Common replacements for Persian display
        var replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Cpu", "CPU" },
            { "Memory", "حافظه" },
            { "Usage", "مصرف" },
            { "Percent", "درصد" },
            { "Count", "تعداد" },
            { "Total", "کل" },
            { "Active", "فعال" },
            { "Requests", "درخواست‌ها" },
            { "Errors", "خطاها" },
            { "Success", "موفق" },
            { "Failed", "ناموفق" },
            { "Response Time", "زمان پاسخ" },
            { "Duration", "مدت" },
            { "Connections", "اتصالات" },
            { "Sessions", "نشست‌ها" },
            { "Users", "کاربران" },
            { "System", "سیستم" },
            { "Application", "برنامه" },
            { "Database", "پایگاه داده" },
            { "Cache", "کش" },
            { "Queue", "صف" },
            { "Gc", "GC" },
        };

        foreach (var replacement in replacements)
        {
            displayName = Regex.Replace(displayName, $@"\b{replacement.Key}\b", replacement.Value, RegexOptions.IgnoreCase);
        }

        return displayName.Trim();
    }

    public string CategorizeMetric(string metricName)
    {
        var normalizedName = metricName.ToLowerInvariant();

        foreach (var pattern in CategoryPatterns)
        {
            if (normalizedName.Contains(pattern.Key))
            {
                return pattern.Value;
            }
        }

        return "عمومی";
    }

    public int CalculatePriority(string metricName, MetricType type, MetricUnitCategory unitCategory)
    {
        var priority = 50; // Base priority
        var normalizedName = metricName.ToLowerInvariant();

        // High priority patterns
        foreach (var pattern in HighPriorityPatterns)
        {
            if (normalizedName.Contains(pattern))
            {
                priority += 20;
                break;
            }
        }

        // Type-based priority
        priority += type switch
        {
            MetricType.Status => 15,
            MetricType.Gauge => 10,
            MetricType.Rate => 10,
            MetricType.Counter => 5,
            MetricType.Histogram => 5,
            _ => 0
        };

        // Unit-based priority
        priority += unitCategory switch
        {
            MetricUnitCategory.Percentage => 10,
            MetricUnitCategory.Duration => 5,
            MetricUnitCategory.Currency => 15,
            _ => 0
        };

        // System metrics get slight boost
        if (normalizedName.StartsWith("system.") || normalizedName.StartsWith("process."))
        {
            priority += 5;
        }

        return Math.Min(100, Math.Max(0, priority));
    }
}

