using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Data;

public class OperationResult
{
    public List<SheetInfo> OverviewResultPerSheet { get; set; }
    private Dictionary<string, SingleMessage> ReadErrors { get; set; }
    public List<SingleMessage> ReadErrorList => ReadErrors?.Values.ToList();
    public List<ApplyErrorMessage> ApplyErrors { get; set; }
    public string Fatal { get; set; }

    public void AddApplyError(string forField, string errorText)
    {
        ApplyErrors ??= [];
        ApplyErrors.Add(new ApplyErrorMessage { ForField = forField, Message = errorText });
    }
    public void AddError(string sheetName, int rowId, HeaderColumn column, string errorText)
    {
        string headerName = column.FieldName;
        ReadErrors ??= [];
        ReadErrors.Add($"{sheetName}-{rowId}-{column.Index}",
            new SingleMessage
            { SheetName = sheetName, Row = rowId, HeaderName = headerName, ErrorText = errorText });
    }
    public void AddError(string excelName, HeaderColumn column, string errorText)
    {
        string headerName = column.FieldName;
        ReadErrors ??= [];
        ReadErrors.Add(Guid.NewGuid().ToString(),
            new SingleMessage
            { SheetName = excelName, Row = -1, HeaderName = headerName, ErrorText = errorText });
    }
    public void AddFatal(string eMessage)
    {
        Fatal = eMessage;
    }
    public void AddSheetOverviewResult(string name, long totalCount, long errorCount)
    {
        OverviewResultPerSheet ??= [];
        OverviewResultPerSheet.Add(new SheetInfo
        {
            SheetName = name,
            TotalCount = totalCount,
            ErrorCount = errorCount
        });
    }
    public bool HasError(string name, int row, int column)
    {
        return ReadErrors != null && ReadErrors.ContainsKey($"{name}-{row}-{column}");
    }
    public class SheetInfo
    {
        public string SheetName { get; set; }
        public long TotalCount { get; set; }
        public long ErrorCount { get; set; }
    }

    public class ApplyErrorMessage
    {
        public string ForField { get; set; }
        public string Message { get; set; }
    }

    public class SingleMessage
    {
        public string SheetName { get; set; }
        public int Row { get; set; }
        public string HeaderName { get; set; }
        public string ErrorText { get; set; }
    }
}
