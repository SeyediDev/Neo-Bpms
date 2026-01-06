using Neo.Bpms.Api.Modules.EditableGrid.Models;
using System.Text.Json;
using OfficeOpenXml;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormsDataRoutines;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Common.Extensions;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Domain.Utility;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;
using Neo.Bpms.UI.MVC.Exceptions;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.CombosData;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Domain.Features.Client;
using Neo.Bpms.Domain.Features.Dynamic;

namespace Neo.Bpms.Api.Modules.EditableGrid.Services;

public partial class EditableGridService : IEditableGridService
{
    private readonly ILogger<EditableGridService> _logger;
    private readonly Dictionary<string, GridConfigResponse> _configCache = new();
    private readonly FormStructRoutines _formStructRoutines;
    private readonly FormDataRoutines _formDataRoutines;
    private readonly IServiceProvider _serviceProvider;
    private readonly IApplyFormData _applyFormData;
    private readonly IFormExcelImporter _excelImporter;
    private readonly FormServiceOperation _formServiceOperation;

    public EditableGridService(
        ILogger<EditableGridService> logger,
        FormStructRoutines formStructRoutines,
        FormDataRoutines formDataRoutines,
        IServiceProvider serviceProvider,
        IApplyFormData applyFormData,
        IFormExcelImporter excelImporter,
        FormServiceOperation formServiceOperation)
    {
        _logger = logger;
        _formStructRoutines = formStructRoutines;
        _formDataRoutines = formDataRoutines;
        _serviceProvider = serviceProvider;
        _applyFormData = applyFormData;
        _excelImporter = excelImporter;
        _formServiceOperation = formServiceOperation;
    }

    public async Task<GridDataResponse> GetGridDataAsync(GridDataRequest request)
    {
        _logger.LogInformation("Getting grid data for endpoint: {Endpoint}", request.Endpoint);

        // Try to parse endpoint as NamespaceId/EntityId/FormId
        var endpointParts = request.Endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (endpointParts.Length == 3)
        {
            // Real form endpoint
            var namespaceId = endpointParts[0];
            var entityId = endpointParts[1];
            var formId = endpointParts[2];
            
            return await GetFormDataAsync(namespaceId, entityId, formId, request);
        }
        
        // Fallback to sample data
        return await GetSampleDataAsync(request);
    }

