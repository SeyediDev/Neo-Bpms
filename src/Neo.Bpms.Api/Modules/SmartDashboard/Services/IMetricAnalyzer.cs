using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Analyzes metrics to determine their type, unit, and characteristics
/// </summary>
public interface IMetricAnalyzer
{
    /// <summary>
    /// Analyze all available metrics
    /// </summary>
    Task<IEnumerable<AnalyzedMetric>> AnalyzeAllMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyze a specific metric by name
    /// </summary>
    Task<AnalyzedMetric?> AnalyzeMetricAsync(string metricName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Detect the type of a metric based on its name and values
    /// </summary>
    MetricType DetectMetricType(string metricName, string? unit, IEnumerable<double>? values);
    
    /// <summary>
    /// Detect the unit category from a unit string
    /// </summary>
    MetricUnitCategory DetectUnitCategory(string? unit);
    
    /// <summary>
    /// Generate a human-readable display name from metric name
    /// </summary>
    string GenerateDisplayName(string metricName);
    
    /// <summary>
    /// Categorize a metric for grouping purposes
    /// </summary>
    string CategorizeMetric(string metricName);
    
    /// <summary>
    /// Calculate priority/importance of a metric
    /// </summary>
    int CalculatePriority(string metricName, MetricType type, MetricUnitCategory unitCategory);
}

