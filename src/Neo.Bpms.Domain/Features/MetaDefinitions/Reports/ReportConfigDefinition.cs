using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredReport;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

/// <summary>
/// Base class to define reports and their details. All report definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract class ReportConfigDefinition : BaseModelingDefinition
{
    protected virtual string Name { get; }
    protected virtual ReportViewType ViewType => ReportViewType.List;
    protected virtual List<string>? Roles { get; }
    protected virtual string WhereCondition { get; }
    protected virtual string TableSuccess { get; }
    protected virtual string TableDanger { get; }
    protected virtual string TableInfo { get; }
    protected virtual string TableWarning { get; }
    protected virtual string TableActive { get; }

    public virtual bool DefineAll(Report r)
    {
        report = r;
        DefinePrimitive();
        DefineExtra();
        DefineColumns();
        DefineOrderBy();
        DefineFilter();
        DefineColumnsFormation();
        return true;
    }

    private void DefinePrimitive()
    {
        if (report == null)
        {
            return;
        }

        string id = GetType().Name;
        reportConfig = new ConfiguredReport(report, ViewType, id, Name)
        {
            IsMeta = true,
            Roles = Roles
        };
        if (report.MetaConfigures == null)
        {
            report.MetaConfigures = [];
        }

        report.MetaConfigures.Add(id, reportConfig);
        if (!string.IsNullOrEmpty(WhereCondition))
        {
            reportConfig.WhereCondition = WhereCondition;
        }
        if (!string.IsNullOrEmpty(TableSuccess))
        {
            AddConditionalFormatting(TableSuccess, "table-success");
        }
        if (!string.IsNullOrEmpty(TableDanger))
        {
            AddConditionalFormatting(TableDanger, "table-danger");
        }
        if (!string.IsNullOrEmpty(TableInfo))
        {
            AddConditionalFormatting(TableInfo, "table-info");
        }
        if (!string.IsNullOrEmpty(TableWarning))
        {
            AddConditionalFormatting(TableWarning, "table-warning");
        }
        if (!string.IsNullOrEmpty(TableActive))
        {
            AddConditionalFormatting(TableActive, "table-active");
        }
    }

    protected virtual void DefineExtra()
    {
    }

    protected virtual void DefineColumns()
    {
    }

    protected virtual void DefineOrderBy()
    {
    }

    protected virtual void DefineColumnsFormation()
    {
    }

    protected virtual void DefineFilter()
    {
    }

    /// <summary>
    /// Add column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void DisplayColumn(string fieldId, string alias = null,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        string[] fieldIds = fieldId.Split('.');
        if (fieldIds.Length > 2)
        {
            return;//todo more than one dot is not supported in this method
        }

        if (fieldIds.Length == 1)
        {
            AddField(eFieldSelectionType.asColumn, fieldId, alias, ReportMatrixType.Horizontal, properties);
        }
        else
        {
            EntityField associationField = report?.entity.GetField(fieldIds[0]);

            if (associationField == null)
            {
                return;
            }

            EntityField f = associationField.AssociationEntity.GetField(fieldIds[1]);
            if (f == null)
            {
                return;
            }

            AddIncludedField(eFieldSelectionType.asColumn, f.Id, associationField.AssociationEntity.Id,
                associationField.Id, alias, ReportMatrixType.Horizontal, properties);
        }
    }
    /// <summary>
    /// Add columns
    /// </summary>
    /// <param name="columns"></param>
    protected void DisplayColumns(params string[] columns)
    {
        foreach (string column in columns)
        {
            DisplayColumn(column);
        }
    }
    /// <summary>
    /// Add column formula
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void DisplayColumnFormula(string formula, string alias = null, 
        ReportMatrixType matrixType = ReportMatrixType.Horizontal,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        AddFormulaField(eFieldSelectionType.asColumn, null, formula, alias, matrixType, properties);
    }

    /// <summary>
    /// Add order by column
    /// </summary>
    /// <param name="field">field</param>
    /// <returns></returns>
    protected void OrderBy(string field)
    {
        reportConfig.SortFields = string.IsNullOrEmpty(reportConfig.SortFields)
            ? field
            : $"{reportConfig.SortFields},{field}";
    }
    protected void OrderByDesc(string field)
    {
        reportConfig.SortFields = string.IsNullOrEmpty(reportConfig.SortFields)
            ? $"{field} DESC"
            : $"{reportConfig.SortFields},{field} DESC";
    }

    /// <summary>
    /// set last column layout
    /// </summary>
    /// <param name="isTooltip"></param>
    /// <param name="associationName"></param>
    /// <param name="matrixType"></param>
    /// <param name="width"></param>
    /// <param name="hAlign"></param>
    /// <param name="vAlign"></param>
    /// <returns></returns>
    protected void LayoutColumn(bool isTooltip, string associationName,
        ReportMatrixType matrixType = ReportMatrixType.Horizontal,
        double width = 0, eHAlign hAlign = eHAlign.Center,
        eVAlign vAlign = eVAlign.Top)
    {
        // LayoutColumn should only apply to GroupByItem fields, not DisplayColumn fields
        // Find the GroupByItem field by fieldId (associationName is the fieldId)
        if (reportConfig == null || string.IsNullOrEmpty(associationName))
        {
            return;
        }

        // Find the GroupByItem field with the matching fieldId
        SelectedField groupByField = null;
        foreach (var field in reportConfig.Fields.Values)
        {
            if (field.type == eFieldSelectionType.asGroupBy &&
                field.fieldId == associationName)
            {
                groupByField = field;
                break;
            }
        }

        if (groupByField == null)
        {
            // Fallback to selectedField if it's a GroupByItem
            if (selectedField != null && selectedField.type == eFieldSelectionType.asGroupBy)
            {
                groupByField = selectedField;
            }
            else
            {
                return; // No matching GroupByItem field found
            }
        }

        groupByField.IsTooltip = isTooltip;
        groupByField.AssociationName = associationName;
        groupByField.MatrixType = matrixType;
        groupByField.width = width;
        groupByField.HAlign = hAlign;
        groupByField.VAlign = vAlign;
    }

    protected void AddFormulaField(eFieldSelectionType type,
        string fieldId, string formula, string alias, ReportMatrixType matrixType= ReportMatrixType.Horizontal,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        selectedField = reportConfig?.AddField(type,
            fieldId ?? $"formulaSjvs_{reportConfig.Fields.Count}", report.EntityId, null,
            alias, formula, false, matrixType);
        ApplySelectedFieldProperties(selectedField, properties);
    }

    protected void AddField(eFieldSelectionType type,
        string fieldId, string alias, ReportMatrixType matrixType = ReportMatrixType.Horizontal,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        selectedField = reportConfig?.AddField(type,
            fieldId, report.EntityId, null,
            alias, null, false, ReportMatrixType.Horizontal);
        if(selectedField==null)
        {
            AddFormulaField(type, fieldId, fieldId, alias, matrixType, properties);
        }
        else ApplySelectedFieldProperties(selectedField, properties);
    }

    protected void AddIncludedField(eFieldSelectionType type,
        string fieldId, string associationEntityId, string associationName, string alias, 
        ReportMatrixType matrixType = ReportMatrixType.Horizontal,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        selectedField = reportConfig?.AddField(type,
            fieldId, associationEntityId, associationName,
            alias, null, false, matrixType);
        ApplySelectedFieldProperties(selectedField, properties);
    }

    private static void ApplySelectedFieldProperties(SelectedField field,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties)
    {
        if (field == null || properties == null)
        {
            return;
        }

        foreach ((eControlPropertyId propertyId, object value) in properties)
        {
            field.AddProperty(propertyId, value);
        }
    }

    /// <summary>
    /// Add Selected Field Filter
    /// </summary>
    /// <param name="Operator">Operator</param>
    /// <param name="operand">Operand</param>
    /// <returns></returns>
    protected bool AddFieldFilter(ColumnFilterDefinition.eOperator Operator, string operand)
    {
        return selectedField?.AddFilter(Operator, operand) != null;
    }

    public void SetSubReports()
    {
        DefineSubReports();
    }
    protected virtual void DefineSubReports()
    {
    }

    /// <summary>
    /// Add Sub Report Config
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type">type</param>
    /// <param name="subReportId">sub Report</param>
    /// <param name="subConfigId"></param>
    /// <param name="viewType"></param>
    /// <returns></returns>
    protected bool AddSubReportConfig(string subConfigId, Report.SubReportType type, string subReportId)
    {
        return reportConfig != null && (subReportId == report.Id
            ? AddSubReportConfigFromThisReport(subConfigId, type)
            : AddSubReportConfigFromOtherReports(subConfigId, type, subReportId));
    }

    /// <summary>
    /// Register a sub report config by referencing its definition type.
    /// </summary>
    /// <typeparam name="TReportConfig">The report config definition class that should be used as the sub report.</typeparam>
    /// <param name="type">Sub report type (DrillDown / inline / ...).</param>
    /// <param name="subReportId">
    /// Optional report id. If not provided we consider the current report definition as the parent container.
    /// </param>
    protected bool AddSubReport<TReportConfig>(Report.SubReportType type = Report.SubReportType.DrillDown,
        string subReportId = null)
        where TReportConfig : ReportConfigDefinition
    {
        if (reportConfig == null || report == null)
        {
            throw new InvalidOperationException("Report configuration has not been defined yet.");
        }

        string targetReportId = subReportId ?? report.Id;
        if (string.IsNullOrWhiteSpace(targetReportId))
        {
            throw new InvalidOperationException("Target report id cannot be resolved for sub report registration.");
        }

        string subConfigId = typeof(TReportConfig).Name;
        return AddSubReportConfig(subConfigId, type, targetReportId);
    }

    private bool AddSubReportConfigFromOtherReports(string subConfigId, Report.SubReportType type, string subReportId)
    {
        Report.PossibleSubReport psr = report.PossibleSubReports.FirstOrDefault(p => p.SubReportId == subReportId);
        if (psr == null)
        {
            return false;
        }

        UiEntity psrEntity = ProjectDefinition.Project.GetEntity(psr.NamespaceId, psr.EntityId) as UiEntity;
        Report psrReport = psrEntity?.GetReport(subReportId);
        return psrReport != null && (!psrReport.MetaConfigures.TryGetValue(subConfigId, out ConfiguredReport configuredReport)
            ? throw new Exception("Config Not Found")
            : reportConfig.AddSubReport(type, configuredReport) != null);
    }

    private bool AddSubReportConfigFromThisReport(string subConfigId, Report.SubReportType type)
    {
        return !report.MetaConfigures.TryGetValue(subConfigId, out ConfiguredReport configuredReport)
            ? throw new Exception("Config Not Found")
            : reportConfig.AddSubReport(type, configuredReport) != null;
    }

    /// <summary>
    /// Add Conditional Formatting
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="className">Class Name</param>
    /// <param name="columnName">Column Name</param>
    /// <returns></returns>
    protected bool AddConditionalFormatting(string filter, string className, string columnName = null)
    {
        return reportConfig?.AddConditionalFormatting(filter, className, columnName) != null;
    }

    protected Report report { get; set; }
    protected ConfiguredReport reportConfig;
    protected SelectedField selectedField;

    protected enum AggregationType
    {
        Sum = eFieldSelectionType.asAggregation_Sum,
        Average = eFieldSelectionType.asAggregation_Average,
        Min = eFieldSelectionType.asAggregation_Min,
        Max = eFieldSelectionType.asAggregation_Max,
        First = eFieldSelectionType.asAggregation_First,
        Last = eFieldSelectionType.asAggregation_Last,
        SD = eFieldSelectionType.asAggregation_SD,
        Mode = eFieldSelectionType.asAggregation_Mode,
        Trend = eFieldSelectionType.asAggregation_Trend,
        Count = eFieldSelectionType.asAggregation_Count
    }
}
