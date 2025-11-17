using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;
using Neo.Bpms.Infrastructure.Features.Cmmn.Reports;
using Neo.Bpms.UI.MVC.Controls;

namespace Neo.Bpms.UI.MVC.Tests;

public class AggregationCalculatorTests
{
    [Fact]
    public void Calculate_EmptyRows_ReturnsZero()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Count);
        var rows = new List<ReportRowInfo>();

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Calculate_NullRows_ReturnsZero()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Count);

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, null, GetValue);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Calculate_SingleRow_ReturnsRowValue()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Count);
        var row = CreateRow(5.0);
        var rows = new List<ReportRowInfo> { row };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Calculate_Count_ReturnsSumOfAllValues()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Count);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(10.0),
            CreateRow(20.0),
            CreateRow(30.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(60.0, result);
    }

    [Fact]
    public void Calculate_Sum_ReturnsSumOfAllValues()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Sum);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(10.5),
            CreateRow(20.3),
            CreateRow(30.2)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(61.0, result, 1); // Allow small floating point differences
    }

    [Fact]
    public void Calculate_Average_ReturnsAverageOfAllValues()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Avg);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(10.0),
            CreateRow(20.0),
            CreateRow(30.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void Calculate_Average_WithDecimalValues_ReturnsCorrectAverage()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Avg);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(10.5),
            CreateRow(20.5),
            CreateRow(30.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(20.333333333333332, result, 10); // 61.0 / 3 = 20.333...
    }

    [Fact]
    public void Calculate_Max_ReturnsMaximumValue()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Max);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(10.0),
            CreateRow(30.0),
            CreateRow(20.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void Calculate_Min_ReturnsMinimumValue()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Min);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(30.0),
            CreateRow(10.0),
            CreateRow(20.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Calculate_Count_WithMultipleRows_SimulatesRangeGrouping()
    {
        // Arrange - Simulate CLV distribution where multiple rows are grouped into ranges
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Count);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(1.0), // CLV: 1077550 -> "کمتر از 1,090,000"
            CreateRow(1.0), // CLV: 1080000 -> "کمتر از 1,090,000"
            CreateRow(1.0), // CLV: 1095000 -> "1,090,000 - 1,100,000"
            CreateRow(1.0), // CLV: 1105000 -> "1,100,000 - 1,110,000"
            CreateRow(1.0)  // CLV: 1140000 -> "بیشتر از 1,130,000"
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert - All rows in the same range should sum their counts
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Calculate_Average_WithRealWorldCLVScenario()
    {
        // Arrange - Simulate average CLV calculation for a range
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Avg);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(1095000.0), // Customer 1
            CreateRow(1098000.0), // Customer 2
            CreateRow(1099000.0)  // Customer 3
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        var expected = (1095000.0 + 1098000.0 + 1099000.0) / 3.0;
        Assert.Equal(expected, result, 1);
    }

    [Fact]
    public void Calculate_Sum_WithLargeNumbers()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.Sum);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(1000000.0),
            CreateRow(2000000.0),
            CreateRow(3000000.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(6000000.0, result);
    }

    [Fact]
    public void Calculate_DefaultAggregationType_ReturnsFirstRowValue()
    {
        // Arrange
        var aggrColumn = CreateAggregationColumn(eAggregationFunctions.InColumn);
        var rows = new List<ReportRowInfo>
        {
            CreateRow(10.0),
            CreateRow(20.0),
            CreateRow(30.0)
        };

        // Act
        var result = AggregationCalculator.Calculate(aggrColumn, rows, GetValue);

        // Assert
        Assert.Equal(10.0, result);
    }

    // Helper methods
    private static ColumnFieldDefinition CreateAggregationColumn(eAggregationFunctions aggrType)
    {
        return new ColumnFieldDefinition("TestColumn")
        {
            aggrType = aggrType
        };
    }

    private static ReportRowInfo CreateRow(double value)
    {
        var row = new ReportRowInfo
        {
            Data = new ElasticObject()
        };
        row.Data.SetField("TestColumn", value);
        return row;
    }

    private static double GetValue(ColumnFieldDefinition column, ReportRowInfo row)
    {
        if (row?.Data == null)
            return 0;

        if (row.Data.GetField("TestColumn", out object value))
        {
            if (value == null)
                return 0;

            if (double.TryParse(value.ToString(), out double result))
                return result;
        }

        return 0;
    }
}

