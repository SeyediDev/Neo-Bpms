using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using static Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;

public interface IDashboardDivDefinition
{
    string Title { get; }
    long Width { get; }
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

/// <summary>
/// Base class to define dashboard divs and their layout. All dashboard div definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract class DashboardDivDefinition : BaseModelingDefinition, IDashboardDivDefinition
{
    public abstract string Title { get; }
    /// <summary>
    /// Div Width
    /// </summary>
    public virtual long Width { get; } = 6;

    /// <summary>
    /// Is Row
    /// </summary>
    public virtual bool IsRow { get; } = false;
    public ConfigDiv Define(ConfiguredDashboard dashboardConfig, ConfigDiv parentDiv)
    {
        ConfigDiv div = ((IDashboardDivDefinition)this).AddDiv(dashboardConfig, parentDiv);
        foreach (var configDefinition in GetType().ExtractSubsInstances<DashboardDivDefinition>())
        {
            configDefinition?.Define(dashboardConfig, div);
        }
        foreach (var configDefinition in GetType().ExtractSubsInstances<DashboardDivWidgetDefinition>())
        {
            configDefinition?.Define(dashboardConfig, div);
        }
        foreach (var configDefinition in GetType().ExtractSubsInstances<DashboardWidgetDefinition>())
        {
            configDefinition?.Define(dashboardConfig, div);
        }
        return div;
    }
}
