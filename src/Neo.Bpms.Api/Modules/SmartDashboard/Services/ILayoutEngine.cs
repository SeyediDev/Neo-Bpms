using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Arranges widgets in an optimal layout
/// </summary>
public interface ILayoutEngine
{
    /// <summary>
    /// Arrange widgets in a grid layout
    /// </summary>
    List<DashboardWidget> ArrangeWidgets(
        IEnumerable<WidgetRecommendation> recommendations, 
        int gridColumns = 4);
    
    /// <summary>
    /// Group widgets by category
    /// </summary>
    List<WidgetGroup> GroupWidgets(IEnumerable<DashboardWidget> widgets, IEnumerable<AnalyzedMetric> metrics);
    
    /// <summary>
    /// Apply a template to widget arrangement
    /// </summary>
    List<DashboardWidget> ApplyTemplate(
        DashboardTemplate template, 
        IEnumerable<WidgetRecommendation> recommendations,
        IEnumerable<AnalyzedMetric> metrics);
}

