using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Arranges widgets in an optimal grid layout
/// </summary>
public class LayoutEngine : ILayoutEngine
{
    public List<DashboardWidget> ArrangeWidgets(
        IEnumerable<WidgetRecommendation> recommendations, 
        int gridColumns = 4)
    {
        var widgets = new List<DashboardWidget>();
        var grid = new bool[100, gridColumns]; // Max 100 rows
        var currentRow = 0;

        // Sort by confidence and then by size (larger first)
        var sortedRecommendations = recommendations
            .OrderByDescending(r => r.Confidence)
            .ThenByDescending(r => GetSizeSpan(r.Size).colSpan * GetSizeSpan(r.Size).rowSpan)
            .ToList();

        foreach (var recommendation in sortedRecommendations)
        {
            var (rowSpan, colSpan) = GetSizeSpan(recommendation.Size);
            var position = FindAvailablePosition(grid, gridColumns, rowSpan, colSpan, ref currentRow);

            if (position == null)
            {
                // Skip if can't fit (shouldn't happen with reasonable limits)
                continue;
            }

            // Mark grid cells as occupied
            for (int r = position.Value.row; r < position.Value.row + rowSpan; r++)
            {
                for (int c = position.Value.col; c < position.Value.col + colSpan; c++)
                {
                    grid[r, c] = true;
                }
            }

            widgets.Add(new DashboardWidget
            {
                MetricName = recommendation.MetricName,
                WidgetType = recommendation.RecommendedWidget,
                Size = recommendation.Size,
                Position = new GridPosition
                {
                    Row = position.Value.row,
                    Column = position.Value.col,
                    RowSpan = rowSpan,
                    ColSpan = colSpan
                },
                Configuration = recommendation.Configuration
            });
        }

        return widgets;
    }

    public List<WidgetGroup> GroupWidgets(IEnumerable<DashboardWidget> widgets, IEnumerable<AnalyzedMetric> metrics)
    {
        var metricMap = metrics.ToDictionary(m => m.Name, m => m.Category ?? "عمومی", StringComparer.OrdinalIgnoreCase);
        var groups = new Dictionary<string, WidgetGroup>();
        var order = 0;

        foreach (var widget in widgets)
        {
            var category = metricMap.GetValueOrDefault(widget.MetricName, "عمومی");
            
            if (!groups.TryGetValue(category, out var group))
            {
                group = new WidgetGroup
                {
                    Name = category,
                    Icon = GetIconForCategory(category),
                    Order = order++,
                    WidgetIds = []
                };
                groups[category] = group;
            }

            group.WidgetIds.Add(widget.Id);
        }

        return groups.Values.OrderBy(g => g.Order).ToList();
    }

    public List<DashboardWidget> ApplyTemplate(
        DashboardTemplate template, 
        IEnumerable<WidgetRecommendation> recommendations,
        IEnumerable<AnalyzedMetric> metrics)
    {
        var widgets = new List<DashboardWidget>();
        var usedMetrics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var recommendationsList = recommendations.ToList();
        var metricsList = metrics.ToList();

        foreach (var slot in template.Slots)
        {
            // Find best matching metric for this slot
            var bestMatch = FindBestMetricForSlot(slot, recommendationsList, metricsList, usedMetrics);
            
            if (bestMatch == null)
                continue;

            usedMetrics.Add(bestMatch.MetricName);

            // Use slot's position and size, but recommendation's configuration
            widgets.Add(new DashboardWidget
            {
                Id = slot.Id,
                MetricName = bestMatch.MetricName,
                WidgetType = slot.AllowedWidgetTypes.Contains(bestMatch.RecommendedWidget) 
                    ? bestMatch.RecommendedWidget 
                    : slot.AllowedWidgetTypes.FirstOrDefault(),
                Size = slot.PreferredSize,
                Position = slot.Position,
                Configuration = bestMatch.Configuration
            });
        }

        return widgets;
    }

    private WidgetRecommendation? FindBestMetricForSlot(
        TemplateWidgetSlot slot,
        List<WidgetRecommendation> recommendations,
        List<AnalyzedMetric> metrics,
        HashSet<string> usedMetrics)
    {
        // First, try to match by name pattern
        if (!string.IsNullOrEmpty(slot.MetricNamePattern))
        {
            var patternMatch = recommendations
                .FirstOrDefault(r => 
                    !usedMetrics.Contains(r.MetricName) &&
                    r.MetricName.Contains(slot.MetricNamePattern, StringComparison.OrdinalIgnoreCase));
            
            if (patternMatch != null)
                return patternMatch;
        }

        // Then, try to match by preferred metric types
        if (slot.PreferredMetricTypes.Any())
        {
            var metricByType = metrics
                .Where(m => 
                    !usedMetrics.Contains(m.Name) &&
                    slot.PreferredMetricTypes.Contains(m.Type))
                .OrderByDescending(m => m.Priority)
                .FirstOrDefault();

            if (metricByType != null)
            {
                return recommendations.FirstOrDefault(r => r.MetricName == metricByType.Name);
            }
        }

        // Finally, try to match by allowed widget types
        if (slot.AllowedWidgetTypes.Any())
        {
            return recommendations
                .FirstOrDefault(r => 
                    !usedMetrics.Contains(r.MetricName) &&
                    slot.AllowedWidgetTypes.Contains(r.RecommendedWidget));
        }

        // No match found
        return null;
    }

    private (int rowSpan, int colSpan) GetSizeSpan(WidgetSize size)
    {
        return size switch
        {
            WidgetSize.Small => (1, 1),
            WidgetSize.Medium => (1, 2),
            WidgetSize.Large => (2, 2),
            WidgetSize.ExtraLarge => (2, 4),
            _ => (1, 1)
        };
    }

    private (int row, int col)? FindAvailablePosition(
        bool[,] grid, 
        int gridColumns, 
        int rowSpan, 
        int colSpan,
        ref int startRow)
    {
        for (int row = startRow; row < grid.GetLength(0) - rowSpan; row++)
        {
            for (int col = 0; col <= gridColumns - colSpan; col++)
            {
                if (CanFit(grid, row, col, rowSpan, colSpan))
                {
                    return (row, col);
                }
            }
        }

        return null;
    }

    private bool CanFit(bool[,] grid, int startRow, int startCol, int rowSpan, int colSpan)
    {
        for (int r = startRow; r < startRow + rowSpan; r++)
        {
            for (int c = startCol; c < startCol + colSpan; c++)
            {
                if (grid[r, c])
                    return false;
            }
        }
        return true;
    }

    private string GetIconForCategory(string category)
    {
        return category switch
        {
            "سیستم" => "💻",
            "شبکه" => "🌐",
            "وب" => "🌍",
            "پایگاه داده" => "🗄️",
            "کش" => "⚡",
            "صف" => "📥",
            "GC" => "🗑️",
            "تردها" => "🧵",
            "استخر" => "🏊",
            "اتصالات" => "🔗",
            "نشست‌ها" => "👥",
            "کاربران" => "👤",
            "احراز هویت" => "🔐",
            "کسب‌وکار" => "📊",
            _ => "📈"
        };
    }
}

