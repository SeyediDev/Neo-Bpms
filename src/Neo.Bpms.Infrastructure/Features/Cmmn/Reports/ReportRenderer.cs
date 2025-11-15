using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportRenderer
{
    public static object GetCelValue(ColumnFieldDefinition colInfo, ReportRowInfo row)
    {
        return GetCelValue(colInfo, row.Data);
    }

    public static object GetCelValue(ColumnFieldDefinition colInfo, ElasticObject data)
    {
        object value = GetValue(colInfo, data);
        if (value == null)
        {
            if (colInfo.aggrType == eAggregationFunctions.InColumn ||
                colInfo.aggrType == eAggregationFunctions.Formula)
            {
                value = " ";
            }
            else
            {
                value = 0;
            }
        }
        return value;
    }

    public static object GetValue(ColumnFieldDefinition colInfo, ElasticObject data)
    {
        if (data == null)
        {
            return colInfo.aggrType == eAggregationFunctions.InColumn ||
                colInfo.aggrType == eAggregationFunctions.Formula
                ? string.Empty
                : 0;
        }
        object value;
        if (colInfo.aggrType == eAggregationFunctions.InColumn ||
            colInfo.aggrType == eAggregationFunctions.Formula)
        {
            bool hasvalue = data.GetField(colInfo.ColumnTypeName, out value);
            if (!hasvalue)
                data.GetField(colInfo.ColumnTypeName + "Id", out value);
        }
        else
        {
            bool hasvalue = data.GetField(colInfo.ColumnTypeName, out value);
            if (!hasvalue)
            {
                data.GetField(colInfo.ColumnTypeName + "Id", out value);
            }
        }
        var decimalDigitsProperty = colInfo.GetProperty(eControlPropertyId.DecimalDigits);
        if (decimalDigitsProperty?.Value != null && value != null)
        {
            int decimalDigits = decimalDigitsProperty.Value.ToInt();
            if (decimalDigits >= 0)
            {
                try
                {
                    decimal numericValue = Convert.ToDecimal(value);
                    value = Math.Round(numericValue, decimalDigits);
                }
                catch
                {
                    // Ignore non-numeric values when rounding is requested.
                }
            }
        }
        return value;
    }
}
