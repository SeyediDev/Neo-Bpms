using Neo.Bpms.Api.Modules.PowerBIDashboard.Models;
using System.Text.Json;

namespace Neo.Bpms.Api.Modules.PowerBIDashboard.Services;

/// <summary>
/// Implementation of data model service with transformations and aggregations
/// </summary>
public class DataModelService : IDataModelService
{
    private readonly ILogger<DataModelService> _logger;

    public DataModelService(ILogger<DataModelService> logger)
    {
        _logger = logger;
    }

    public async Task<DataTable> GetDataAsync(DataSourceRequest request)
    {
        // TODO: Implement actual data source resolution
        // This should resolve sourceId to actual data source (database table, API, file, etc.)
        
        _logger.LogInformation("Getting data from source: {SourceId}", request.SourceId);

        // Placeholder implementation
        return await Task.FromResult(new DataTable
        {
            Columns = new List<DataColumn>
            {
                new() { Name = "id", Type = "number" },
                new() { Name = "name", Type = "string" },
                new() { Name = "value", Type = "number" },
                new() { Name = "date", Type = "date" },
            },
            Rows = new List<DataRow>(),
            TotalCount = 0,
        });
    }

    public async Task<DataTable> TransformDataAsync(DataTable source, TransformationRequest transformation)
    {
        var result = new DataTable
        {
            Columns = new List<DataColumn>(source.Columns),
            Rows = new List<DataRow>(source.Rows),
            TotalCount = source.TotalCount,
        };

        switch (transformation.Type.ToLower())
        {
            case "filter":
                if (transformation.Parameters?.ContainsKey("expression") == true)
                {
                    // Apply filter expression
                    // TODO: Implement expression parser
                }
                break;

            case "sort":
                if (transformation.Parameters?.ContainsKey("column") == true)
                {
                    var column = transformation.Parameters["column"]?.ToString();
                    var direction = transformation.Parameters.GetValueOrDefault("direction", "asc")?.ToString() ?? "asc";
                    
                    if (!string.IsNullOrEmpty(column))
                    {
                        var orderedRows = result.Rows.OrderBy(row =>
                        {
                            var value = row.Values.GetValueOrDefault(column);
                            return value;
                        }).ToList();

                        if (direction == "desc")
                        {
                            orderedRows.Reverse();
                        }
                        
                        result.Rows = orderedRows;
                    }
                }
                break;

            case "select":
                if (transformation.Parameters?.ContainsKey("columns") == true)
                {
                    var columns = JsonSerializer.Deserialize<List<string>>(
                        transformation.Parameters["columns"]?.ToString() ?? "[]");
                    
                    if (columns != null)
                    {
                        result.Columns = result.Columns.Where(c => columns.Contains(c.Name)).ToList();
                        result.Rows = result.Rows.Select(row => new DataRow
                        {
                            Values = row.Values
                                .Where(kvp => columns.Contains(kvp.Key))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                        }).ToList();
                    }
                }
                break;

            case "rename":
                if (transformation.Parameters != null)
                {
                    foreach (var kvp in transformation.Parameters)
                    {
                        var column = result.Columns.FirstOrDefault(c => c.Name == kvp.Key);
                        if (column != null)
                        {
                            column.Name = kvp.Value?.ToString() ?? column.Name;
                        }
                    }
                }
                break;
        }

        return await Task.FromResult(result);
    }

    public async Task<DataTable> AggregateDataAsync(DataTable source, AggregationRequest aggregation)
    {
        var result = new DataTable
        {
            Columns = new List<DataColumn>(),
            Rows = new List<DataRow>(),
        };

        // Add group by columns
        foreach (var groupBy in aggregation.GroupBy)
        {
            var column = source.Columns.FirstOrDefault(c => c.Name == groupBy);
            if (column != null)
            {
                result.Columns.Add(new DataColumn
                {
                    Name = column.Name,
                    Type = column.Type,
                    Format = column.Format,
                });
            }
        }

        // Add aggregation columns
        foreach (var agg in aggregation.Aggregations)
        {
            var sourceColumn = source.Columns.FirstOrDefault(c => c.Name == agg.Key);
            if (sourceColumn != null)
            {
                result.Columns.Add(new DataColumn
                {
                    Name = $"{agg.Value}({agg.Key})",
                    Type = "number",
                });
            }
        }

        // Group and aggregate
        var grouped = source.Rows
            .GroupBy(row => string.Join("|", aggregation.GroupBy.Select(gb => row.Values.GetValueOrDefault(gb)?.ToString() ?? "")))
            .ToList();

        foreach (var group in grouped)
        {
            var row = new DataRow { Values = new Dictionary<string, object?>() };

            // Add group by values
            var firstRow = group.First();
            foreach (var groupBy in aggregation.GroupBy)
            {
                row.Values[groupBy] = firstRow.Values.GetValueOrDefault(groupBy);
            }

            // Add aggregations
            foreach (var agg in aggregation.Aggregations)
            {
                var values = group.Select(r => r.Values.GetValueOrDefault(agg.Key))
                    .Where(v => v != null)
                    .ToList();

                object? aggregatedValue = agg.Value.ToLower() switch
                {
                    "sum" => values.OfType<decimal>().Sum(),
                    "avg" => values.OfType<decimal>().Average(),
                    "count" => values.Count,
                    "min" => values.Min(),
                    "max" => values.Max(),
                    _ => values.Count,
                };

                row.Values[$"{agg.Value}({agg.Key})"] = aggregatedValue;
            }

            result.Rows.Add(row);
        }

        result.TotalCount = result.Rows.Count;
        return await Task.FromResult(result);
    }

