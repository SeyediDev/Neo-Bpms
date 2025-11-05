namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

public class MainExcelRecord : ExcelRecordBase
{
    public Dictionary<string, List<ExcelRecord>> SubTableRecords { get; set; }
    public Dictionary<string, List<ExcelRecord>> MultiComboRecords { get; set; }
}