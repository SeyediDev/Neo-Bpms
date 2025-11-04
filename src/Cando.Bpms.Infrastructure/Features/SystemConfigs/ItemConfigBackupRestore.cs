using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public abstract class ItemConfigBackupRestore()
{
    protected abstract string SubjectTitle { get; }
    protected static string SubjectId(string itemType, string namespaceId, string entityId, string itemId)
    {
        return $"{namespaceId}.{entityId}.{itemId}.{itemType}";
    }

    public static string ItemType(string formId, string reportId, string dashboardId)
    {
        return !string.IsNullOrEmpty(formId) ? ConfigType.Form :
        !string.IsNullOrEmpty(reportId) ? ConfigType.Report :
        !string.IsNullOrEmpty(dashboardId) ? ConfigType.Dashboard : "-";
    }

    public static string ItemId(string formId, string reportId, string dashboardId)
    {
        return !string.IsNullOrEmpty(formId) ? formId :
            !string.IsNullOrEmpty(reportId) ? reportId :
            !string.IsNullOrEmpty(dashboardId) ? dashboardId : "-";
    }
}

public abstract class ItemConfigBackupRestore<TConfiguredItem>() : ItemConfigBackupRestore
    where TConfiguredItem : ConfiguredItem
{
    public static TConfiguredItem GetDefaultItem(IEnumerable<TConfiguredItem> list, IdentityUser user)
    {
        var items = list?.Where(f => f.CheckAccess(user)).ToList();
        return items?.FirstOrDefault(f =>
                   f.IsDefault && !f.IsPublic )
               ?? items?.FirstOrDefault(f =>
                   f.IsDefault && f.IsPublic && f.Roles != null && f.Roles.Any())
               ?? items?.FirstOrDefault(f =>
                   f.IsDefault && f.IsPublic && (f.Roles == null || !f.Roles.Any()));
    }
}
