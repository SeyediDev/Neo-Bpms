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
        return selectedItem?.iconName ?? indexSelectedItem?.iconName;
    }
}
