using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Dashboards;

public interface IDashboardDivDefinition
{
    string Title { get; }
    int Width { get; }
    bool IsRow { get; }
    ConfigDiv AddDiv(ConfiguredDashboard dashboardConfig, ConfigDiv parentDiv)
    {
        ConfigDiv div = new()
        {
            Id = GetType().Name,
            Title = Title,
            Width = Width,
            IsRow = IsRow,
            Children = [],
            Parent = parentDiv,
            Widget = null,
            WidgetId = null,
        };
        if (parentDiv != null)
        {
            parentDiv?.Children.Add(div); 
        }
        else
        {
            dashboardConfig.Divs.Add(div);
        }
        return div;
    }
}
