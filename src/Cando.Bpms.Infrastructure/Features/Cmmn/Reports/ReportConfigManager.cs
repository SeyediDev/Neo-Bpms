using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class GetReportConfigResult(string configId)
{
    public string ConfigId { get; set; } = configId;
    public ConfiguredReport Config { get; set; }
}

public class ReportConfigManager(
    ReportConfigBackupRestore reportConfigBackupRestore,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore)
{
    public async Task<bool> GetReportConfig(string namespaceId, string entityId,
        string reportId,
        GetReportConfigResult result, IdentityUser user,
        CancellationToken cancellationToken = default)
    {
        result.Config = null;
        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Report report = uiEntity?.GetReport(reportId) ?? uiEntity?.GetReports()?.FirstOrDefault();
        string defaultConfiguredId = "default";
        if (!string.IsNullOrEmpty(result.ConfigId))
        {
            result.Config = report == null
                    ? await reportConfigBackupRestore.GetConfig(result.ConfigId, cancellationToken)
                    : await reportConfigBackupRestore.GetConfig(report, result.ConfigId, cancellationToken);
            if (result.Config != null)
            {
                if (!CheckAccess(result.Config, user))
                {
                    result.Config = null;
                }
            }
            if (result.Config == null && result.ConfigId == defaultConfiguredId)
                result.Config = await AddNewConfig(result.ConfigId, report, cancellationToken);
            return result.Config != null;
        }
        if (report == null)
            return false;
        if (report.MetaConfigures?.Count is > 0)
        {
            result.Config = report.MetaConfigures.Values.FirstOrDefault(x => x.IsDefault && CheckAccess(x, user)) ??
                report.MetaConfigures.Values.FirstOrDefault(x => !x.IsDefault && CheckAccess(x, user));
            result.ConfigId = result.Config?.ConfigId;
            if (result.Config != null)
            {
                return true;
            }
        }
        
        List<ConfiguredReport> configurations = [.. (await reportConfigBackupRestore.Configurations(report, cancellationToken)).Where(x=>CheckAccess(x, user))];
        result.Config = ReportConfigBackupRestore.GetDefaultItem(configurations, user)
                ?? configurations.FirstOrDefault(x => x.ConfigId == "default");
        if (result.Config != null)
        {
            result.ConfigId = result.Config.ConfigId;
            return true;
        }
        result.ConfigId = defaultConfiguredId;
        result.Config = await AddNewConfig(result.ConfigId, report, cancellationToken);
        return true;
    }

    private async Task<ConfiguredReport> AddNewConfig(string configId, Report report, CancellationToken cancellationToken)
    {
        ConfiguredReport config = new(report, ReportViewType.List, configId, report.Name);
        await reportConfigBackupRestore.Save(config, cancellationToken);
        return config;
    }

    private static bool CheckAccess(ConfiguredReport config, IdentityUser user)
    {
        return (config?.Parent?.ParentConfiguredReport ?? config).CheckAccess(user);
    }

    public static void SetConfigColumns(PostedOrderdColumn[] selectedColumns, ConfiguredReport config)
    {
        config.Fields.Clear();
        if (selectedColumns == null) return;
        foreach (PostedOrderdColumn colSequence in selectedColumns)
        {
            ConfiguredReport.eFieldSelectionType type = GetAggrType(colSequence.AggrId);
            if (config.ViewType != ReportViewType.List)
            {
                if (colSequence.AggrId == "Count")
                {
                    config.AddField(ConfiguredReport.eFieldSelectionType.asAggregation_Count, "*",
                        colSequence.EntityId, colSequence.AssociationName,
                        colSequence.Alias, colSequence.Formula, colSequence.IsTooltip, colSequence.MatrixType);
                    continue;
                }
            }
            Entity entity1 = colSequence.EntityId == config.Report.entity.Id
                ? config.Report.entity
                : ProjectDefinition.Project.GetEntityByEntityId(colSequence.EntityId);
            if (entity1 == null) continue;
            if (colSequence.EntityId == config.Report.entity.Id)
            {
                if (!config.Report.reportFields.ContainsKey(colSequence.FieldId) &&
                     string.IsNullOrEmpty(colSequence.Formula))
                    continue;
            }
            else if (config.Report.Includes != null &&
                        string.IsNullOrEmpty(colSequence.Formula))
            {
                bool valid = false;
                foreach (IncludeEntity inc in config.Report.Includes)
                {
                    if (colSequence.AssociationName != inc.AssociationId) continue;
                    string[] associationItems = inc.AssociationId.Split('.');
                    int ia = 0;
                    Entity incEntity = config.Report.entity;
                    foreach (string associationItem in associationItems)
                    {
                        EntityField associationField = incEntity.GetField(associationItem);
                        if (associationField?.AssociationEntity?.Entity() != null)
                        {
                            if (ia == associationItems.Length - 1)
                            {
                                if (associationField.AssociationEntity.Entity().Id == colSequence.EntityId &&
                                     inc.reportFields.ContainsKey(colSequence.FieldId))
                                    valid = true;
                                break;
                            }
                            incEntity = associationField.AssociationEntity.Entity();
                        }
                        else break;
                        ia++;
                    }
                    break;
                }
                if (!valid) continue;
            }
            config.AddField(type, colSequence.FieldId, colSequence.EntityId, colSequence.AssociationName, colSequence.Alias,
                colSequence.Formula, colSequence.IsTooltip, colSequence.MatrixType);
        }
    }

    public async Task DeleteReportConfigs(string namespaceId, string entityId, string reportId,
        CancellationToken cancellationToken)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Report report = entity?.GetReport(reportId);
        if (report == null) return;

        await filterConfigBackupRestore.RemoveConfigs(namespaceId, entityId, null, reportId, null, null, cancellationToken);
        await folderConfigBackupRestore.RemoveConfigs(namespaceId, entityId, null, reportId, null, null, cancellationToken);
    }

    public static void SetConfigProperties(ValueRange valueRange, ValueRange[] levels, PostedProperty[] properties, ConfiguredReport config)
    {
        config.Range = valueRange != null ? GetConfigRange(valueRange) : null;
        if (string.IsNullOrEmpty(config.RangeId))
            config.RangeId = config.Range?.RangeId;
        else if (config.Range != null)
            config.Range.RangeId = config.RangeId;
        if (levels != null)
        {
            config.Levels = [];
            foreach (ValueRange level in levels)
                config.Levels.Add(GetConfigRange(level));
        }

        config.Properties = [];
        if (properties != null)
            foreach (PostedProperty property in properties)
                config.AddProperty(property.PropertyId, property.Value);
    }

    private static ConfiguredReport.ConfigRange GetConfigRange(ValueRange valueRange)
    {
        ConfiguredReport.ConfigRange result = new()
        {
            RangeId = Guid.NewGuid().ToString(),
            MinType = (ConfiguredReport.ConfigRange.ValueType)valueRange.MinType,
            MinValue = valueRange.MinValue,
            MaxType = (ConfiguredReport.ConfigRange.ValueType)valueRange.MaxType,
            MaxValue = valueRange.MaxValue,
            Color = valueRange.Color,
        };
        return result;
    }

    private static ConfiguredReport.eFieldSelectionType GetAggrType(string aggrId)
    {
        return aggrId switch
        {
            "Avg" or "Average" => ConfiguredReport.eFieldSelectionType.asAggregation_Average,
            "First" => ConfiguredReport.eFieldSelectionType.asAggregation_First,
            "Last" => ConfiguredReport.eFieldSelectionType.asAggregation_Last,
            "Max" => ConfiguredReport.eFieldSelectionType.asAggregation_Max,
            "Min" => ConfiguredReport.eFieldSelectionType.asAggregation_Min,
            "Mode" => ConfiguredReport.eFieldSelectionType.asAggregation_Mode,
            "StDev" or "SD" => ConfiguredReport.eFieldSelectionType.asAggregation_SD,
            "Sum" => ConfiguredReport.eFieldSelectionType.asAggregation_Sum,
            "Trend" => ConfiguredReport.eFieldSelectionType.asAggregation_Trend,
            "InColumn" or "Formula" => ConfiguredReport.eFieldSelectionType.asColumn,
            "GroupByItem" => ConfiguredReport.eFieldSelectionType.asGroupBy,
            _ => ConfiguredReport.eFieldSelectionType.asGroupBy,
        };
    }
}
