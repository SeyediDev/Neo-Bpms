namespace Neo.Bpms.UI.MVC.Controls;

/// <summary>
/// Provides menu rendering functionality
/// </summary>
public interface IMenuHelper
{
    /// <summary>
    /// Gets navigation menu items as HTML string
    /// </summary>
    /// <param name="pagePackId">The page pack ID</param>
    /// <param name="user">The current user</param>
    /// <param name="urlHelper">The URL helper</param>
    /// <returns>HTML string containing menu items</returns>
    HtmlString GetMenuItems(string pagePackId, IdentityUser user, IUrlHelper urlHelper);
}

