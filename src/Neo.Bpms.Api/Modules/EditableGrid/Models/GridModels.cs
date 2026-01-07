namespace Neo.Bpms.Api.Modules.EditableGrid.Models;

public class GridDataRequest
{
    public string Endpoint { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public string? Filter { get; set; }
}

public class GridDataResponse
{
    public List<GridColumn> Columns { get; set; } = [];
    public List<Dictionary<string, object?>> Rows { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GridColumn
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "text"; // text, number, date, boolean, select, multiselect
    public bool Editable { get; set; } = true;
    public bool Required { get; set; } = false;
    public List<SelectOption>? Options { get; set; } // For select/multiselect
    public string? Format { get; set; } // For date/number formatting
    public int? Width { get; set; }
}

public class SelectOption
{
    public object Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class BatchUpdateRequest
{
    public List<CellChange> Changes { get; set; } = [];
}

public class CellChange
{
    public object RowId { get; set; } = string.Empty;
    public string ColumnId { get; set; } = string.Empty;
    public object? Value { get; set; }
}

public class BatchUpdateResponse
{
    public bool Success { get; set; }
    public int Updated { get; set; }
    public List<UpdateError>? Errors { get; set; }
}

public class UpdateError
{
    public object RowId { get; set; } = string.Empty;
    public string ColumnId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class GridConfigResponse
{
    public List<GridColumn> Columns { get; set; } = [];
    public bool EnableEditing { get; set; } = true;
    public bool EnablePagination { get; set; } = true;
    public bool EnableSorting { get; set; } = true;
    public bool EnableFiltering { get; set; } = true;
    public RowActionsConfig? RowActions { get; set; }
}

public class RowActionsConfig
{
    public bool HasDetails { get; set; }
    public bool HasEdit { get; set; }
    public bool HasDelete { get; set; }
    public string? DetailFormId { get; set; }
    public string? EditFormId { get; set; }
    public string? DeleteFormId { get; set; }
    public string? DetailAction { get; set; }
    public string? EditAction { get; set; }
    public List<SubjectFormLink>? SubjectForms { get; set; }
    public List<SpecificLinkColumn>? SpecificLinks { get; set; }
}

public class SubjectFormLink
{
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public bool HasEditForm { get; set; }
    public bool HasDetailsForm { get; set; }
    public string? EditFormId { get; set; }
    public string? DetailFormId { get; set; }
    public string? EditAction { get; set; }
    public string? DetailAction { get; set; }
}

public class SpecificLinkColumn
{
    public string Label { get; set; } = string.Empty;
    public string LinkTarget { get; set; } = string.Empty;
    public Dictionary<string, string>? LinkParameters { get; set; }
}

public class ExcelExportRequest
{
    public List<FilterCondition>? Filters { get; set; }
    public SortOption? SortBy { get; set; }
    public List<string>? Columns { get; set; }
}

public class FilterCondition
{
    public string ColumnId { get; set; } = string.Empty;
    public string Operator { get; set; } = "equals";
    public object? Value { get; set; }
}

public class SortOption
{
    public string Column { get; set; } = string.Empty;
    public string Direction { get; set; } = "asc";
}

public class ExcelImportResponse
{
    public bool Success { get; set; }
    public int Imported { get; set; }
    public List<string>? Errors { get; set; }
}

