namespace Neo.Bpms.UI.MVC.Controls;

public class PageIconHelper
{
    public static string GetPageMenuIcon(string pagePackId)
    {
        return GetPageMenuIcon(ProjectDefinition.Project?.MainMenuItem, pagePackId);
    }
    
    public static string GetPageMenuIcon(MenuItem item, string pagePackId)
    {
        if (string.IsNullOrWhiteSpace(pagePackId) || item == null)
            return "";
        MenuItem selectedItem = null, indexSelectedItem = null;
        MenuHelper.GetSelectedItems(item, pagePackId, ref selectedItem, ref indexSelectedItem);
        
        // Get icon from menu item or fallback to entity icon
        return GetMenuItemIcon(selectedItem) ?? GetMenuItemIcon(indexSelectedItem);
    }
    
    /// <summary>
    /// Gets the icon for a menu item.
    /// If the menu item has an icon, it is returned.
    /// Otherwise, the entity icon is returned (fallback).
    /// </summary>
    private static string GetMenuItemIcon(MenuItem item)
    {
        if (item == null) return null;
        
        // Primary: Menu item's own icon
        if (!string.IsNullOrEmpty(item.iconName))
            return item.iconName;
        
        // Fallback: Entity's icon
        string namespaceId = item.GetParameterValue(eMenuItemParameter.NamespaceId);
        string entityId = item.GetParameterValue(eMenuItemParameter.EntityId);
        
        if (string.IsNullOrEmpty(entityId))
            return null;
        
        var entity = ProjectDefinition.Project?.GetEntity(namespaceId, entityId);
        return entity?.Icon;
    }
}
