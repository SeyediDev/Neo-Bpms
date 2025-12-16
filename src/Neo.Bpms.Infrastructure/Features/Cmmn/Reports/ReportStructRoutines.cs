using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using CultureInfo = System.Globalization.CultureInfo;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportStructRoutines(FormStructRoutines formStructRoutines,
    ScheduledReportConfigBackupRestore scheduledReportConfigBackupRestore,
    ReportConfigBackupRestore reportConfigBackupRestore,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore,
    IEnsureAutoFolderGrouping ensureAutoFolderGrouping)
{
    public async Task<(ReportStructure reportStructure, int configRecordsPerPage)>
        GetReportStructure(ConfiguredReport config, string persistenceSortedFields, string culture, IdentityUser user)
    {
        Report report = config.Report;
        int configRecordsPerPage = config.RecordsPerPage;
        UiEntity entity = report.entity;
        ReportStructure structure = new() { ConfigId = config.ConfigId };
        if (formStructRoutines.GetCommonStructure(culture, structure, report, user, null, true) == null)
            return (null, configRecordsPerPage);

        structure.Name = config.Name;
        //config.Formats
        GetSelectionColumns(config, culture, entity, structure.SelectedColumns, out IList<ReportColumnFilter> selectedFilters, report);
        structure.SelectedFilters = selectedFilters;

        // Set ChartType from config, with fallback to Column if None or Line (default)
        // Line is the default value in ConfiguredReport, so we treat it as unset and use Column
        structure.ChartType = (config.ChartType != ChartType.None && config.ChartType != ChartType.Line) 
            ? config.ChartType 
            : ChartType.Column;
        structure.ReportViewType = config.ViewType;
        structure.GroupByViewType = config.GroupByViewType;

        if (config.ViewType == ReportViewType.GroupByList && config.GroupByViewType == GroupByViewType.Matrix)
            configRecordsPerPage = 10000;
        AddFieldsToStructure(structure, entity, report.reportFields, "", "", culture);
        AddIncludesToStructure(culture, report, entity, structure);
        AddCountColumnIfNeeded(structure, entity);
        AddSubReports(config, structure);
        SetOrderBy(config, persistenceSortedFields, structure, entity);
        SetFilter(config, structure);
        SetLevelAndRange(config, structure);
        SetProperties(config, structure);
        await SetOtherConfigs(user, structure, report);
        return (structure, configRecordsPerPage);
    }

    private async Task SetOtherConfigs(IdentityUser user, ReportStructure structure, Report report)
    {
        structure.Configs = [.. (await reportConfigBackupRestore.Configurations(report))
            .Where(cfg =>cfg.CheckAccess(user))];
        foreach (ConfiguredReport configuredReport in structure.Configs.Where(c => c.ScheduledReports == null))
        {
            configuredReport.ScheduledReports =
                [.. await scheduledReportConfigBackupRestore.Configurations(configuredReport.ConfigId)];
        }

        var configuredFilters = await filterConfigBackupRestore.Configurations("Report", report.NamespaceId, report.entity.Id, report.Id);
        structure.ConfiguredFilters = configuredFilters?.Where(cfg =>
                cfg.CheckAccess(user))
            .ToList();
        var configuredFolders = await folderConfigBackupRestore.Configurations("Report", report.NamespaceId, report.entity.Id, report.Id);
        structure.ConfiguredFolders = configuredFolders?.Where(cfg =>
                cfg.CheckAccess(user))
            .ToList();

        // Auto-folder grouping for reports with more than 6 configs
        await ensureAutoFolderGrouping.Run(structure, report, user);

        structure.UserGroups = [.. user.Roles.Values.Where(role => report.CheckRole(role.Code))];
    }



    private static void SetFilter(ConfiguredReport config, ReportStructure structure)
    {
        structure.HavingConstraint = config.HavingCondition;
        structure.Constraint = config.WhereCondition;
    }

    private static void SetOrderBy(ConfiguredReport config, string persistenceSortedFields, ReportStructure structure,
        UiEntity entity)
    {
        int sortOrderIndex = 1;
        ParseSortedFields(structure, entity, persistenceSortedFields, ref sortOrderIndex, true);
        ParseSortedFields(structure, entity, config.SortFields, ref sortOrderIndex, false);
    }

    private static void SetLevelAndRange(ConfiguredReport config, ReportStructure structure)
    {
        if (config.Levels != null)
        {
            structure.Levels = [];
            foreach (ConfiguredReport.ConfigRange range in config.Levels)
            {
                structure.Levels.Add(GetRange(range));
            }
        }

        if (config.Range != null)
        {
            structure.ValueRange = GetRange(config.Range);
        }
    }

    private static void SetProperties(ConfiguredReport config, ReportStructure structure)
    {
        if (config.Properties != null)
        {
            structure.Properties = [];
            foreach (ConfiguredReport.ConfigProperty property in config.Properties)
            {
                structure.Properties.Add(new PostedProperty
                {
                    PropertyId = property.PropertyId,
                    Value = property.Value
                });
            }
        }
    }

    private static void AddSubReports(ConfiguredReport config, ReportStructure structure)
    {
        foreach (KeyValuePair<string, ConfiguredReport.ConfiguredSubReport> item in config.SubReports)
        {
            structure.SubReports ??= [];
            structure.SubReports.Add(item.Value);
        }
    }

    private static void AddCountColumnIfNeeded(ReportStructure structure, UiEntity entity)
    {
        if (structure.ReportViewType == ReportViewType.GroupByList ||
            structure.ReportViewType == ReportViewType.Chart)
        {
            ColumnFieldDefinition col = AddColumn(eAggregationFunctions.Count,
                CulturalTexts.Count,
                structure, entity, "", null, null);
            col.ColumnName = "*";
            col.FieldType = TVariableTypes.Long;
        }
    }

    private static void AddIncludesToStructure(string culture, Report report, UiEntity entity, ReportStructure structure)
    {
        foreach (IncludeEntity inc in report.Includes ?? Enumerable.Empty<IncludeEntity>())
        {
            string[] associationItems = inc.AssociationId.Split('.');
            int ia = 0;
            Entity incEntity = entity;
            string associationAlias = "";
            foreach (string associationItem in associationItems)
            {
                EntityField associationField = incEntity.GetField(associationItem);
                if (associationField?.AssociationEntity != null)
                {
                    associationAlias += (!string.IsNullOrEmpty(associationAlias) ? associationAlias + "-" : "") +
                                        associationField.Name + " ";
                    if (ia == associationItems.Length - 1)
                        AddFieldsToStructure(structure, associationField.AssociationEntity.Entity(), inc.reportFields,
                            inc.AssociationId, associationAlias, culture);
                    incEntity = associationField.AssociationEntity.Entity();
                }

                ia++;
            }
        }
    }

    private static void ParseSortedFields(ReportStructure structure, UiEntity entity, string sortedFields,
        ref int sortOrderIndex, bool isFromPersistence)
    {
        if (sortedFields?.StartsWith("#") ?? false)
            sortedFields = sortedFields[1..];
        string[] sortFieldsList = string.IsNullOrEmpty(sortedFields) ? null : sortedFields.Split('#');
        if (sortFieldsList == null) return;
        foreach (string sortField in sortFieldsList)
        {
            if (string.IsNullOrEmpty(sortField)) continue;
            string[] sortFieldItems = sortField.Split(' ');
            string sortFieldEntityId = sortFieldItems.Length > 2 ? sortFieldItems[2] : entity.Id;
            bool orderById = sortFieldItems.Length > 3 && ConvUtill.ToBoolean(sortFieldItems[3]);
            string associationName = sortFieldItems.Length > 4 ? sortFieldItems[4] : "";
            if (associationName == "undefined")
                associationName = "";
            string sortFieldFormula = "";
            if (!string.IsNullOrEmpty(sortFieldItems[0]) && sortFieldItems[0].Contains('$'))
                sortFieldFormula = sortFieldItems[0].Split('$')[1];
            List<ColumnFieldDefinition> cells = structure.SelectedColumns.Where(
                c => c.aggrType != eAggregationFunctions.GroupByItem).ToList();
            List<ColumnFieldDefinition> cols = structure.ColumnInfos.Where(
                c => c.aggrType != eAggregationFunctions.GroupByItem).ToList();
            ColumnFieldDefinition col =
                cells.FirstOrDefault(
                    c => c.entityId + "." + c.GetColumnOrderName() ==
                         sortFieldEntityId + "." + sortFieldItems[0]) ??
                cols.FirstOrDefault(
                    c => c.entityId + "." + c.GetColumnOrderName() ==
                         sortFieldEntityId + "." + sortFieldItems[0]) ??
                cells.FirstOrDefault(
                    c => c.entityId + "." + c.ColumnTypeName == sortFieldEntityId + "." + sortFieldItems[0]) ??
                cells.FirstOrDefault(
                    c => c.ColumnTypeName == sortFieldItems[0]) ??
                cols.FirstOrDefault(
                    c => c.entityId + "." + c.ColumnTypeName == sortFieldEntityId + "." + sortFieldItems[0]) ??
                cells.FirstOrDefault(
                    c => c.entityId + "." + c.Formula == sortFieldEntityId + "." + sortFieldFormula);
            if (col == null)
                continue;
            col.SortOrder = sortOrderIndex;
            sortOrderIndex++;
            if (sortFieldItems.Length > 1 && sortFieldItems[1].ToUpper() == "DESC")
                col.Descending = true;

            if (structure.OrderInfos.All(o => o.ColumnName != col.GetColumnOrderName()))
                structure.OrderInfos.Add(new ReportOrderInfo
                {
                    ColumnName = col.GetColumnOrderName(),
                    Descending = col.Descending,
                    EntityId = sortFieldEntityId,
                    AssociationName = associationName,
                    OrderById = orderById,
                    FromPersistence = isFromPersistence,
                });
        }
    }

    private static ValueRange GetRange(ConfiguredReport.ConfigRange range)
    {
        return new ValueRange
        {
            MinType = (ValueRange.ValueType)range.MinType,
            MaxType = (ValueRange.ValueType)range.MaxType,
            MinValue = range.MinValue,
            MaxValue = range.MaxValue,
            Color = range.Color,
        };
    }

    private static void GetSelectionColumns(ConfiguredReport config, string culture, UiEntity entity,
        IList<ColumnFieldDefinition> selectedColumns, out IList<ReportColumnFilter> selectedFilters, Report report)
    {
        selectedFilters = [];
        foreach (ConfiguredReport.SelectedField fld in config.Fields.Values.OrderBy(f => f.Order))
        {
            string alias = fld.Alias;
            Entity entity1 = entity.Id == fld.entityId ? entity : ProjectDefinition.Project.GetEntityByEntityId(fld.entityId);
            if (entity1 == null) continue;
            string fieldId = fld.fieldId ?? fld.Alias;
            EntityField entityField = entity1.GetField(fieldId);
            TVariableTypes colType = TVariableTypes.Double;
            report.reportFields.TryGetValue(fieldId, out Report.Field reportField);
            if (fieldId == "*")
                alias = "";
            else
            {
                if (entityField == null)
                {
                    if (!string.IsNullOrEmpty(fld.formula))
                    {
                        ColumnFieldDefinition col = null;
                        switch (fld.type)
                        {
                            case ConfiguredReport.eFieldSelectionType.asColumn:
                                col = AddSelectedColumn(eAggregationFunctions.InColumn,
                                fld.Alias ?? "" + (alias ?? "فرمول"),
                                colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asGroupBy:
                                col = AddSelectedColumn(eAggregationFunctions.GroupByItem,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asFilter:
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAscending:
                                break;
                            case ConfiguredReport.eFieldSelectionType.asDescending:
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation:
                                col = AddSelectedColumn(eAggregationFunctions.AggregationFormula,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Sum:
                                col = AddSelectedColumn(eAggregationFunctions.Sum,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Average:
                                col = AddSelectedColumn(eAggregationFunctions.Avg,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Min:
                                col = AddSelectedColumn(eAggregationFunctions.Min,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Max:
                                col = AddSelectedColumn(eAggregationFunctions.Max,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_First:
                                col = AddSelectedColumn(eAggregationFunctions.First,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Last:
                                col = AddSelectedColumn(eAggregationFunctions.Last,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_SD:
                                col = AddSelectedColumn(eAggregationFunctions.StDev,//TODO
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Mode:
                                col = AddSelectedColumn(eAggregationFunctions.StDevP,//TODO
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Trend:
                                col = AddSelectedColumn(eAggregationFunctions.Var,//TODO
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                            case ConfiguredReport.eFieldSelectionType.asAggregation_Count:
                                col = AddSelectedColumn(eAggregationFunctions.AggregationFormula,
                                    fld.Alias ?? "" + (alias ?? "فرمول"),
                                    colType, null, fld, selectedColumns, reportField);
                                col.Formula = fld.formula;
                                break;
                        }
                    }

                    continue;
                }

                colType = entityField.FieldType;
                if (string.IsNullOrEmpty(alias))
                    alias = culture == "en" ? entityField.EnName : entityField.Name;
            }

            if (entityField?.Formula != null && fld.type != ConfiguredReport.eFieldSelectionType.asGroupBy)
            {
                if (config.ViewType == ReportViewType.List && entityField.Formula.UsedForAggregationOnly)
                    continue;
            }

            switch (fld.type)
            {
                case ConfiguredReport.eFieldSelectionType.asAggregation_Average:
                    AddSelectedColumn(eAggregationFunctions.Avg,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.AverageOf, alias),
                        colType, entityField, fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_First:
                    AddSelectedColumn(eAggregationFunctions.First,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.FirstOf, alias),
                        colType, entityField, fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_Last:
                    AddSelectedColumn(eAggregationFunctions.Last,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.LastOf, alias), colType,
                        entityField, fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_Max:
                    AddSelectedColumn(eAggregationFunctions.Max,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.MaxOf, alias), colType,
                        entityField, fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_Min:
                    AddSelectedColumn(eAggregationFunctions.Min,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.MinOf, alias), colType, entityField,
                        fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_SD:
                    AddSelectedColumn(eAggregationFunctions.StDev,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.StandardDeviationOf, alias), colType,
                        entityField, fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_Sum:
                    AddSelectedColumn(eAggregationFunctions.Sum,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.SumOf, alias), colType, entityField,
                        fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asAggregation_Count:
                    AddSelectedColumn(eAggregationFunctions.Count,
                        fld.Alias ?? DefaultAggregationAlias(CulturalTexts.CountOf, alias), colType,
                        entityField, fld, selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asColumn:
                    ColumnFieldDefinition col = AddSelectedColumn(eAggregationFunctions.InColumn, alias, colType, entityField, fld,
                        selectedColumns, reportField);
                    if (report.reportFields.TryGetValue(fld.fieldId, out Report.Field value))
                    {
                        Report.Field columnField = value;
                        if (columnField.properties != null)
                        {
                            foreach (FormProperty item in columnField.properties)
                                col.AddProperty(item.id, item.value);
                        }
                    }

                    break;
                case ConfiguredReport.eFieldSelectionType.asGroupBy:
                    AddSelectedColumn(eAggregationFunctions.GroupByItem, alias, colType, entityField, fld,
                        selectedColumns, reportField);
                    break;
                case ConfiguredReport.eFieldSelectionType.asFilter:
                    break;
            }

            if (fld.FieldFilters != null)
            {
                foreach (ColumnFilterDefinition fieldFilter in fld.FieldFilters)
                {
                    //todo بررسی شود 
                    selectedFilters.Add(new ReportColumnFilter
                    {
                        FieldName = fld.fieldId,
                        Operand = fieldFilter.Operand,
                        Operator = fieldFilter.Operator
                    });
                }
            }
        }
    }

    private static string DefaultAggregationAlias(string aggregation, string alias)
    {
        return $"{aggregation} {alias}";
    }

    private static ColumnFieldDefinition AddSelectedColumn(eAggregationFunctions aggregationFunction,
        string alias, TVariableTypes colType,
        EntityField entityField,
        ConfiguredReport.SelectedField selectedField,
        IList<ColumnFieldDefinition> selectedColumns,
        Report.Field reportField)
    {
        ColumnFieldDefinition col = new(alias)
        {
            aggrType = aggregationFunction,
            IsTooltip = selectedField.IsTooltip,
            MatrixType = selectedField.MatrixType,
            ColumnName = selectedField.fieldId,
            FieldType = colType,
            entityId = selectedField.entityId,
            AssociationName = selectedField.AssociationName,
        };
        FillProperties(entityField, reportField, col, selectedField);
        selectedColumns.Add(col);
        return col;
    }

    private static void AddFieldsToStructure(ReportStructure result, Entity entity,
        Report.ReportFields reportFields,
        string associationName, string associationAlias, string culture)
    {
        foreach (Report.Field fld in reportFields.Values)
        {
            EntityField entityField = entity.GetField(fld.fieldId);
            if (entityField == null)
            {
                if (!string.IsNullOrEmpty(fld.formula))
                {
                    ColumnFieldDefinition col = AddColumn(
                        fld.asGroupBy ? eAggregationFunctions.GroupByItem : eAggregationFunctions.InColumn,
                        (!string.IsNullOrEmpty(associationAlias) ? associationAlias + "-" : "") + fld.alias,
                        result, entity, associationName, fld, null);
                    col.Formula = fld.formula;
                }

                continue;
            }

            string alias = (!string.IsNullOrEmpty(associationAlias) ? associationAlias + "-" : "") +
                        (culture == "en" ? entityField.EnName : entityField.Name);
            string fldAlias = fld.alias ?? alias;
            if (entityField.Formula != null)
            {
                if (!fld.asAggregation && entityField.Formula.UsedForAggregationOnly)
                    continue;
            }

            if (result.ReportViewType == ReportViewType.List && fld.asColumn)
            {
                AddColumn(eAggregationFunctions.InColumn, fldAlias, result, entity, associationName, fld,
                    entityField);
            }

            if ((result.ReportViewType == ReportViewType.GroupByList ||
                 result.ReportViewType == ReportViewType.Chart) && fld.asGroupBy)
            {
                AddColumn(eAggregationFunctions.GroupByItem, fldAlias, result, entity, associationName, fld,
                    entityField);
            }

            if (result.ReportViewType != ReportViewType.GroupByList &&
                result.ReportViewType != ReportViewType.Chart || !fld.asAggregation)
                continue;
            switch (entityField.FieldType)
            {
                case TVariableTypes.Char:
                case TVariableTypes.Short:
                case TVariableTypes.Int:
                case TVariableTypes.Long:
                case TVariableTypes.Decimal:
                case TVariableTypes.UChar:
                case TVariableTypes.UShort:
                case TVariableTypes.ULong:
                case TVariableTypes.Double:
                //case TVariableTypes.varStringListItem:
                case TVariableTypes.HourMinute:
                case TVariableTypes.DayHourMinute:
                case TVariableTypes.DoubleMinuteSecond:
                //case TVariableTypes.varBOOL:
                case TVariableTypes.DurHourMinute:
                    if (entityField.Formula != null && entityField.Formula.UsedForAggregationOnly)
                    {
                        AddColumn(eAggregationFunctions.AggregationFormula, fld.alias ?? alias, result, entity,
                            associationName, fld, entityField);
                    }
                    else
                    {
                        AddColumn(eAggregationFunctions.Sum, fld.alias ?? DefaultAggregationAlias(CulturalTexts.SumOf, alias), result, entity,
                            associationName, fld, entityField);
                        AddColumn(eAggregationFunctions.Avg, fld.alias ?? DefaultAggregationAlias(CulturalTexts.AverageOf, alias), result, entity,
                            associationName, fld, entityField);
                        AddColumn(eAggregationFunctions.Min, fld.alias ?? DefaultAggregationAlias(CulturalTexts.MinOf, alias), result, entity,
                            associationName, fld, entityField);
                        AddColumn(eAggregationFunctions.Max, fld.alias ?? DefaultAggregationAlias(CulturalTexts.MaxOf, alias), result, entity,
                            associationName, fld, entityField);
                        AddColumn(eAggregationFunctions.StDev, fld.alias ?? DefaultAggregationAlias(CulturalTexts.StandardDeviationOf, alias), result, entity,
                            associationName, fld, entityField);
                    }

                    break;
                case TVariableTypes.Date:
                case TVariableTypes.DateTime:
                    if (result.ReportViewType != ReportViewType.Chart)
                    {
                        AddColumn(eAggregationFunctions.Min, fld.alias ?? "کمینه " + alias, result, entity,
                            associationName, fld, entityField);
                        AddColumn(eAggregationFunctions.Max, fld.alias ?? "بیشینه " + alias, result, entity,
                            associationName, fld, entityField);
                    }

                    //AddColumn(eAggregationFunctions.Avg, fld.alias ?? "متوسط " + alias, result, entity, associationName, fld, entityField);
                    //AddColumn(eAggregationFunctions.StDev, fld.alias ?? "انحراف معیار " + alias, result, entity, associationName, fld, entityField);
                    break;
                default:
                    if (result.ReportViewType != ReportViewType.Chart)
                    {
                        AddColumn(eAggregationFunctions.First, fld.alias ?? DefaultAggregationAlias(CulturalTexts.FirstOf, alias), result, entity,
                            associationName, fld, entityField);
                        AddColumn(eAggregationFunctions.Last, fld.alias ?? DefaultAggregationAlias(CulturalTexts.LastOf, alias), result, entity,
                            associationName, fld, entityField);
                    }

                    break;
            }

            //foreach (var fieldFilter in fld.FieldFilters)
            //{
            //	if (result.SelectedFilters == null)
            //		result.SelectedFilters = new List<ReportSelectedFilter>();
            //	result.SelectedFilters.Add(new ReportSelectedFilter() { FilterName = fld.fieldId, Operand = fieldFilter.Operand, Operator = fieldFilter.Operator });
            //}
        }
    }

    private static void FillProperties(EntityField entityField,
        Report.Field reportField, FormFieldDefinition col,
        ConfiguredReport.SelectedField selectedField = null)
    {
        if (entityField?.Properties != null)
        {
            foreach (FieldProperty prop in entityField.Properties)
            {
                try
                {
                    switch (prop.PropertyId)
                    {
                        case EntityFieldPropertyId.DisplayFields:
                            col.AddProperty(eControlPropertyId.DisplayFields, prop.Value);
                            break;
                        case EntityFieldPropertyId.Multiple:
                            col.AddProperty(eControlPropertyId.IsMultiple, prop.Value);
                            break;
                        case EntityFieldPropertyId.UseFileServer:
                            col.AddProperty(eControlPropertyId.UseFileServer, prop.Value);
                            break;
                        case EntityFieldPropertyId.ShowDocumentInPage:
                            col.AddProperty(eControlPropertyId.ShowDocumentInPage, prop.Value);
                            break;
                        case EntityFieldPropertyId.DocumentType:
                            col.AddProperty(eControlPropertyId.DocumentType, prop.Value);
                            break;
                        default:
                            col.AddProperty((eControlPropertyId)prop.PropertyId, prop.Value);
                            break;
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        if (reportField?.properties != null)
        {
            foreach (FormProperty item in reportField.properties)
                col.AddProperty(item.id, item.value);
        }

        if (selectedField?.Properties != null)
        {
            foreach (FormProperty property in selectedField.Properties)
            {
                col.AddProperty(property.PropertyId, property.Value);
            }
        }
    }

    private static ColumnFieldDefinition AddColumn(eAggregationFunctions aggregationFunction, string alias,
        ReportStructure result, Entity entity, string associationName,
        Report.Field reportField, EntityField entityField)
    {
        ColumnFieldDefinition col = new(alias)
        {
            aggrType = aggregationFunction,
            entityId = entity.Id,
            AssociationName = associationName,
            ColumnName = reportField?.fieldId,
            FieldType = entityField?.FieldType ?? TVariableTypes.Double
        };
        result.ColumnInfos.Add(col);
        FillProperties(entityField, reportField, col);
        return col;
    }

    public static ReportColumnFilterSettings GetColumnFilterSettings(string namespaceId, string entityId,
        string reportId, string configId, ConfiguredReport.SelectedField field)
    {
        ReportColumnFilterSettings filterSettings = new()
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            ReportId = reportId,
            ConfigId = configId
        };

        if (field.FieldFilters == null) return filterSettings;
        filterSettings.FieldFilters = [];
        foreach (ColumnFilterDefinition filter in field.FieldFilters)
        {
            filterSettings.FieldFilters.Add(new ReportColumnFilterDefinition
            {
                Operand = filter.Operand,
                Operator = filter.Operator.ToString()
            });
        }

        return filterSettings;
    }

    public static void SetColumnFilterSetting(ReportColumnFilterSettings filterSettings,
        ConfiguredReport.SelectedField field)
    {
        field.FieldFilters = [];
        if (filterSettings.FieldFilters == null) return;
        foreach (ReportColumnFilterDefinition filter in filterSettings.FieldFilters)
        {
            field.FieldFilters.Add(new ColumnFilterDefinition
            {
                Operand = filter.Operand,
                Operator = GetOperator(filter.Operator)
            });
        }
    }

    private static ColumnFilterDefinition.eOperator GetOperator(string Operator)
    {
        return Operator switch
        {
            "EndsWith" => ColumnFilterDefinition.eOperator.EndsWith,
            "Equal" => ColumnFilterDefinition.eOperator.Equal,
            "GreaterEqual" => ColumnFilterDefinition.eOperator.GreaterEqual,
            "GreaterThan" => ColumnFilterDefinition.eOperator.GreaterThan,
            "Includes" => ColumnFilterDefinition.eOperator.Includes,
            "LessEqual" => ColumnFilterDefinition.eOperator.LessEqual,
            "LessThan" => ColumnFilterDefinition.eOperator.LessThan,
            "NotEqual" => ColumnFilterDefinition.eOperator.NotEqual,
            "NotIncludes" => ColumnFilterDefinition.eOperator.NotIncludes,
            "StartsWith" => ColumnFilterDefinition.eOperator.StartsWith,
            _ => ColumnFilterDefinition.eOperator.Equal,
        };
    }

    public static ReportConditionalFormattingSettings GetFormatSetting(string namespaceId, string entityId,
        string reportId, string configId, string columnName, bool isColumn,
        ConfiguredReport config)
    {
        ReportConditionalFormattingSettings formatSetting = new()
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            ReportId = reportId,
            ConfigId = configId,
            ColumnName = columnName
        };

        List<ConditionalFormatting> formats = config.Formats?.Where(f => f.ColumnName == columnName).ToList();
        if (formats == null || formats.Count == 0) return formatSetting;
        formatSetting.Formats = [];
        foreach (ConditionalFormatting format in formats)
        {
            if (isColumn && format.Type != ConditionalFormatting.eType.Column) continue;
            if (!isColumn && format.Type != ConditionalFormatting.eType.Row) continue;
            if (columnName != format.ColumnName) continue;
            ReportConditionalFormatting fmt = new() { ClassName = format.ClassName, Filter = format.Filter };
            formatSetting.Formats.Add(fmt);
        }

        return formatSetting;
    }

    public static void SetFormatSetting(ReportConditionalFormattingSettings formatSetting, ConfiguredReport config)
    {
        config.Formats?.RemoveAll(f => f.ColumnName == formatSetting.ColumnName);
        if (formatSetting.Formats.Count <= 0) return;
        config.Formats ??= [];
        foreach (ReportConditionalFormatting format in formatSetting.Formats)
        {
            ConditionalFormatting fmt = new()
            {
                ClassName = format.ClassName,
                ColumnName = formatSetting.ColumnName,
                Filter = format.Filter,
                Type =
                    formatSetting.IsColumn
                        ? ConditionalFormatting.eType.Column
                        : ConditionalFormatting.eType.Row
            };
            config.Formats.Add(fmt);
        }
    }

    public static void SetColumnProperties(string aliasValue, string widthValue, string hAlignValue,
        string vAlignValue, string direction, ConfiguredReport.SelectedField field)
    {
        field.Alias = aliasValue;
        _ = double.TryParse(widthValue.Replace("/", "."), out double width);
        field.width = width;
        if (hAlignValue == "left")
            field.HAlign = ConfiguredReport.eHAlign.Left;
        else if (hAlignValue == "right")
            field.HAlign = ConfiguredReport.eHAlign.Right;
        else
            field.HAlign = ConfiguredReport.eHAlign.Center;
        if (vAlignValue == "top")
            field.VAlign = ConfiguredReport.eVAlign.Top;
        else if (vAlignValue == "bottom")
            field.VAlign = ConfiguredReport.eVAlign.Bottom;
        else
            field.VAlign = ConfiguredReport.eVAlign.Middle;
        field.ltrDirection = direction == "ltr";
    }

    public static ReportColumnDisplayEditor GetColumnProperties(ConfiguredReport.SelectedField field)
    {
        ReportColumnDisplayEditor result = new()
        {
            aliasValue = field.Alias,
            alignValue = field.HAlign.ToString(),
            vAlignValue = field.VAlign.ToString(),
            widthValue = field.width.ToString(CultureInfo.CurrentCulture),
            dirValue = field.ltrDirection ? "ltr" : "rtl"
        };
        return result;
    }


    public static async Task<PossibleSubReportSelections> GetPossibleSubReportSelections(
        string namespaceId, string entityId, string reportId, string configId,
        ConfiguredReport config, Report.SubReportType type, DashboardConfigBackupRestore dashboardConfigBackupRestore)
    {
        PossibleSubReportSelections possibleSubReportSelections = new()
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            ReportId = reportId,
            ConfigId = configId,
            PossibleSubReports = [],
            DashboardConfigs = []
        };

        UiEntity entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        foreach (Dashboard dashboard in entity?.GetDashboards()?.Values ?? [])
        {
            List<ConfiguredDashboard> configurations = await dashboardConfigBackupRestore.Configurations(dashboard);
            foreach (ConfiguredDashboard configuration in configurations)
            {
                possibleSubReportSelections.DashboardConfigs.Add(
                    new PossibleSubReportSelections.DashboardConfiguration
                    {
                        Id = configuration.ConfigId,
                        Name = configuration.Name,
                    });
            }
        }

        switch (config.ViewType)
        {
            case ReportViewType.List:
                if (config.Report.PossibleSubReports != null)
                {
                    foreach (Report.PossibleSubReport psr in config.Report.PossibleSubReports)
                    {
                        UiEntity uiEntity =
                            ProjectDefinition.Project.GetEntity(psr.NamespaceId, psr.EntityId) as UiEntity;
                        Report subReport = uiEntity?.GetReport(psr.SubReportId);
                        if (subReport == null) continue;
                        psr.Name = subReport.Name;
                        psr.Type = type;
                        possibleSubReportSelections.PossibleSubReports.Add(psr);
                    }
                }

                break;
            case ReportViewType.GroupByList:
                possibleSubReportSelections.PossibleSubReports.Add(new Report.PossibleSubReport
                {
                    EntityId = entityId,
                    NamespaceId = namespaceId,
                    AssociationName = null,
                    SubReportId = config.Report.Id,
                    Type = Report.SubReportType.SubReport,
                    Name = CulturalTexts.SubReportOfThisEntityForEachRow
                });
                break;
            case ReportViewType.Chart:
                possibleSubReportSelections.PossibleSubReports.Add(new Report.PossibleSubReport
                {
                    EntityId = entityId,
                    NamespaceId = namespaceId,
                    AssociationName = null,
                    SubReportId = config.Report.Id,
                    Name = CulturalTexts.SubReportOfThisEntityForEachChartRow,
                    Type = Report.SubReportType.DrillDown
                });
                break;
            case ReportViewType.Dashboard:
                break;
        }

        return possibleSubReportSelections;
    }
}
