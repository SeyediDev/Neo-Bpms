namespace Neo.Bpms.Api.Modules.PowerBIDashboard.Models;

/// <summary>
/// Data table representation
/// </summary>
public class DataTable
{
    public List<DataColumn> Columns { get; set; } = [];
    public List<DataRow> Rows { get; set; } = [];
    public int TotalCount { get; set; }
}

/// <summary>
/// Data column definition
/// </summary>
public class DataColumn
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string"; // string, number, date, boolean
    public string? Format { get; set; }
    public bool IsCalculated { get; set; }
    public string? Formula { get; set; }
}

/// <summary>
/// Data row
/// </summary>
public class DataRow
{
    public Dictionary<string, object?> Values { get; set; } = [];
}

/// <summary>
/// Data source request
/// </summary>
public class DataSourceRequest
{
    public string SourceId { get; set; } = string.Empty;
    public string? Query { get; set; }
    public Dictionary<string, object?>? Parameters { get; set; }
    public int? Limit { get; set; }
}

/// <summary>
/// Transformation request
/// </summary>
public class TransformationRequest
{
    public string Type { get; set; } = string.Empty; // filter, sort, select, rename, etc.
    public Dictionary<string, object?>? Parameters { get; set; }
}

/// <summary>
/// Aggregation request
/// </summary>
public class AggregationRequest
{
    public List<string> GroupBy { get; set; } = [];
    public Dictionary<string, string> Aggregations { get; set; } = []; // column -> function (sum, avg, count, etc.)
}

/// <summary>
/// Calculated field definition
/// </summary>
public class CalculatedField
{
    public string Name { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public string Type { get; set; } = "number";
    public string? Format { get; set; }
}

/// <summary>
/// Filter request
/// </summary>
public class FilterRequest
{
    public string Column { get; set; } = string.Empty;
    public string Operator { get; set; } = "equals"; // equals, notEquals, greaterThan, lessThan, contains, etc.
    public object? Value { get; set; }
    public List<FilterRequest>? AndFilters { get; set; }
    public List<FilterRequest>? OrFilters { get; set; }
}

/// <summary>
/// Group request
/// </summary>
public class GroupRequest
{
    public List<string> GroupBy { get; set; } = [];
    public Dictionary<string, string> Aggregations { get; set; } = [];
}

/// <summary>
/// Join request
/// </summary>
public class JoinRequest
{
    public DataSourceRequest LeftSource { get; set; } = new();
    public DataSourceRequest RightSource { get; set; } = new();
    public string JoinType { get; set; } = "inner"; // inner, left, right, full
    public string LeftKey { get; set; } = string.Empty;
    public string RightKey { get; set; } = string.Empty;
}