    private async Task<GridDataResponse> GetFormDataAsync(
        string namespaceId, 
        string entityId, 
        string formId, 
        GridDataRequest request)
    {
        try
        {
            // Get form
            var form = FormStructRoutines.GetForm(namespaceId, entityId, formId, Neo.Bpms.Domain.Model.UI.Forms.Form.eFormType.Index, null);
            if (form == null)
            {
                _logger.LogWarning("Form not found: {NamespaceId}/{EntityId}/{FormId}", namespaceId, entityId, formId);
                return await GetSampleDataAsync(request);
            }

            // Get form structure
            var culture = "fa"; // TODO: Get from request context
            var user = GetCurrentUser(); // TODO: Get from request context
            var structure = await _formStructRoutines.GetIndexStructure(
                culture, namespaceId, entityId, form.FormSubjectId, formId, 
                request.SortBy, form, user);

            if (structure == null)
            {
                _logger.LogWarning("Form structure not found for: {FormId}", formId);
                return await GetSampleDataAsync(request);
            }

            // Parse filters
            ElasticObject? filterValues = null;
            if (!string.IsNullOrEmpty(request.Filter))
            {
                try
                {
                    var filters = JsonSerializer.Deserialize<List<FilterCondition>>(request.Filter);
                    if (filters != null && filters.Count > 0)
                    {
                        filterValues = new ElasticObject();
                        foreach (var filter in filters)
                        {
                            filterValues.SetField(filter.ColumnId, filter.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse filter: {Filter}", request.Filter);
                }
            }

            // Get records
            var sortFields = !string.IsNullOrEmpty(request.SortBy) 
                ? $"{request.SortBy}#{(request.SortDirection ?? "asc")}" 
                : null;

            var cancellationToken = CancellationToken.None;
            var localParameters = user != null ? new LocalParameters(user) : new LocalParameters();
            
            // Set combo data for select/multiselect fields
            if (user != null)
            {
                FormComboData.SetCombosData(form, structure, culture, null, localParameters);
            }
            
            IndexFormData records;
            if (string.IsNullOrEmpty(form.GetServiceOperation))
            {
                records = await _formDataRoutines.GetRecordsWithoutJoin(
                    structure, form.entity, form, culture, filterValues,
                    sortFields, request.Page, request.PageSize, null,
                    localParameters,
                    null, // AdditionalFilters
                    user, true, true, cancellationToken);
            }
            else
            {
                // Handle service operation
                var filterValuesObj = filterValues ?? new ElasticObject();
                records = await CallIndexGetServiceOperation(form, filterValuesObj, sortFields, user, request.Page, request.PageSize);
            }

            // Convert to grid format
            var columns = ConvertColumns(structure);
            var rows = ConvertRows(records.Rows, structure);

            return new GridDataResponse
            {
                Columns = columns,
                Rows = rows,
                TotalCount = (int)records.recordCount,
                Page = request.Page,
                PageSize = request.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting form data for {Endpoint}", request.Endpoint);
            return await GetSampleDataAsync(request);
        }
    }

    private async Task<GridDataResponse> GetSampleDataAsync(GridDataRequest request)
    {
        // Parse filter if provided
        List<FilterCondition>? filters = null;
        if (!string.IsNullOrEmpty(request.Filter))
        {
            try
            {
                filters = JsonSerializer.Deserialize<List<FilterCondition>>(request.Filter);
            }
            catch
            {
                _logger.LogWarning("Failed to parse filter: {Filter}", request.Filter);
            }
        }

        // Get all rows (for filtering)
        var allRows = GetSampleRows(request.Endpoint, 1, int.MaxValue);
        
        // Apply filters
        if (filters != null && filters.Count > 0)
        {
            allRows = ApplyFilters(allRows, filters);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            allRows = ApplySorting(allRows, request.SortBy, request.SortDirection ?? "asc");
        }

        var totalCount = allRows.Count;

        // Apply pagination
        var paginatedRows = allRows
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new GridDataResponse
        {
            Columns = GetColumnsForEndpoint(request.Endpoint),
            Rows = paginatedRows,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };

        return await Task.FromResult(response);
    }

    private List<GridColumn> ConvertColumns(CommonFormStructure structure)
    {
        var columns = new List<GridColumn>();

            if (structure.ColumnInfos != null)
            {
                foreach (var col in structure.ColumnInfos)
            {
                var field = structure.Fields?.FirstOrDefault(f => f.FieldName == col.FieldName);
                if (field == null) continue;

                var column = new GridColumn
                {
                    Id = col.FieldName,
                    Name = col.Label ?? col.FieldName,
                    Type = GetColumnType(field),
                    Editable = true, // TODO: Check ReadOnly property
                    Required = false, // TODO: Check IsMandatory property
                    Width = 150, // Default width
                };

                // Add options for select fields
                if (field.ControlType == eControlTypeId.ComboBox || 
                    field.ControlType == eControlTypeId.MultipleSelectableCombo)
                {
                    // Get options from combo data (set by FormComboData.SetCombosData)
                    if (structure.CombosData != null && 
                        structure.CombosData.TryGetValue(field.FieldName, out var comboData) &&
                        comboData?.Rows != null)
                    {
                        column.Options = comboData.Rows
                            .Select(row => new SelectOption
                            {
                                Value = row.Ids ?? "",
                                Label = row.DisplayValue ?? row.Ids ?? ""
                            })
                            .ToList();
                    }
                }

                columns.Add(column);
            }
        }

        return columns;
    }

    private RowActionsConfig? ConvertRowActions(CommonFormStructure structure)
    {
        if (!structure.HasDetails && !structure.HasEdit && !structure.HasDelete && 
            (structure.Subjects == null || structure.Subjects.Count == 0) &&
            (structure.Fields == null || !structure.Fields.Any(f => f.ControlType == eControlTypeId.SpecificLinkColumn)))
        {
            return null;
        }

        var config = new RowActionsConfig
        {
            HasDetails = structure.HasDetails,
            HasEdit = structure.HasEdit,
            HasDelete = structure.HasDelete,
            DetailFormId = structure.DetailFormId,
            EditFormId = structure.EditFormId,
            DeleteFormId = structure.DeleteFormId,
            DetailAction = structure.DetailAction,
            EditAction = structure.EditAction,
        };

        // Convert subject forms
        if (structure.Subjects != null && structure.Subjects.Count > 0)
        {
            config.SubjectForms = structure.Subjects
                .Where(s => s.HasEditForm || s.HasDetailsForm)
                .Select(s => new SubjectFormLink
                {
                    Name = s.Name,
                    Alias = s.Alias,
                    HasEditForm = s.HasEditForm,
                    HasDetailsForm = s.HasDetailsForm,
                    EditFormId = s.EditFormId,
                    DetailFormId = s.DetailFormId,
                    EditAction = s.EditAction,
                    DetailAction = s.DetailAction,
                })
                .ToList();
        }

        // Convert specific link columns
        if (structure.Fields != null)
        {
            var specificLinks = structure.Fields
                .Where(f => f.ControlType == eControlTypeId.SpecificLinkColumn && f.GetProperties() != null)
                .Select(f =>
                {
                    var link = new SpecificLinkColumn
                    {
                        Label = f.Label ?? f.FieldName,
                        LinkTarget = f.PropertyValue(eControlPropertyId.LinkTarget) ?? "",
                        LinkParameters = new Dictionary<string, string>(),
                    };

                    // Extract link parameters
                    foreach (var property in f.GetProperties(eControlPropertyId.LinkParameter))
                    {
                        var paramStr = property.Value?.ToString() ?? "";
                        var parts = paramStr.Split('=');
                        if (parts.Length == 2)
                        {
                            link.LinkParameters[parts[0]] = parts[1];
                        }
                    }

                    return link;
                })
                .ToList();

            if (specificLinks.Count > 0)
            {
                config.SpecificLinks = specificLinks;
            }
        }

        return config;
    }

    private string GetColumnType(InputFieldDefinition field)
    {
        return field.ControlType switch
        {
            eControlTypeId.NumberInput => "number",
            eControlTypeId.DatePicker => "date",
            eControlTypeId.BooleanCombo => "boolean",
            eControlTypeId.ComboBox => "select",
            eControlTypeId.MultipleSelectableCombo => "multiselect",
            _ => "text",
        };
    }

    private List<Dictionary<string, object?>> ConvertRows(List<ElasticObject> elasticRows, CommonFormStructure structure)
    {
        var rows = new List<Dictionary<string, object?>>();

        foreach (var elasticRow in elasticRows)
        {
            var row = new Dictionary<string, object?>();
            
            // Extract ID from primary key fields
            // For single key: use the key field value
            // For composite keys: join with '#'
            if (structure.KeyFields != null && structure.KeyFields.Count > 0)
            {
                var keyValues = structure.KeyFields
                    .Select(keyFieldId => 
                    {
                        elasticRow.GetField(keyFieldId, out var keyValue);
                        return keyValue?.ToString() ?? "";
                    })
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();
                
                if (keyValues.Count > 0)
                {
                    row["id"] = keyValues.Count == 1 
                        ? keyValues[0] 
                        : string.Join("#", keyValues);
                }
                else
                {
                    // Fallback to "Id" field
                    elasticRow.GetField("Id", out var idValue);
                    row["id"] = idValue?.ToString() ?? "";
                }
            }
            else
            {
                // Fallback to "Id" field
                elasticRow.GetField("Id", out var idValue);
                row["id"] = idValue?.ToString() ?? "";
            }

            // Add other columns
            if (structure.ColumnInfos != null)
            {
                foreach (var col in structure.ColumnInfos)
                {
                    elasticRow.GetField(col.FieldName, out var value);
                    row[col.FieldName] = value;
                }
            }

            rows.Add(row);
        }

        return rows;
    }

    private IdentityUser? GetCurrentUser()
    {
        try
        {
            // Get from HttpContext if available
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;
            
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                // Get IRequesterUser from service provider (same as ControllerBaseMVC.GetUser)
                var requesterUser = httpContext.RequestServices.GetService<IRequesterUser>();
                if (requesterUser != null)
                {
                    // Get IdentityUser from RequesterUser
                    var identityUser = requesterUser.GetProperty(nameof(IdentityUser)) as IdentityUser;
                    return identityUser;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get current user from HttpContext");
        }
        
        return null;
    }

    private List<Dictionary<string, object?>> ApplyFilters(
        List<Dictionary<string, object?>> rows,
        List<FilterCondition> filters)
    {
        return rows.Where(row =>
        {
            return filters.All(filter =>
            {
                var value = row.GetValueOrDefault(filter.ColumnId);
                return EvaluateFilter(value, filter.Operator, filter.Value);
            });
        }).ToList();
    }

    private bool EvaluateFilter(object? value, string operatorStr, object? filterValue)
    {
        if (value == null && filterValue == null) return operatorStr == "equals";
        if (value == null || filterValue == null) return false;

        return operatorStr.ToLower() switch
        {
            "equals" => value.ToString() == filterValue.ToString(),
            "notequals" => value.ToString() != filterValue.ToString(),
            "contains" => value.ToString()?.Contains(filterValue.ToString() ?? "", StringComparison.OrdinalIgnoreCase) == true,
            "startswith" => value.ToString()?.StartsWith(filterValue.ToString() ?? "", StringComparison.OrdinalIgnoreCase) == true,
            "endswith" => value.ToString()?.EndsWith(filterValue.ToString() ?? "", StringComparison.OrdinalIgnoreCase) == true,
            "greaterthan" => CompareNumbers(value, filterValue) > 0,
            "lessthan" => CompareNumbers(value, filterValue) < 0,
            "greaterthanorequal" => CompareNumbers(value, filterValue) >= 0,
            "lessthanorequal" => CompareNumbers(value, filterValue) <= 0,
            _ => false,
        };
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

    private List<Dictionary<string, object?>> ApplySorting(
        List<Dictionary<string, object?>> rows,
        string sortBy,
        string direction)
    {
        var sorted = direction.ToLower() == "desc"
            ? rows.OrderByDescending(r => r.GetValueOrDefault(sortBy))
            : rows.OrderBy(r => r.GetValueOrDefault(sortBy));
        
        return sorted.ToList();
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

        // Parse endpoint
        var endpointParts = endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (endpointParts.Length != 3)
        {
            response.Success = false;
            response.Errors!.Add(new UpdateError
            {
                RowId = "",
                ColumnId = "",
                Message = "Invalid endpoint format. Expected: NamespaceId/EntityId/FormId",
            });
            return response;
        }

        var namespaceId = endpointParts[0];
        var entityId = endpointParts[1];
        var formId = endpointParts[2];

        try
        {
            // Get form and structure
            var form = FormStructRoutines.GetForm(namespaceId, entityId, formId, Neo.Bpms.Domain.Model.UI.Forms.Form.eFormType.Edit, null);
            if (form == null)
            {
                response.Success = false;
                response.Errors!.Add(new UpdateError
                {
                    RowId = "",
                    ColumnId = "",
                    Message = "Form not found",
                });
                return response;
            }

            var culture = "fa";
            var user = GetCurrentUser();
            if (user == null)
            {
                response.Success = false;
                response.Errors!.Add(new UpdateError
                {
                    RowId = "",
                    ColumnId = "",
                    Message = "User not authenticated",
                });
                return response;
            }

            var structure = _formStructRoutines.GetCommonFormStructure(
                culture, namespaceId, entityId, formId, Neo.Bpms.Domain.Model.UI.Forms.Form.eFormType.Edit, 
                form.FormSubjectId, form, user);

            // Group changes by row
            var changesByRow = request.Changes
                .GroupBy(c => c.RowId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Process each row
            foreach (var rowGroup in changesByRow)
            {
                var rowId = rowGroup.Key;
                var rowChanges = rowGroup.Value;

                try
                {
                    // Get existing record (using static method)
                    var keyRecord = FormDataRoutines.GetKeyRecord(form.entity, rowId.ToString());
                    if (keyRecord == null)
                    {
                        response.Errors!.Add(new UpdateError
                        {
                            RowId = rowId,
                            ColumnId = "",
                            Message = "Record not found",
                        });
                        continue;
                    }

                    // Build form data from changes
                    var formData = new ElasticObject();
                    foreach (var change in rowChanges)
                    {
                        formData.SetField(change.ColumnId, change.Value);
                    }

                    // Convert posted data to record format
                    _applyFormData.ConvertPostedElasticToRecord(ref formData, structure);

                    // Create audit trail
                    var triggerTypeId = form.TriggerTypeId(false);
                    var auditTrail = new AuditTrail(
                        triggerTypeId,
                        $"Editable Grid Update {triggerTypeId} Form {form.Id}",
                        user,
                        0)
                    {
                        MetaEntityId = form.entity.DbId,
                        MetaFormId = form.DbId,
                        FormId = form.Id,
                        EntityPkv = rowId.ToString()
                    };

                    // Update record
                    var errors = new ExceptionInfos();
                    var localParameters = new LocalParameters(user);
                    
                    var saved = await _applyFormData.UpdateRecord(
                        auditTrail,
                        namespaceId,
                        entityId,
                        form,
                        formData,
                        keyRecord,
                        null, // validFieldIds
                        errors,
                        false, // forVirtualDelete
                        null, // apply
                        localParameters,
                        CancellationToken.None);

                    if (saved && !(errors?.Any() ?? false))
                    {
                        // Update tables if needed
                        if (structure.Tables != null && structure.Tables.Count > 0)
                        {
                            saved = await _applyFormData.UpdateTables(
                                auditTrail,
                                namespaceId,
                                entityId,
                                form.FormSubjectId,
                                formData,
                                rowId.ToString(),
                                culture,
                                structure.Tables,
                                errors,
                                CancellationToken.None);
                        }
                    }

                    if (saved && !(errors?.Any() ?? false))
                    {
                        response.Updated += rowChanges.Count;
                    }
                    else
                    {
                        foreach (var error in errors ?? new ExceptionInfos())
                        {
                            response.Errors!.Add(new UpdateError
                            {
                                RowId = rowId,
                                ColumnId = error.ForField ?? "",
                                Message = error.Exception?.Message ?? "Update failed",
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating row {RowId}", rowId);
                    response.Errors!.Add(new UpdateError
                    {
                        RowId = rowId,
                        ColumnId = "",
                        Message = ex.Message,
                    });
                }
            }

            response.Success = response.Errors!.Count == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch update for endpoint: {Endpoint}", endpoint);
            response.Success = false;
            response.Errors!.Add(new UpdateError
            {
                RowId = "",
                ColumnId = "",
                Message = ex.Message,
            });
        }

        return response;
    }

    public async Task<GridConfigResponse> GetGridConfigAsync(string endpoint)
    {
        if (_configCache.TryGetValue(endpoint, out var cached))
        {
            return cached;
        }

        // Try to parse endpoint as NamespaceId/EntityId/FormId
        var endpointParts = endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        GridConfigResponse config;
        if (endpointParts.Length == 3)
        {
            // Real form endpoint
            var namespaceId = endpointParts[0];
            var entityId = endpointParts[1];
            var formId = endpointParts[2];
            
            try
            {
                var form = FormStructRoutines.GetForm(namespaceId, entityId, formId, Neo.Bpms.Domain.Model.UI.Forms.Form.eFormType.Index, null);
                if (form != null)
                {
                    var culture = "fa";
                    var user = GetCurrentUser();
                    var structure = await _formStructRoutines.GetIndexStructure(
                        culture, namespaceId, entityId, form.FormSubjectId, formId, 
                        null, form, user);
                    
                    if (structure != null)
                    {
                        // Set combo data for select/multiselect fields
                        if (user != null)
                        {
                            var localParameters = new LocalParameters(user);
                            FormComboData.SetCombosData(form, structure, culture, null, localParameters);
                        }
                        
                        config = new GridConfigResponse
                        {
                            Columns = ConvertColumns(structure),
                            EnableEditing = true,
                            EnablePagination = true,
                            EnableSorting = true,
                            EnableFiltering = true,
                            RowActions = ConvertRowActions(structure),
                        };
                        
                        _configCache[endpoint] = config;
                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get form config for endpoint: {Endpoint}", endpoint);
            }
        }

        // Fallback to sample config
        config = new GridConfigResponse
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

    public async Task<byte[]> ExportToExcelAsync(string endpoint, ExcelExportRequest request)
    {
        _logger.LogInformation("Exporting Excel for endpoint: {Endpoint}", endpoint);

        // Parse endpoint
        var endpointParts = endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (endpointParts.Length == 3)
        {
            // Real form endpoint - use ExportExcelForm
            var namespaceId = endpointParts[0];
            var entityId = endpointParts[1];
            var formId = endpointParts[2];

            try
            {
                var form = FormStructRoutines.GetForm(namespaceId, entityId, formId, Neo.Bpms.Domain.Model.UI.Forms.Form.eFormType.Index, null);
                if (form != null)
                {
                    var culture = "fa";
                    var user = GetCurrentUser();
                    if (user == null)
                    {
                        throw new UnauthenticatedUserException();
                    }

                    var structure = await _formStructRoutines.GetIndexStructure(
                        culture, namespaceId, entityId, form.FormSubjectId, formId, 
                        request.SortBy?.Column, form, user);

                    // Build filter values from request
                    ElasticObject? filterValues = null;
                    if (request.Filters != null && request.Filters.Count > 0)
                    {
                        filterValues = new ElasticObject();
                        foreach (var filter in request.Filters)
                        {
                            filterValues.SetField(filter.ColumnId, filter.Value);
                        }
                    }

                    // Use ExportExcelForm
                    var exportExcel = new ExportExcelForm(
                        form,
                        structure,
                        filterValues,
                        user,
                        true, // setData
                        culture,
                        true, // isRightToLeft
                        ExportType.Edit,
                        CancellationToken.None);

                    var stream = exportExcel.Export(_formStructRoutines, _formDataRoutines, out _);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting Excel using ExportExcelForm for endpoint: {Endpoint}", endpoint);
                // Fall through to simple export
            }
        }

        // Fallback to simple export
        var dataRequest = new GridDataRequest
        {
            Endpoint = endpoint,
            Page = 1,
            PageSize = int.MaxValue,
            SortBy = request.SortBy?.Column,
            SortDirection = request.SortBy?.Direction ?? "asc",
        };

        if (request.Filters != null && request.Filters.Count > 0)
        {
            dataRequest.Filter = JsonSerializer.Serialize(request.Filters);
        }

        var gridData = await GetGridDataAsync(dataRequest);
        var columns = request.Columns != null && request.Columns.Count > 0
            ? gridData.Columns.Where(c => request.Columns.Contains(c.Id)).ToList()
            : gridData.Columns;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Data");

        for (int i = 0; i < columns.Count; i++)
        {
            worksheet.Cells[1, i + 1].Value = columns[i].Name;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }

        for (int row = 0; row < gridData.Rows.Count; row++)
        {
            var dataRow = gridData.Rows[row];
            for (int col = 0; col < columns.Count; col++)
            {
                var column = columns[col];
                var value = dataRow.ContainsKey(column.Id) ? dataRow[column.Id] : null;
                worksheet.Cells[row + 2, col + 1].Value = value;
            }
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<ExcelImportResponse> ImportFromExcelAsync(string endpoint, IFormFile file)
    {
        _logger.LogInformation("Importing Excel for endpoint: {Endpoint}", endpoint);

        var response = new ExcelImportResponse
        {
            Success = true,
            Imported = 0,
            Errors = new List<string>(),
        };

        // Parse endpoint
        var endpointParts = endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (endpointParts.Length == 3)
        {
            // Real form endpoint - use FormExcelImporter
            var namespaceId = endpointParts[0];
            var entityId = endpointParts[1];
            var formId = endpointParts[2];

            try
            {
                var user = GetCurrentUser();
                if (user == null)
                {
                    response.Success = false;
                    response.Errors!.Add("User not authenticated");
                    return response;
                }

                var culture = "fa";
                var form = FormStructRoutines.GetForm(namespaceId, entityId, formId, Neo.Bpms.Domain.Model.UI.Forms.Form.eFormType.Index, null);
                if (form == null)
                {
                    response.Success = false;
                    response.Errors!.Add("Form not found");
                    return response;
                }

                // Initialize importer
                _excelImporter.Init(user, culture, namespaceId, entityId, form.FormSubjectId);

                // Import from stream
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var result = await _excelImporter.Import(stream, file.ContentType, long.MaxValue, CancellationToken.None);

                // Calculate success and imported count from OperationResult
                var hasErrors = (result.ReadErrorList != null && result.ReadErrorList.Count > 0) ||
                               (result.ApplyErrors != null && result.ApplyErrors.Count > 0) ||
                               !string.IsNullOrEmpty(result.Fatal);
                
                response.Success = !hasErrors;
                
                // Calculate imported count from sheet overview
                if (result.OverviewResultPerSheet != null && result.OverviewResultPerSheet.Count > 0)
                {
                    response.Imported = (int)result.OverviewResultPerSheet
                        .Sum(s => s.TotalCount - s.ErrorCount);
                }
                
                if (result.ReadErrorList != null && result.ReadErrorList.Count > 0)
                {
                    response.Errors!.AddRange(result.ReadErrorList.Select(e => e.ErrorText));
                }
                if (result.ApplyErrors != null && result.ApplyErrors.Count > 0)
                {
                    response.Errors!.AddRange(result.ApplyErrors.Select(e => e.Message));
                }
                if (!string.IsNullOrEmpty(result.Fatal))
                {
                    response.Errors!.Add(result.Fatal);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing Excel using FormExcelImporter for endpoint: {Endpoint}", endpoint);
                response.Success = false;
                response.Errors!.Add(ex.Message);
                return response;
            }
        }

        // Fallback to simple import
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet == null)
            {
                response.Success = false;
                response.Errors!.Add("Worksheet not found");
                return response;
            }

            var columnMapping = new Dictionary<int, string>();
            var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
            var config = await GetGridConfigAsync(endpoint);

            foreach (var cell in headerRow)
            {
                var headerValue = cell.Value?.ToString();
                var column = config.Columns.FirstOrDefault(c => 
                    c.Name.Equals(headerValue, StringComparison.OrdinalIgnoreCase));
                if (column != null)
                {
                    columnMapping[cell.Start.Column] = column.Id;
                }
            }

            var batchUpdates = new List<CellChange>();
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var rowData = new Dictionary<string, object?>();
                var rowId = worksheet.Cells[row, 1].Value?.ToString();

                if (string.IsNullOrEmpty(rowId))
                    continue;

                foreach (var mapping in columnMapping)
                {
                    var columnId = mapping.Value;
                    var cellValue = worksheet.Cells[row, mapping.Key].Value;
                    rowData[columnId] = cellValue;
                }

                foreach (var kvp in rowData)
                {
                    batchUpdates.Add(new CellChange
                    {
                        RowId = rowId,
                        ColumnId = kvp.Key,
                        Value = kvp.Value,
                    });
                }

                response.Imported++;
            }

            if (batchUpdates.Count > 0)
            {
                var batchRequest = new BatchUpdateRequest
                {
                    Changes = batchUpdates,
                };
                var updateResult = await BatchUpdateAsync(endpoint, batchRequest);
                
                if (!updateResult.Success && updateResult.Errors != null)
                {
                    response.Errors!.AddRange(updateResult.Errors.Select(e => e.Message));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel");
            response.Success = false;
            response.Errors!.Add(ex.Message);
        }

        return response;
    }
}

