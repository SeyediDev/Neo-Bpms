using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Recommends the best widget type for a given metric
/// </summary>
public interface IWidgetRecommender
{
    /// <summary>
    /// Get widget recommendation for a single metric
    /// </summary>
    WidgetRecommendation RecommendWidget(AnalyzedMetric metric);
    
    /// <summary>
    /// Get widget recommendations for multiple metrics
    /// </summary>
    IEnumerable<WidgetRecommendation> RecommendWidgets(IEnumerable<AnalyzedMetric> metrics);
    
    /// <summary>
    /// Get default configuration for a widget type
    /// </summary>
    WidgetConfiguration GetDefaultConfiguration(WidgetType widgetType, AnalyzedMetric metric);
    
    /// <summary>
    /// Get threshold configuration based on metric characteristics
    /// </summary>
    ThresholdConfiguration? GetDefaultThresholds(AnalyzedMetric metric);
}

