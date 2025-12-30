using Neo.Bpms.Api.Modules.EditableGrid.Models;
using System.Text.Json;

namespace Neo.Bpms.Api.Modules.EditableGrid.Services;

public class EditableGridService : IEditableGridService
{
    private readonly ILogger<EditableGridService> _logger;
    private readonly Dictionary<string, GridConfigResponse> _configCache = new();

    public EditableGridService(ILogger<EditableGridService> logger)
    {
        _logger = logger;
    }

    public async Task<GridDataResponse> GetGridDataAsync(GridDataRequest request)
    {
        // TODO: Implement actual data fetching based on endpoint
        // This is a placeholder that should be replaced with actual data source
        
        _logger.LogInformation("Getting grid data for endpoint: {Endpoint}", request.Endpoint);

        // Example: For now, return sample data
        // In production, this should:
        // 1. Resolve endpoint to actual data source (table, view, API, etc.)
        // 2. Apply pagination, sorting, filtering
        // 3. Return formatted data

        var response = new GridDataResponse
        {
            Columns = GetColumnsForEndpoint(request.Endpoint),
            Rows = GetSampleRows(request.Endpoint, request.Page, request.PageSize),
            TotalCount = 100, // TODO: Get actual count
            Page = request.Page,
            PageSize = request.PageSize,
        };

        return await Task.FromResult(response);
    }

    public async Task<BatchUpdateResponse> BatchUpdateAsync(string endpoint, BatchUpdateRequest request)
    {
        _logger.LogInformation("Batch updating {Count} cells for endpoint: {Endpoint}", 
            request.Changes.Count, endpoint);

        var response = new BatchUpdateResponse
        {
            Success = true,
            Updated = 0,
            Errors = new List<UpdateError>(),
        };

        // TODO: Implement actual batch update logic
        // This should:
        // 1. Validate all changes
        // 2. Apply changes in a transaction
        // 3. Return success/error for each change

        foreach (var change in request.Changes)
        {
            try
            {
                // Validate change
                if (string.IsNullOrEmpty(change.ColumnId))
                {
                    response.Errors!.Add(new UpdateError
                    {
                        RowId = change.RowId,
                        ColumnId = change.ColumnId,
                        Message = "Column ID is required",
                    });
                    continue;
                }

                // TODO: Apply change to actual data source
                // For now, just mark as updated
                response.Updated++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cell {RowId}/{ColumnId}", 
                    change.RowId, change.ColumnId);
                
                response.Errors!.Add(new UpdateError
                {
                    RowId = change.RowId,
                    ColumnId = change.ColumnId,
                    Message = ex.Message,
                });
            }
        }

        response.Success = response.Errors!.Count == 0;
        return await Task.FromResult(response);
    }

    public async Task<GridConfigResponse> GetGridConfigAsync(string endpoint)
    {
        if (_configCache.TryGetValue(endpoint, out var cached))
        {
            return cached;
        }

        var config = new GridConfigResponse
        {
            Columns = GetColumnsForEndpoint(endpoint),
            EnableEditing = true,
            EnablePagination = true,
            EnableSorting = true,
            EnableFiltering = true,
        };

        _configCache[endpoint] = config;
        return await Task.FromResult(config);
    }

    private List<GridColumn> GetColumnsForEndpoint(string endpoint)
    {
        // TODO: Resolve columns from actual data source metadata
        // For now, return sample columns based on endpoint
        
        return endpoint switch
        {
            "sample" => new List<GridColumn>
            {
                new() { Id = "id", Name = "شناسه", Type = "number", Editable = false, Width = 100 },
                new() { Id = "name", Name = "نام", Type = "text", Editable = true, Required = true, Width = 200 },
                new() { Id = "email", Name = "ایمیل", Type = "text", Editable = true, Width = 250 },
                new() { Id = "age", Name = "سن", Type = "number", Editable = true, Width = 100 },
                new() { Id = "status", Name = "وضعیت", Type = "select", Editable = true, Width = 150,
                    Options = new List<SelectOption>
                    {
                        new() { Value = "active", Label = "فعال" },
                        new() { Value = "inactive", Label = "غیرفعال" },
                        new() { Value = "pending", Label = "در انتظار" },
                    }
                },
                new() { Id = "createdAt", Name = "تاریخ ایجاد", Type = "date", Editable = true, Width = 150 },
                new() { Id = "isActive", Name = "فعال", Type = "boolean", Editable = true, Width = 80 },
            },
            _ => new List<GridColumn>
            {
                new() { Id = "id", Name = "شناسه", Type = "number", Editable = false },
                new() { Id = "name", Name = "نام", Type = "text", Editable = true },
            },
        };
    }

    private List<Dictionary<string, object?>> GetSampleRows(string endpoint, int page, int pageSize)
    {
        // TODO: Get actual rows from data source
        var rows = new List<Dictionary<string, object?>>();
        
        var startId = (page - 1) * pageSize + 1;
        for (int i = 0; i < pageSize; i++)
        {
            var row = new Dictionary<string, object?>
            {
                ["id"] = startId + i,
                ["name"] = $"نام {startId + i}",
                ["email"] = $"user{startId + i}@example.com",
                ["age"] = 20 + (i % 30),
                ["status"] = i % 3 == 0 ? "active" : i % 3 == 1 ? "inactive" : "pending",
                ["createdAt"] = DateTime.Now.AddDays(-i).ToString("O"),
                ["isActive"] = i % 2 == 0,
            };
            rows.Add(row);
        }

        return rows;
    }
}

