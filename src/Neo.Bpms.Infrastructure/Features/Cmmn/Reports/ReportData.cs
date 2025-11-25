using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportData
{
    public ReportData(ConfiguredReport config, ReportStructure structure,
        ElasticObject filterValues, bool shouldGiveQueries = false)
    {
        Config = config;
        Structure = structure;
        Structure.FilterValues = filterValues;
        QueryInfo = new QueryInfo(shouldGiveQueries);
    }

    public ReportStructure Structure { get; set; }
    public ConfiguredReport Config { get; set; }
    public IList<ReportRowInfo> Rows { get; set; } = [];

    public int RecordCount { get; set; }
    public ElasticObject TotalRecord { get; set; }

    public QueryInfo QueryInfo { get; }
    public int RecordsPerPage { get; set; }
    public readonly ErrorInformationList Errors = [];

    public IEnumerable<ColumnFieldDefinition> InColumns =>
        Structure.SelectedColumns.Where(
            col => col.aggrType == eAggregationFunctions.InColumn);

    public IEnumerable<ColumnFieldDefinition> AggregationColumns =>
        Structure.SelectedColumns.Where(col =>
            col.aggrType != eAggregationFunctions.GroupByItem &&
            col.aggrType != eAggregationFunctions.InColumn);
    public string GetMetricBoxValue()
    {
        double singleValue = 0;
        var singleColumnAlias = "";
        var row = Rows!=null && Rows.Count > 0 ? Rows[0] : null;
        var singleValueColumn = Structure.SelectedColumns.FirstOrDefault(s =>
            s.aggrType != eAggregationFunctions.GroupByItem &&
            s.aggrType !=
            eAggregationFunctions
                .InColumn); 
        if (singleValueColumn != null)
        {
            singleColumnAlias = singleValueColumn.Alias;

            singleValue = row == null
                ? 0
                : !string.IsNullOrEmpty(singleValueColumn.Formula)
                    ? Convert.ToDouble(row.Data[singleValueColumn.ColumnTypeName] ?? 0)
                    : row.Data.GetDouble(singleValueColumn.ColumnTypeName);
        }
        var culture = CultureInfo.InvariantCulture;
        var hasFraction = Math.Abs(singleValue % 1) > double.Epsilon;
        var format = hasFraction ? "#,0.00" : "#,0";
        return singleValue.ToString(format, culture);
    }
    public string GetReportKey() 
    {
        return Structure?.ConfigId ?? Structure?.Form_ReportId ?? Config?.ConfigId ?? Guid.NewGuid().ToString("N");
    }
}

public class ReportOrderInfo
{
    public string Alias { get; set; }
    public string ColumnName { get; set; }
    public bool Descending { get; set; }
    public bool OrderById { get; set; }

    public string EntityId { get; set; }
    public string AssociationName { get; set; }
    public bool FromPersistence { get; set; }
}

public class ReportRowInfo
{
    public ElasticObject Data { get; set; }
    public List<ReportData> SubReports { get; set; } = null;
}

public enum eExportType
{
    Word,
    PDF,
    Excel
}

public class ReportColumnFilterDefinition
{
    public string Operator { set; get; }
    public string Operand { set; get; }
}

public class ReportColumnFilter
{
    public string FieldName { set; get; }
    public ColumnFilterDefinition.eOperator Operator { set; get; }
    public string Operand { set; get; }
}

public class ReportSelectedGroupBy
{
    public string FieldName { get; set; }
}

public class SubReportConfigData
{
    public ReportViewType ViewType { get; set; }
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string DashboardId { get; set; }
    public string DashboardConfigId { get; set; }
}

public class ReportColumnDisplayEditor
{
    public string ColumnName { get; set; }
    public string aliasValue { get; set; }
    public string widthValue { get; set; }
    public string alignValue { get; set; }
    public string vAlignValue { get; set; }
    public string dirValue { get; set; }
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public string Ids { get; set; }
}

public class PossibleSubReportSelections
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public List<Report.PossibleSubReport> PossibleSubReports { get; set; }
    public List<ConfiguredReport.ConfiguredSubReport> SubReports { get; set; }

    public class DashboardConfiguration
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public List<DashboardConfiguration> DashboardConfigs { get; set; }
}

public class ReportColumnFilterSettings
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public string ColumnName { get; set; }
    public IList<ReportColumnFilterDefinition> FieldFilters { get; set; }
}

public class ReportConditionalFormatting
{
    public string ClassName { get; set; }
    public string Filter { get; set; }
}

public class ReportConditionalFormattingSettings
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
    public string ColumnName { get; set; }
    public bool IsColumn { get; set; }
    public IList<ReportConditionalFormatting> Formats { get; set; }
}

public class AddNewConfigParameters
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string NewConfigName { get; set; }
    public ReportViewType viewType { get; set; }
    public ChartType chartType { get; set; }
}

public class AddSubReportParameters
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string ReportId { get; set; }
    public string ConfigId { get; set; }
}

public class PostedOrderdColumn
{
    public string EntityId { get; set; }
    public string AssociationName { get; set; }
    public string FieldId { get; set; }
    public string AggrId { get; set; }
    public string Alias { get; set; }
    public string Formula { get; set; }
    public bool IsTooltip { get; set; }
    public ConfiguredReport.ReportMatrixType MatrixType { get; set; }
}

public class PostedProperty
{
    public long PropertyId { get; set; }
    public string Value { get; set; }
}

public class ValueRange
{
    public enum ValueType
    {
        Fixed = 1,
        Field
    }

    public ValueType MinType { get; set; }
    public ValueType MaxType { get; set; }
    public string MinValue { get; set; }
    public string MaxValue { get; set; }
    public string Color { get; set; }
}
