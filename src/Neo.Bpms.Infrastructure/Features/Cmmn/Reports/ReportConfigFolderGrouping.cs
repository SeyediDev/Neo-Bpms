using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public interface IReportConfigFolderGrouping
{
    public List<ConfigTreeItem> ConfigsTree(ReportStructure structure);
    Task EnsureAutoFolderGrouping(ReportStructure structure, Report report, IdentityUser user);
}
public class ReportConfigFolderGrouping(
    FolderConfigBackupRestore folderConfigBackupRestore,
    ReportConfigBackupRestore reportConfigBackupRestore) 
    : IReportConfigFolderGrouping
{
    public List<ConfigTreeItem> ConfigsTree(ReportStructure structure)
    {
        List<ConfiguredFolder> configuredFolders = structure.ConfiguredFolders;
        List<ConfiguredReport> configs = structure.Configs;
        string configId = structure.ConfigId;
        List<ConfigTreeItem> result = configuredFolders?.Where(f => f.IsForConfig)
            .OrderBy(f => f.IsPublic.ToString() + string.Join(",", f.Roles ?? []) + f.Name)
            .Select(f =>
            {
                ConfiguredFolder folder = configuredFolders?.FirstOrDefault(ff => ff.Id == f.FolderId);
                return new ConfigTreeItem
                {
                    id = f.Id.ToString(),
                    data = new
                    {
                        isPublic = f.IsPublic,
                        userGroupId = string.Join(",", f.Roles ?? []),
                        isForConfig = true,
                        parentFolderId = folder?.Id
                    },
                    parent = folder?.Id.ToString() ?? "#",
                    text = f.Name,
                    type = "default"
                };
            }).ToList();

        result?.AddRange(configs?.Where(cf => cf.Parent == null )
                             .OrderBy(f => f.ViewType + f.IsPublic.ToString() + string.Join(",", f.Roles ?? []) + f.Name)
                             .Select(c =>
                             {
                                 ConfiguredFolder folder = configuredFolders?.FirstOrDefault(ff => ff.Id == c.FolderId);
                                 // Convert ViewType to tree type: List -> ReportList
                                 // For Chart type, use Chart.{GetChartType(ChartType)} format for icon switching in _Scripts.report-modals.cshtml
                                 string treeType = c.ViewType == ReportViewType.List 
                                     ? "ReportList" 
                                     : c.ViewType == ReportViewType.Chart 
                                         ? $"Chart.{c.ChartType}" 
                                         : c.ViewType.ToString();
                                 return new ConfigTreeItem
                                 {
                                     id = c.ConfigId,
                                     parent = folder?.Id.ToString() ?? "#",
                                     text = c.Name,
                                     type = treeType,
                                     data = new
                                     {
                                         hasSchedules = c.ScheduledReports?.Any() ?? false,
                                         configId = c.ConfigId,
                                         isPublic = c.IsPublic,
                                         isMeta = c.IsMeta,
                                         isDefault = c.IsDefault,
                                         isActive = c.ConfigId == configId,
                                         chartType = c.ViewType == ReportViewType.Chart ? c.ChartType.ToString() : null
                                     }
                                 };
                             }) ?? []);

        return result;
    }
    public async Task EnsureAutoFolderGrouping(ReportStructure structure, Report report, IdentityUser user)
    {
        const int configThreshold = 6;

        // Get existing folders
        var existingFolders = structure.ConfiguredFolders?.Where(f => f.IsForConfig).ToList() ?? [];

        // Validate existing folder assignments - remove invalid FolderIds
        var validFolderIds = existingFolders.Select(f => f.Id).ToHashSet();
        foreach (var config in structure.Configs ?? [])
        {
            if (config.FolderId.HasValue && config.FolderId.Value > 0 && !validFolderIds.Contains(config.FolderId.Value))
            {
                // Folder doesn't exist, clear the assignment
                config.FolderId = null;
                await reportConfigBackupRestore.Save(config);
            }
        }

        // Only process configs that don't have a folder assigned
        var configsWithoutFolder = structure.Configs?.Where(c => c.FolderId == null || c.FolderId == 0).ToList() ?? [];

        // If all configs already have folders, don't process
        if (configsWithoutFolder.Count == 0)
            return;

        // Only process if we have more than threshold configs without folder
        if (configsWithoutFolder.Count <= configThreshold)
            return;

        // Step 1: Group by ViewType first
        var viewTypeGroups = configsWithoutFolder
            .GroupBy(config => $"{config.ViewType}.{GetChartType(config.ChartType)}")
            .ToList();

        foreach (var viewTypeGroup in viewTypeGroups)
        {
            var configsInViewType = viewTypeGroup.ToList();
            var viewTypeParts = viewTypeGroup.Key.Split('.');
            var viewType = Enum.Parse<ReportViewType>(viewTypeParts[0]);
            var chartType = Enum.Parse<ChartType>(viewTypeParts[1]);
            var viewTypeName = GetViewTypeChartTypeName(viewType, chartType);

            var folderName = GenerateFolderName(viewTypeName, null);
            var viewTypeGroupFolder = await CreateOrGetFolder(
                folderName, null, existingFolders, structure, report);

            if (configsInViewType.Count <= configThreshold)
            {
                foreach (var config in configsInViewType)
                {
                    config.FolderId = viewTypeGroupFolder.Id;
                    await reportConfigBackupRestore.Save(config);
                }
            }
            else
            {
                // Group by GroupBy fields and create sub-folders
                var groupByGroups = configsInViewType
                    .GroupBy(GetGroupByFieldsKey)
                    .ToList();

                foreach (var groupByGroup in groupByGroups)
                {
                    var configsInGroupBy = groupByGroup.ToList();
                    var groupByKey = groupByGroup.Key;

                    var groupByFolderName = GenerateFolderName(null, groupByKey);
                    var groupByFolder = await CreateOrGetFolder(
                        groupByFolderName, viewTypeGroupFolder.Id,
                        existingFolders, structure, report);

                    foreach (var config in configsInGroupBy)
                    {
                        config.FolderId = groupByFolder.Id;
                        await reportConfigBackupRestore.Save(config);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Creates or gets an existing folder
    /// </summary>
    private async Task<ConfiguredFolder> CreateOrGetFolder(
        string folderName,
        long? parentFolderId,
        List<ConfiguredFolder> existingFolders,
        ReportStructure structure,
        Report report)
    {
        var targetFolder = existingFolders.FirstOrDefault(f => f.Name == folderName && GetFolderId(f.FolderId) == GetFolderId(parentFolderId));

        if (targetFolder == null)
        {
            targetFolder = new ConfiguredFolder
            {
                Name = folderName,
                IsForConfig = true,
                IsPublic = true,
                FolderId = parentFolderId,
                ConfigId = Guid.NewGuid().ToString(),
                EntityItem = new EntityItem
                {
                    ItemType = "Report",
                    NamespaceId = report.NamespaceId,
                    EntityId = report.entity.Id,
                    ItemId = report.Id
                }
            };

            await folderConfigBackupRestore.Save(targetFolder);
            structure.ConfiguredFolders ??= [];
            structure.ConfiguredFolders.Add(targetFolder);
            existingFolders.Add(targetFolder);
        }

        return targetFolder;
    }

    private static long? GetFolderId(long? parentFolderId)
    {
        return parentFolderId == 0 ? null : parentFolderId;
    }

    /// <summary>
    /// Gets a key representing the GroupBy fields for grouping purposes
    /// </summary>
    private static string GetGroupByFieldsKey(ConfiguredReport config)
    {
        if (config.Fields == null || config.Fields.IsEmpty)
        {
            return (config.ViewType == ReportViewType.List) ? "بدون گروه‌بندی" : "کل";
        }

        var groupByFields = config.Fields.Values
            .Where(f => f.type == ConfiguredReport.eFieldSelectionType.asGroupBy)
            .OrderBy(f => f.Order)
            .Select(f => !string.IsNullOrEmpty(f.Alias) ? f.Alias : f.fieldId)
            .ToList();

        return groupByFields.Count == 0
            ? (config.ViewType == ReportViewType.List) ? "بدون گروه‌بندی" : "کل"
            : string.Join(" - ", groupByFields);
    }

    /// <summary>
    /// Generates an appropriate folder name based on ViewType, GroupBy fields
    /// </summary>
    private static string GenerateFolderName(
        string viewTypeName,
        string? groupByFields)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(viewTypeName))
        {
            parts.Add(viewTypeName);
        }

        if (!string.IsNullOrEmpty(groupByFields))
        {
            parts.Add(groupByFields);
        }

        return string.Join(" - ", parts);
    }

    private static ChartType GetChartType(ChartType chartType)
    {
        return chartType switch
        {
            ChartType.IranMap or ChartType.WorldMap or ChartType.Treemap => ChartType.Treemap,
            ChartType.BpmnDiagram => ChartType.BpmnDiagram,
            ChartType.MetricBox => ChartType.MetricBox,
            ChartType.Gauge => ChartType.Gauge,
            _ => ChartType.Column
        };
    }
    private static string GetChartTypeName(ChartType chartType)
    {
        return chartType switch
        {
            ChartType.IranMap or ChartType.WorldMap or ChartType.Treemap => "نقشه ها",
            ChartType.BpmnDiagram => "دیاگرام BPMN",
            ChartType.MetricBox => "متریک ها",
            ChartType.Gauge => "گاج ها",
            _ => "نمودارها"
        };
    }

    /// <summary>
    /// Gets a friendly name for ReportViewType
    /// </summary>
    private static string GetViewTypeChartTypeName(ReportViewType viewType, ChartType chartType)
    {
        return viewType switch
        {
            ReportViewType.List => "جداول ریزاطلاعات",
            ReportViewType.GroupByList => "جداول گروه‌بندی شده",
            ReportViewType.Chart => GetChartTypeName(chartType),
            ReportViewType.Dashboard => "داشبورد",
            _ => "گزارش"
        };
    }
}
