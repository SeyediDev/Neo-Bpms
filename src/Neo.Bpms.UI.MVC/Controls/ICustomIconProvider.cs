namespace Neo.Bpms.UI.MVC.Controls;

/// <summary>
/// Provides custom icon names for the menu system
/// </summary>
public interface ICustomIconProvider
{
    /// <summary>
    /// Checks if an icon name is a custom icon
    /// </summary>
    /// <param name="iconName">The icon name to check</param>
    /// <returns>True if the icon is a custom icon, false otherwise</returns>
    bool IsCustomIcon(string iconName);
}

