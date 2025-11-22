using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;
using Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

namespace Neo.Bpms.UI.MVC.Controls;

/// <summary>
/// Helper class for calculating aggregations on grouped report rows
/// This class is designed to be testable
/// </summary>
public static class AggregationCalculator
{
    /// <summary>
    /// Calculates aggregation value for a group of rows based on aggregation type
    /// </summary>
    /// <param name="aggrColumn">The aggregation column definition</param>
    /// <param name="rows">List of rows in the group</param>
    /// <param name="getValue">Function to extract value from a row for the aggregation column</param>
    /// <returns>Calculated aggregation value</returns>
    public static double Calculate(
        ColumnFieldDefinition aggrColumn,
        IReadOnlyList<ReportRowInfo> rows,
        Func<ColumnFieldDefinition, ReportRowInfo, double> getValue)
    {
        if (rows == null || rows.Count == 0)
            return 0;

        // If only one row, return its value directly
        if (rows.Count == 1)
            return getValue(aggrColumn, rows[0]);

        // Calculate aggregation based on aggregation type
        return aggrColumn.aggrType switch
        {
            eAggregationFunctions.Count => 
                // For Count, sum all count values in the group (each row may represent multiple records)
                rows.Sum(row => getValue(aggrColumn, row)),

            eAggregationFunctions.Sum => 
                // For Sum, sum all values in the group
                rows.Sum(row => getValue(aggrColumn, row)),

            eAggregationFunctions.Avg => 
                // For Average, calculate average of all values in the group
                CalculateAverage(rows, row => getValue(aggrColumn, row)),

            eAggregationFunctions.Max => 
                // For Max, return maximum value in the group
                rows.Max(row => getValue(aggrColumn, row)),

            eAggregationFunctions.Min => 
                // For Min, return minimum value in the group
                rows.Min(row => getValue(aggrColumn, row)),

            _ => 
                // For other aggregation types, return the first row's value
                getValue(aggrColumn, rows.FirstOrDefault())
        };
    }

    private static double CalculateAverage(IReadOnlyList<ReportRowInfo> rows, Func<ReportRowInfo, double> getValue)
    {
        if (rows == null || rows.Count == 0)
            return 0;

        double totalSum = rows.Sum(getValue);
        return totalSum / rows.Count;
    }
}







