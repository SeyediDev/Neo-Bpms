namespace Neo.Bpms.UI.MVC.Controllers.Public;

public class FilterController(
    ControllerMethods controllerMethods,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore
    ) : ControllerBaseMVC
{
    [HttpPost]
    public async Task<JsonResult> SaveFilterConfig(
        string NamespaceId, string EntityId,
        string FormId, string ReportId, string DashboardId,
        long? FilterId, string Name,
        bool? IsPublic, string? UserGroupId, string ConfigId,
        long? FolderId, bool? IsDefault,
        List<ConfiguredFilterValue> FilterValues, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(NamespaceId, EntityId);
        Form form = null;
        Report report = null;
        Dashboard dashboard = null;
        if (!controllerMethods.CheckPageAccess(NamespaceId, EntityId, FormId, ReportId, DashboardId,
            entity, user, ref form, ref report, ref dashboard, out string errorString))
        {
            throw new HttpException(errorString);
        }

        if ((IsPublic ?? false) && !CheckAccess(user, SystemFeatureId.PublishConfigs))
        {
            return Json("خطا: شما دسترسی انتشار طراحی گزارش ندارید.");
        }

        if (!string.IsNullOrEmpty(UserGroupId) && !user.IsAdmin && !user.CheckRole(UserGroupId))
        {
            return Json("خطا: شما دسترسی به گروه کاربری فوق را ندارید.");
        }

        ConfiguredFilter configuredFilter;
        if (FilterId is null or 0)
        {
            configuredFilter = new ConfiguredFilter(0, Name)
            {
                IsPublic = IsPublic ?? false,
                Roles = UserGroupId != null ? [UserGroupId] : [],
                UserId = user.Id,
                ConfigId = ConfigId,
                FolderId = FolderId ?? 0,
                IsDefault = IsDefault ?? false,
                Values = FilterValues,
                EntityItem = new EntityItem
                {
                    NamespaceId = NamespaceId,
                    EntityId = EntityId,
                    ItemType = ItemConfigBackupRestore.ItemType(FormId, ReportId, DashboardId),
                    ItemId = ItemConfigBackupRestore.ItemId(FormId, ReportId, DashboardId)
                }
            };
            await filterConfigBackupRestore.Save(configuredFilter, cancellationToken);
        }
        else
        {
            configuredFilter = await filterConfigBackupRestore.GetConfig(FilterId.Value, cancellationToken);
            if (configuredFilter != null)
            {
                if (configuredFilter.IsPublic && !CheckAccess(user, SystemFeatureId.PublishConfigs))
                {
                    return Json("خطا: شما دسترسی تغییر طراحی همگانی را ندارید.");
                }

                configuredFilter.IsPublic = IsPublic ?? false;
                configuredFilter.Roles = UserGroupId != null ? [UserGroupId] : null;
                configuredFilter.UserId = user.Id;
                configuredFilter.ConfigId = ConfigId;
                configuredFilter.FolderId = FolderId ?? 0;
                configuredFilter.IsDefault = IsDefault ?? false;
                configuredFilter.Name = Name;
                if (FilterValues != null)
                {
                    configuredFilter.Values = FilterValues;
                }

                await filterConfigBackupRestore.Save(configuredFilter, cancellationToken);
            }
            else
            {
                return Json("خطا: کد طراحی فیلتر معتبر نیست.");
            }
        }

        return Json(configuredFilter.Id);
    }

    [HttpPost]
    public async Task<JsonResult> SaveFilterValues([FromBody] SaveFilterValuesModel saveFilterValuesModel)
    {
        _ = GetUser();
        ConfiguredFilter configuredFilter = await FetchFilter(saveFilterValuesModel.FilterId);
        if (configuredFilter != null)
        {
            configuredFilter.Values = saveFilterValuesModel.FilterValues;
            await filterConfigBackupRestore.Save(configuredFilter);
        }
        else
        {
            return Json("خطا: کد طراحی فیلتر معتبر نیست.");
        }

        return Json(configuredFilter.Id);
    }

    [HttpPost]
    public async Task<JsonResult> ChangeParent(long? filterId, long? folderId, long? newFolderId)
    {
        _ = GetUser();
        if (folderId != null)
        {
            ConfiguredFolder folderConfig = await folderConfigBackupRestore.GetConfig(folderId.Value)
                ?? throw new Exception("خطا کد پوشه صحیح نیست");
            folderConfig.FolderId = newFolderId ?? 0;
            await folderConfigBackupRestore.Save(folderConfig);
        }
        else if (filterId != null)
        {
            ConfiguredFilter filter = await FetchFilter(filterId.Value);
            if (filter != null)
            {
                filter.FolderId = newFolderId ?? 0;
                await filterConfigBackupRestore.Save(filter);
            }
            else
            {
                return Json("خطا: کد طراحی فیلتر معتبر نیست.");
            }
        }

        return Json(string.Empty);
    }

    [HttpPost]
    public async Task<JsonResult> DeleteFilterConfig(long filterId)
    {
        IdentityUser user = GetUser();
        ConfiguredFilter configuredFilter = await FetchFilter(filterId);
        if (configuredFilter != null)
        {
            if (configuredFilter.IsPublic && !CheckAccess(user, SystemFeatureId.PublishConfigs))
            {
                return Json("خطا: شما دسترسی حذف طراحی همگانی را ندارید.");
            }

            if(!configuredFilter.CheckAccess(user))
            {
                return Json("خطا: شما دسترسی به نقش فوق را ندارید.");
            }

            await filterConfigBackupRestore.RemoveConfig(configuredFilter);
        }
        else
        {
            return Json("خطا: کد طراحی فیلتر معتبر نیست.");
        }

        return Json("");
    }

    public async Task<ActionResult> OpenFilter(long filterId, string configId, CancellationToken cancellationToken)
    {
        ConfiguredFilter filter = await filterConfigBackupRestore.GetConfig(filterId, cancellationToken) ??
            throw new Exception("Invalid filter");
        return RedirectToAction("", filter.EntityItem.ItemType, new
        {
            filter.EntityItem.NamespaceId,
            filter.EntityItem.EntityId,
            filter.EntityItem.ItemId,
            ConfigId = configId,
            FilterId = filter.Id
        });
    }

    private async Task<ConfiguredFilter?> FetchFilter(long filterId)
    {
        ConfiguredFilter? filter = await filterConfigBackupRestore.GetConfig(filterId);
        return filter;
    }
}

public class SaveFilterValuesModel
{
    public long FilterId { get; set; }
    public List<ConfiguredFilterValue> FilterValues { get; set; }
}
