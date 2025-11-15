using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

/// <summary>
/// Base class to define reports and their details. All report definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract class ReportConfigDefinition : BaseModelingDefinition
{
    protected virtual List<string>? Roles { get; }
    protected virtual string WhereCondition { get; }
    protected virtual string HavingCondition { get; }
    public bool DefineAll(Report r)
    {
        report = r;
        Identify();
        DefineGroupBy();
        DefineColumns();
        DefineOrderBy();
        DefineSubReports();
        DefineFilter();
        return true;
    }

    protected abstract void Identify();

    protected virtual void DefineGroupBy()
    {
    }

    protected virtual void DefineColumns()
    {
    }

    protected virtual void DefineOrderBy()
    {
    }

    protected virtual void DefineFormation()
    {
    }

    protected virtual void DefineSubReports()
    {
    }

    protected virtual void DefineFilter()
    {
    }

    /// <summary>
    /// Define Pre Configured Report
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="viewType"></param>
    /// <returns></returns>
    protected void DefineConfig(string name, ReportViewType viewType, Report.ChartType? chartType = null)
    {
        if (report == null)
        {
            return;
        }

        string id = GetType().Name;
        reportConfig = new ConfiguredReport(report, viewType, id, name)
        {
            IsMeta = true,
            Roles = Roles
        };
        if (report.MetaConfigures == null)
        {
            report.MetaConfigures = [];
        }

        report.MetaConfigures.Add(id, reportConfig);
        if (chartType != null)
        {
            reportConfig.ChartType = chartType.Value;
        }
        if(string.IsNullOrEmpty(WhereCondition))
        {
            SetWhereCondition(WhereCondition);
        }
        if (string.IsNullOrEmpty(HavingCondition))
        {
            SetHavingCondition(HavingCondition);
        }
    }

    /// <summary>
    /// Set Chart Type
    /// </summary>
    /// <param name="chartType">Chart Type</param>
    /// <returns></returns>
    protected bool SetChartType(Report.ChartType chartType)
    {
        if (reportConfig == null)
        {
            return false;
        }

        reportConfig.ChartType = chartType;
        return true;
    }

    /// <summary>
    /// Set filter
    /// </summary>
    /// <param name="whereCondition">field</param>
    /// <returns></returns>
    protected void SetWhereCondition(string whereCondition)
    {
        reportConfig.WhereCondition = whereCondition;
    }

    /// <summary>
    /// Set having filter
    /// </summary>
    /// <param name="havingCondition">field</param>
    /// <returns></returns>
    protected void SetHavingCondition(string havingCondition)
    {
        reportConfig.HavingCondition = havingCondition;
    }

    /// <summary>
    /// Add column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void DisplayColumn(string fieldId, string alias = null)
    {
        string[] fieldIds = fieldId.Split('.');
        if (fieldIds.Length > 2)
        {
            return;//todo more than one dot is not supported in this method
        }

        if (fieldIds.Length == 1)
        {
            AddField(ConfiguredReport.eFieldSelectionType.asColumn, fieldId, alias);
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

            AddIncludedField(ConfiguredReport.eFieldSelectionType.asColumn, f.Id, associationField.AssociationEntity.Id
                , associationField.Id, alias);
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
    protected void DisplayColumnFormula(string formula, string alias = null)
    {
        AddFormulaField(ConfiguredReport.eFieldSelectionType.asColumn, null, formula, alias);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="aggregationType"></param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Aggregation(string fieldId, AggregationType aggregationType, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AddField((ConfiguredReport.eFieldSelectionType)aggregationType, fieldId, alias, properties);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="aggregationType"></param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void AggregationFormula(string formula, AggregationType aggregationType, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AddFormulaField((ConfiguredReport.eFieldSelectionType)aggregationType, null, formula, alias, properties);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Sum(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Sum, alias, properties);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Average(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Average, alias, properties);
    }

    /// <summary>
    /// Add sum formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void SumFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Sum, alias, properties);
    }

    /// <summary>
    /// Add count column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Count(string fieldId = null, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        if (string.IsNullOrEmpty(fieldId))
        {
            fieldId = "*";
        }

        Aggregation(fieldId, AggregationType.Count, alias, properties);
    }

    /// <summary>
    /// Add sum formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void CountFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Count, alias, properties);
    }

    /// <summary>
    /// Add max column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Max(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Max, alias, properties);
    }

    /// <summary>
    /// Add max formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void MaxFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Max, alias, properties);
    }

    /// <summary>
    /// Add min column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Min(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Min, alias, properties);
    }

    /// <summary>
    /// Add min formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void MinFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Min, alias, properties);
    }

    /// <summary>
    /// Add group by column
    /// </summary>
    /// <param name="field">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void GroupBy(string field, string alias = null, bool addAsDisplayColumn=true)
    {
        string[] fieldIds = field.Split('.');
        if (fieldIds.Length > 2)
        {
            return;//todo more than one dot is not supported in this method
        }

        if (fieldIds.Length == 1)
        {
            AddField(ConfiguredReport.eFieldSelectionType.asGroupBy, field, alias);
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

            AddIncludedField(ConfiguredReport.eFieldSelectionType.asGroupBy, f.Id, associationField.AssociationEntity.Id
                , associationField.Id, alias);
        }
        if(addAsDisplayColumn)
        {
            DisplayColumn(field, alias);
        }
    }

    protected void GroupBys(params string[] fields)
    {
        foreach (string field in fields)
        {
            AddField(ConfiguredReport.eFieldSelectionType.asGroupBy, field, null);
        }
    }
    /// <summary>
    /// Add group by formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void GroupByFormula(string formula, string alias = null, bool addAsDisplayColumn = true)
    {
        AddFormulaField(ConfiguredReport.eFieldSelectionType.asGroupBy, null, formula, alias);
        if (addAsDisplayColumn)
        {
            AddFormulaField(ConfiguredReport.eFieldSelectionType.asColumn, selectedField?.fieldId, formula, alias);
        }
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
        ConfiguredReport.ReportMatrixType matrixType = ConfiguredReport.ReportMatrixType.Horizontal,
        double width = 0, ConfiguredReport.eHAlign hAlign = ConfiguredReport.eHAlign.Center,
        ConfiguredReport.eVAlign vAlign = ConfiguredReport.eVAlign.Top)
    {
        if (selectedField == null)
        {
            return;
        }

        selectedField.IsTooltip = isTooltip;
        selectedField.AssociationName = associationName;
        selectedField.MatrixType = matrixType;
        selectedField.width = width;
        selectedField.HAlign = hAlign;
        selectedField.VAlign = vAlign;
    }

    private void AddFormulaField(ConfiguredReport.eFieldSelectionType type,
        string fieldId, string formula, string alias,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        selectedField = reportConfig?.AddField(type,
            fieldId??$"formulaSjvs_{reportConfig.Fields.Count}", report.EntityId, null,
            alias, formula, false, ConfiguredReport.ReportMatrixType.Horizontal);
        ApplySelectedFieldProperties(selectedField, properties);
    }

    private void AddField(ConfiguredReport.eFieldSelectionType type,
        string fieldId, string alias,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        selectedField = reportConfig?.AddField(type,
            fieldId, report.EntityId, null,
            alias, null, false, ConfiguredReport.ReportMatrixType.Horizontal);
        ApplySelectedFieldProperties(selectedField, properties);
    }
    private void AddIncludedField(ConfiguredReport.eFieldSelectionType type,
        string fieldId, string associationEntityId, string associationName, string alias,
        IEnumerable<(eControlPropertyId propertyId, object value)> properties = null)
    {
        selectedField = reportConfig?.AddField(type,
            fieldId, associationEntityId, associationName,
            alias, null, false, ConfiguredReport.ReportMatrixType.Horizontal);
        ApplySelectedFieldProperties(selectedField, properties);
    }

    private static void ApplySelectedFieldProperties(ConfiguredReport.SelectedField field,
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
    protected ConfiguredReport.SelectedField selectedField;

    protected enum AggregationType
    {
        Sum = ConfiguredReport.eFieldSelectionType.asAggregation_Sum,
        Average = ConfiguredReport.eFieldSelectionType.asAggregation_Average,
        Min = ConfiguredReport.eFieldSelectionType.asAggregation_Min,
        Max = ConfiguredReport.eFieldSelectionType.asAggregation_Max,
        First = ConfiguredReport.eFieldSelectionType.asAggregation_First,
        Last = ConfiguredReport.eFieldSelectionType.asAggregation_Last,
        SD = ConfiguredReport.eFieldSelectionType.asAggregation_SD,
        Mode = ConfiguredReport.eFieldSelectionType.asAggregation_Mode,
        Trend = ConfiguredReport.eFieldSelectionType.asAggregation_Trend,
        Count = ConfiguredReport.eFieldSelectionType.asAggregation_Count
    }
}
