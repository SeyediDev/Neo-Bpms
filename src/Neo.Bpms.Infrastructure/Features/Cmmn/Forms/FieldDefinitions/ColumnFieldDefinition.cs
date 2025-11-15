using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;

public class ColumnFieldDefinition(string alias) : InputFieldDefinition(alias)
{
    public string entityId { get; set; }
    public string AssociationName { get; set; }
    public string ColumnName { get; set; }
    public string Formula { get; set; }
    public bool IsSorted => (SortOrder ?? 0) > 0;
    public bool Descending { get; set; }
    public int? SortOrder { get; set; }
    public bool IsTooltip { get; set; }
    public ConfiguredReport.ReportMatrixType? MatrixType { get; set; }
    public eAggregationFunctions aggrType = eAggregationFunctions.InColumn;

    public string AssociationPrefix =>
        string.IsNullOrEmpty(AssociationName) ? "" : AssociationName + ".";

    public string ColumnTypeName =>
        AssociationPrefix +
        (aggrType != eAggregationFunctions.GroupByItem &&
          aggrType != eAggregationFunctions.InColumn
            ? aggrType + "$"
            : "") +
        ColumnName;

    public string GetColumnOrderName()
    {
        string colName = ColumnName;
        // if (!string.IsNullOrEmpty(entityId) && ColumnName != "*")
        // {
        //     colName = $"{entityId}.{ColumnName}";
        // }

        string orderName = aggrType switch
        {
            eAggregationFunctions.AggregationFormula => "(" + Formula + ")",
            eAggregationFunctions.First => "Min(" + colName + ")",
            eAggregationFunctions.Last => "Max(" + colName + ")",
            eAggregationFunctions.Sum or eAggregationFunctions.Avg or eAggregationFunctions.Min or 
            eAggregationFunctions.Max or eAggregationFunctions.Count or eAggregationFunctions.StDev or 
            eAggregationFunctions.StDevP or eAggregationFunctions.Var or eAggregationFunctions.VarP or 
            eAggregationFunctions.CHECKSUM_AGG or eAggregationFunctions.GROUPING => $"{aggrType}(" + colName + ")",
            _ => colName,
        };
        return orderName.Replace(" ", "");
    }

    public string Property(eControlPropertyId propertyId) =>
        GetProperty(propertyId)?.Value?.ToString();

    public bool IsLongParam()
    {
        return FieldType switch
        {
            TVariableTypes.Char or TVariableTypes.UChar or TVariableTypes.Short or TVariableTypes.UShort or TVariableTypes.Int or TVariableTypes.Long or TVariableTypes.ULong or TVariableTypes.Decimal=> true,
            _ => false,
        };
    }

    public bool IsSummable => PropertyBoolean(eControlPropertyId.Summable);

    public string SummationMethod
    {
        get
        {
            return FieldType switch
            {
                TVariableTypes.Char or TVariableTypes.UChar or TVariableTypes.Short or TVariableTypes.UShort or TVariableTypes.Int or TVariableTypes.Long or TVariableTypes.ULong or TVariableTypes.Double or TVariableTypes.Decimal => "number",
                TVariableTypes.DurHourMinute or TVariableTypes.DayHourMinute => "duration",
                _ => string.Empty,
            };
        }
    }
}
