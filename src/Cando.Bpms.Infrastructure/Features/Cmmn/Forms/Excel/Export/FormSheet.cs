using System.Drawing;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;
using OfficeOpenXml.Style;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;

public class FormSheet
{
    //internal bool _writeRecords;
    private ExcelWorksheet _worksheet;
    private int _iRow;

    private readonly ExcelPackage _package;
    private readonly Form _form;
    private readonly CommonFormStructure _commonFormStructure;
    private readonly string _name;
    private readonly bool _isRightToLeft;
    private readonly bool _isChild;
    private readonly string _sourceField;
    private List<(int colNo, string colName)> ColList { get; set; } = [];
    public bool IsChild => _isChild;
    public Form Form => _form;
    public CommonFormStructure Structure => _commonFormStructure;
    public FormSheet(ExcelPackage package, Form form, CommonFormStructure commonFormStructure,
        bool isRightToLeft, bool isChild, string sourceField)
    {
        _package = package;
        _form = form;
        _commonFormStructure = commonFormStructure;
        _name = commonFormStructure.Name;
        _isRightToLeft = isRightToLeft;
        _isChild = isChild;
        _sourceField = sourceField;
        _iRow = 1;
    }

    internal int CreateFormSheet(bool setData)
    {
        _worksheet = _package.Workbook.Worksheets.Add(_name);
        _worksheet.Cells.AutoFitColumns(0);
        _worksheet.View.RightToLeft = _isRightToLeft;

        return CreateHeader(setData);
    }

    internal void CreateRecords(FormExportData data, int colCount)
    {
        CreateRecordRows(data, colCount);
    }

    private int CreateHeader(bool setData)
    {
        int iCol = 1;
        string keyField = _form.Entity.KeyFields?.FirstOrDefault()?.Id;
        if (setData)
            if (_form.Entity.KeyFields?.Count() > 1)
                throw new NotImplementedException("Entity has more than one primary key");
            else
                SetHeader(iCol++, Texts.PrimaryKey, ExcelHeaderReservedIds.PrimaryKey, keyField);
        if (_isChild)
            SetHeader(iCol++, Texts.ParentKey, ExcelHeaderReservedIds.ParentKey, _sourceField);
        if (!_isChild &&
            _commonFormStructure.Tables.Any(t => t.Editable && t.ControlType == eControlTypeId.IndexTable))
            SetHeader(iCol++, Texts.RelationshipKey, ExcelHeaderReservedIds.RelationshipKey, keyField);

        foreach (InputFieldDefinition formField in GetFormFields())
        {
            SetHeader(iCol, formField.Label, formField.FieldName);
            if (formField.ControlType.In(eControlTypeId.DurationInput, eControlTypeId.TimeInput))
                SetCell(_iRow + 2, iCol, $"{Texts.TimeFormat} hh:mm:ss/hh:mm");
            if (formField.ControlType.In(eControlTypeId.DateTime, eControlTypeId.DatePicker))
                SetCell(_iRow + 2, iCol, $"{Texts.Date} : {Texts.GregorianCalendar}");
            iCol++;
        }

        foreach (TableDefinition formField in GetFormMultiComboTables())
        {
            object label = GetMultiComboLabel(formField);
            SetHeader(iCol, label, formField.FieldName);
            SetCell(_iRow + 2, iCol, Texts.MultipleComboWithCommaSeperator);
            iCol++;
        }

        ExcelStyle style = _worksheet.Cells[1, 1, _iRow, iCol - 1].Style;
        style.Font.Bold = true;
        SetStyle(style, Color.Black, ExcelFillStyle.Solid, Color.BurlyWood);
        //_worksheet.Cells[$"A{_iRow+1}:XFD{_iRow+1}"].Style.Hidden = true; //hide key row
        _iRow += 2;
        return iCol - 1;
    }

    private object GetMultiComboLabel(TableDefinition formField)
    {
        return formField.GetProperty(_isRightToLeft
                   ? eControlPropertyId.LabelName
                   : eControlPropertyId.EnLabelName)?.Value
               ??
               formField.GetProperty(_isRightToLeft
                   ? eControlPropertyId.EnLabelName
                   : eControlPropertyId.LabelName)?.Value
               ?? formField.Label;
    }

    private void CreateRecordRows(FormExportData data, int colCount)
    {
        int firstRow = _iRow;
        IndexFormData result = data.Result;
        if (result.recordCount < 1) return;

        foreach (ElasticObject record in result.Rows)
        {
            CreateRecordRow(record, _iRow);
            _iRow++;
        }

        ExcelStyle style = _worksheet.Cells[firstRow, 1, (int)(firstRow + result.recordCount - 1), colCount].Style;
        SetStyle(style, Color.Black, ExcelFillStyle.Solid, Color.AliceBlue);
    }

    private void CreateRecordRow(ElasticObject record, int iRow)
    {
        foreach ((int colNo, string colName) col in ColList)
        {
            if (record.GetField(col.colName, out object value))
                SetCell(iRow, col.colNo, value);
        }
    }

    private static void SetStyle(ExcelStyle style, Color color, ExcelFillStyle patternType, Color backgroundColor)
    {
        style.Font.Color.SetColor(color);
        style.Fill.PatternType = patternType;
        style.Fill.BackgroundColor.SetColor(backgroundColor);
        style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }

    private IEnumerable<InputFieldDefinition> GetFormFields()
    {
        return _commonFormStructure.Fields.Where(f => f.FormFieldType == FormField.Type.Field);
    }

    private IEnumerable<TableDefinition> GetFormMultiComboTables()
    {
        return _commonFormStructure.Tables.Where(f => f.ControlType == eControlTypeId.MultipleSelectableCombo);
    }

    private void SetCell(int iRow, int iCol, object value)
    {
        _worksheet.Cells[iRow, iCol].Value = value;
    }

    private void SetHeader(int iCol, object label, object key, string fieldId = null)
    {
        if (!string.IsNullOrEmpty(fieldId))
            ColList.Add((iCol, fieldId));
        else
            ColList.Add((iCol, key.ToString()));
        SetHeaderCell(iCol, label, key);
    }

    private void SetHeaderCell(int iCol, object label, object key)
    {
        _worksheet.Cells[_iRow, iCol].Value = label;
        _worksheet.Cells[_iRow + 1, iCol].Value = key;
    }
}
