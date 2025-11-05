using Neo.Bpms.Domain.Models.Cmmn;

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
        return value;
    }
}