    public async Task<DataTable> AddCalculatedFieldAsync(DataTable source, CalculatedField calculatedField)
    {
        var result = new DataTable
        {
            Columns = new List<DataColumn>(source.Columns)
            {
                new DataColumn
                {
                    Name = calculatedField.Name,
                    Type = calculatedField.Type,
                    Format = calculatedField.Format,
                    IsCalculated = true,
                    Formula = calculatedField.Formula,
                }
            },
            Rows = new List<DataRow>(),
            TotalCount = source.TotalCount,
        };

        // TODO: Implement formula evaluation
        // For now, just add the column with null values
        foreach (var row in source.Rows)
        {
            var newRow = new DataRow
            {
                Values = new Dictionary<string, object?>(row.Values)
            };
            newRow.Values[calculatedField.Name] = null; // TODO: Evaluate formula
            result.Rows.Add(newRow);
        }

        return await Task.FromResult(result);
    }

    public async Task<DataTable> FilterDataAsync(DataTable source, FilterRequest filter)
    {
        var result = new DataTable
        {
            Columns = new List<DataColumn>(source.Columns),
            Rows = new List<DataRow>(),
        };

        result.Rows = source.Rows.Where(row =>
        {
            return EvaluateFilter(row, filter);
        }).ToList();

        result.TotalCount = result.Rows.Count;
        return await Task.FromResult(result);
    }

    private bool EvaluateFilter(DataRow row, FilterRequest filter)
    {
        var value = row.Values.GetValueOrDefault(filter.Column);
        var filterValue = filter.Value;

        var matches = filter.Operator.ToLower() switch
        {
            "equals" => Equals(value, filterValue),
            "notequals" => !Equals(value, filterValue),
            "greaterthan" => CompareNumbers(value, filterValue) > 0,
            "lessthan" => CompareNumbers(value, filterValue) < 0,
            "greaterthanorequal" => CompareNumbers(value, filterValue) >= 0,
            "lessthanorequal" => CompareNumbers(value, filterValue) <= 0,
            "contains" => value?.ToString()?.Contains(filterValue?.ToString() ?? "", StringComparison.OrdinalIgnoreCase) == true,
            "startswith" => value?.ToString()?.StartsWith(filterValue?.ToString() ?? "", StringComparison.OrdinalIgnoreCase) == true,
            "endswith" => value?.ToString()?.EndsWith(filterValue?.ToString() ?? "", StringComparison.OrdinalIgnoreCase) == true,
            _ => false,
        };

        // Handle AND filters
        if (filter.AndFilters != null && filter.AndFilters.Count > 0)
        {
            matches = matches && filter.AndFilters.All(f => EvaluateFilter(row, f));
        }

        // Handle OR filters
        if (filter.OrFilters != null && filter.OrFilters.Count > 0)
        {
            matches = matches || filter.OrFilters.Any(f => EvaluateFilter(row, f));
        }

        return matches;
    }

    private int CompareNumbers(object? value1, object? value2)
    {
        if (value1 == null || value2 == null) return 0;
        
        if (decimal.TryParse(value1.ToString(), out var d1) && 
            decimal.TryParse(value2.ToString(), out var d2))
        {
            return d1.CompareTo(d2);
        }

        return string.Compare(value1.ToString(), value2.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public async Task<DataTable> GroupDataAsync(DataTable source, GroupRequest group)
    {
        return await AggregateDataAsync(source, new AggregationRequest
        {
            GroupBy = group.GroupBy,
            Aggregations = group.Aggregations,
        });
    }

    public async Task<DataTable> JoinDataAsync(JoinRequest join)
    {
        // TODO: Implement join logic
        var leftData = await GetDataAsync(join.LeftSource);
        var rightData = await GetDataAsync(join.RightSource);

        // Placeholder: simple inner join
        var result = new DataTable
        {
            Columns = new List<DataColumn>(leftData.Columns),
            Rows = new List<DataRow>(),
        };

        // Add right columns (excluding the join key)
        foreach (var col in rightData.Columns.Where(c => c.Name != join.RightKey))
        {
            result.Columns.Add(new DataColumn
            {
                Name = col.Name,
                Type = col.Type,
            });
        }

        // Perform join
        foreach (var leftRow in leftData.Rows)
        {
            var leftKeyValue = leftRow.Values.GetValueOrDefault(join.LeftKey);
            var matchingRightRow = rightData.Rows.FirstOrDefault(r =>
                Equals(r.Values.GetValueOrDefault(join.RightKey), leftKeyValue));

            if (matchingRightRow != null || join.JoinType == "left")
            {
                var joinedRow = new DataRow
                {
                    Values = new Dictionary<string, object?>(leftRow.Values)
                };

                if (matchingRightRow != null)
                {
                    foreach (var kvp in matchingRightRow.Values.Where(kvp => kvp.Key != join.RightKey))
                    {
                        joinedRow.Values[kvp.Key] = kvp.Value;
                    }
                }

                result.Rows.Add(joinedRow);
            }
        }

        result.TotalCount = result.Rows.Count;
        return await Task.FromResult(result);
    }
}

