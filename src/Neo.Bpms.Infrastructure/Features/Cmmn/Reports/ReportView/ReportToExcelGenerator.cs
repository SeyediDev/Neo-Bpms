using System.Drawing;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using OfficeOpenXml.Style;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.ReportView;

public class ReportToExcelGenerator : ReportViewGenerator
{
    public ReportToExcelGenerator(CancellationToken cancellationToken, CommonFormStructure structure,
        List<ColumnFieldDefinition> columnList, string culture, string calendar)
        : base(cancellationToken, structure, columnList, culture, calendar)
    {
    }

    public override object Content => _stream;
    private MemoryStream _stream;
    private ExcelPackage _package;
    private ExcelWorksheet _worksheet;
    private ExcelRange _cells;
    private int _excelRowIndex;
    private int _colIndex;
    private ElasticObject _totalRecord;
    private int _worksheetIndex;

    public override void Init()
    {
        base.Init();
        
        _stream = new MemoryStream();
        _package = new ExcelPackage(_stream);
    }

    public override void Release()
    {
        base.Release();
        _package.Dispose();
        _package = null;
        _stream = null;
    }

    public override void GenerateTotalRow(ElasticObject totalRecord)
    {
        _totalRecord = totalRecord;
    }

    protected override void GenerateRowsHeader(IList<ReportRowInfo> rowList)
    {
        _worksheet = _package.Workbook.Worksheets.Add($"Page {_worksheetIndex}");
        _worksheet.View.RightToLeft =
            Culture == "fa" || string.IsNullOrEmpty(Culture); // todo CultureHelper.IsRightToLeft()
                                                              // تعیین محدوده سلول‌ها و اعمال استایل
        int startRow = 1;
        int startCol = 1;
        int endRow = startRow + rowList.Count;
        int endCol = startCol + ColumnList.Count;
        _cells = _worksheet.Cells[startRow, startCol, endRow, endCol];
        SetExcelRangeStyle(_cells);
        _worksheetIndex++;
        _excelRowIndex = 1;
        GenerateHeaderPerSheet();
        WriteTotalRecord();
    }
    
    protected override void GenerateRowsFooter(IList<ReportRowInfo> rowList)
    {
        _cells.AutoFitColumns(0); //Auto-fit columns for all cells
        _worksheet.Protection.SetPassword("Neo_SJS_$#");
    }

    protected override void GenerateRowHeader(ReportRowInfo row)
    {
        _colIndex = 1;
        //var worksheetRow = _worksheet.Row(_excelRowIndex);
        //SetRowStyle(worksheetRow);
    }

    protected override void GenerateRowIndex(IList<ReportRowInfo> rowList)
    {
        ExcelRange cell = _cells[_excelRowIndex, _colIndex++];
        //SetColumnStyle(cell, null);
        cell.Value = RowIndex;
    }

    protected override void GenerateRowColumn(ColumnFieldDefinition column, ReportRowInfo row)
    {
        SetDataCel(_cells[_excelRowIndex, _colIndex++], row.Data, column);
    }

    protected override void GenerateRowFooter(ReportRowInfo row)
    {
        _excelRowIndex++;
    }

    public override void GenerateHeader()
    {
        _worksheetIndex = 1;
    }

    public override void GenerateFooter()
    {
        _package.Workbook.Properties.Title = Structure.Name;
        _package.Workbook.Properties.Author = "تحقیق و توسعه ارتباط - تتا";
        _package.Workbook.Properties.Company = "شرکت تحقیق و توسعه ارتباط";
        // set some extended property values
        _package.Compression = CompressionLevel.BestSpeed;
        _package.Save();
    }

    private void GenerateHeaderPerSheet()
    {
        int i = 1;
        ExcelRange cell = _cells[_excelRowIndex, i++];
        SetHeaderStyle(cell);
        cell.Value = CulturalTexts.Row;
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            cell = _cells[_excelRowIndex, i++];
            cell.Value = column.Alias;
            SetHeaderStyle(cell);
        }

        _excelRowIndex++;
    }

    private void WriteTotalRecord()
    {
        if (_totalRecord == null) return;
        int i = 1;
        ExcelRange cell = _cells[_excelRowIndex, i++];
        cell.Value = CulturalTexts.Total;
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            SetDataCel(_cells[_excelRowIndex, i++], _totalRecord, column);
        }

        _excelRowIndex++;
        _totalRecord = null;
    }

    private void SetDataCel(ExcelRange cell, ElasticObject row, ColumnFieldDefinition column)
    {
        //SetColumnStyle(cell, column);
        object value = ReportRenderer.GetValue(column, row);
        if (value == null) return;
        if (column.IsLongParam() || column.FieldType == TVariableTypes.Double)
        {
            try
            {
                if (!string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    if (double.TryParse(value.ToString(), out _))
                        cell.Value = Convert.ToDecimal(value);
                    else
                        cell.Value = FormDataRoutines.GetCellElementValue(value, column.FieldType, Calendar);
                }
            }
            catch
            {
                cell.Value = FormDataRoutines.GetCellElementValue(value, column.FieldType, Calendar);
            }
        }
        else
            cell.Value = FormDataRoutines.GetCellElementValue(value, column.FieldType, Calendar);
    }

    private static void SetColumnStyle(ExcelRange cell, InputFieldDefinition column)
    {
        ExcelStyle style = cell.Style;
        //style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
        style.Font.Bold = false;
        style.Font.Color.SetColor(Color.Black);
        style.Fill.PatternType = ExcelFillStyle.Solid;
        style.Fill.BackgroundColor.SetColor(Color.White);
        switch (column?.FieldType)
        {
            case TVariableTypes.Association:
                style.Fill.BackgroundColor.SetColor(Color.LightGoldenrodYellow);
                break;
        }
    }

    private static void SetHeaderStyle(ExcelRange cell)
    {
        ExcelStyle style = cell.Style;
        style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        style.Font.Bold = true;
        style.Font.Color.SetColor(Color.White);
        style.Fill.PatternType = ExcelFillStyle.Solid;
        style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
    }

    private static void SetRowStyle(ExcelRow row)
    {
        ExcelStyle style = row.Style;
        style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        style.Font.Bold = false;
        style.Font.Color.SetColor(Color.Black);
        style.Fill.PatternType = ExcelFillStyle.Solid;
        style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
    }
    private static void SetExcelRangeStyle(ExcelRange excelRange)
    {
        ExcelStyle style = excelRange.Style;
        style.Font.Bold = false;
        style.Font.Color.SetColor(Color.Black);
        style.Fill.PatternType = ExcelFillStyle.Solid;
        style.Fill.BackgroundColor.SetColor(Color.White);
    }
}
