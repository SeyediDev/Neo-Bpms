using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Security.Authorization;

namespace Neo.Bpms.UI.MVC.Controllers.Public;

public class FolderController(DashboardConfigBackupRestore dashboardConfigBackupRestore,
    ReportConfigBackupRestore reportConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore,
    ControllerMethods controllerMethods, 
    ReportConfigManager reportConfigManager,
    DashboardConfigManager dashboardConfigManager) : ControllerBaseMVC
{
    [HttpPost]
    public async Task<JsonResult> SaveFolderConfig(string NamespaceId, string EntityId,
        string FormId, string ReportId, string DashboardId,
        long? FolderId, string Name,
        bool? IsPublic, string? UserGroupId,
        long? ParentFolderId, bool? IsDefault, bool? IsForConfig)
    {
        IdentityUser user = GetUser();
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(NamespaceId, EntityId);
        Form form = null;
        Report report = null;
        Dashboard dashboard = null;
        if (!controllerMethods.CheckPageAccess(NamespaceId, EntityId, FormId, ReportId, DashboardId,
            entity, user, ref form, ref report, ref dashboard, out string errorString))
            throw new HttpException(errorString);
        if ((IsPublic ?? false) && !CheckAccess(user, SystemFeatureId.PublishConfigs))
            return Json("خطا: شما دسترسی انتشار طراحی گزارش ندارید.");
        if (!string.IsNullOrEmpty(UserGroupId) && !user.IsAdmin && !user.CheckRole(UserGroupId))
        {
            return Json("خطا: شما دسترسی به گروه کاربری فوق را ندارید.");
        }

        ConfiguredFolder configuredFolder;
        if (!FolderId.HasValue || FolderId == 0)
        {
            configuredFolder = new ConfiguredFolder(0, Name)
            {
                IsDefault = IsDefault ?? false,
                IsPublic = IsPublic ?? false,
                Roles = UserGroupId!=null ? [UserGroupId] : [],
                UserId = user.Id,
                FolderId = ParentFolderId ?? 0,
                IsForConfig = IsForConfig ?? false,
                EntityItem = new EntityItem
                {
                    NamespaceId = NamespaceId,
                    EntityId = EntityId,
                    ItemType = ItemConfigBackupRestore.ItemType(FormId, ReportId, DashboardId),
                    ItemId = ItemConfigBackupRestore.ItemId(FormId, ReportId, DashboardId)
                }
            };
            await folderConfigBackupRestore.Save(configuredFolder);
        }
        else
        {
            configuredFolder = await folderConfigBackupRestore.GetConfig(FolderId.Value);
            if (configuredFolder != null)
            {
                if (configuredFolder.IsPublic && !CheckAccess(user, SystemFeatureId.PublishConfigs))
                    return Json("خطا: شما دسترسی تغییر پوشه همگانی را ندارید.");
                configuredFolder.IsPublic = IsPublic ?? false;
                configuredFolder.Roles = UserGroupId != null ? [UserGroupId] : null;
                configuredFolder.UserId = user.Id;
                configuredFolder.FolderId = ParentFolderId ?? 0;
                configuredFolder.IsForConfig = IsForConfig ?? false;
                configuredFolder.Name = Name;
                await folderConfigBackupRestore.Save(configuredFolder);
            }
            else
                return Json("خطا: کد تنظیم معتبر نیست.");
        }
        return Json(new { configuredFolder.Id, ParentFolderId, configuredFolder.Name });
    }

    [HttpPost]
    public async Task<JsonResult> ChangeFolderParent(long? folderId, long? newFolderId, CancellationToken cancellationToken = default)
    {
        GetUser();
        ConfiguredFolder configuredFolder = await controllerMethods.FetchFolder(folderId ?? 0);
        if (configuredFolder != null)
        {
            configuredFolder.FolderId = newFolderId ?? 0;
            await folderConfigBackupRestore.Save(configuredFolder, cancellationToken );
        }
        return Json(string.Empty);
    }

    [HttpPost]
    public async Task<JsonResult> ChangeConfigParent(
        string configType, string configId, long? folderId, long? newFolderId, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();
        if (folderId != null)
        {
            await ChangeFolderParent(folderId, newFolderId, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(configId))
        {
            if (string.IsNullOrEmpty(configType) || configType == ConfigType.Report)
            {
                GetReportConfigResult getReportConfigResult = new(configId);
                if (await reportConfigManager.GetReportConfig("", "", "", getReportConfigResult, user, cancellationToken))
                {
                    getReportConfigResult.Config.FolderId = newFolderId ?? 0;
                    await reportConfigBackupRestore.Save(getReportConfigResult.Config, cancellationToken);
                }
            }
            else if (configType == ConfigType.Dashboard)
            {
                GetDashboardConfigResult cfgResult = new(configId);
                if (await dashboardConfigManager.GetDashboardConfig("", "", "", cfgResult, user, cancellationToken))
                {
                    cfgResult.Config.FolderId = newFolderId ?? 0;
                    await dashboardConfigBackupRestore.Save(cfgResult.Config, cancellationToken);
                }
            }
        }
        return Json(string.Empty);
    }

    [HttpPost]
    public async Task<JsonResult> DeleteFolderConfig(long folderId, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();

        ConfiguredFolder folderConfig = await controllerMethods.FetchFolder(folderId);

        if (folderConfig != null)
        {
            if (folderConfig.IsPublic && !CheckAccess(user, SystemFeatureId.PublishConfigs))
                return Json("خطا: شما دسترسی حذف پوشه همگانی را ندارید.");
            if (!folderConfig.CheckAccess(user))
            {
                return Json("خطا: شما دسترسی به نقش فوق را ندارید.");
            }
            await folderConfigBackupRestore.RemoveConfig(folderConfig, cancellationToken);
        }
        else
        {
            return Json("خطا: کد پوشه معتبر نیست.");
        }
        return Json("");
    }
}
