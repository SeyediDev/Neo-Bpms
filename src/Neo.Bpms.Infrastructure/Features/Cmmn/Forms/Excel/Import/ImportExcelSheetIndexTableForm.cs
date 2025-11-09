using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

internal class SubtableSheetImporter
{
    public SubtableSheetImporter(Form form, CommonFormStructure formStructure, FormField tableTableDef)
    {
        _form = form;
        _formStructure = formStructure;
        _parentRelation = tableTableDef;
    }

    private readonly Form _form;
    private readonly CommonFormStructure _formStructure;
    private readonly FormField _parentRelation;
    private Dictionary<string, HeaderColumn> _headerColumns;

    internal string Name => _formStructure.Name.Length > 31 ?
        _formStructure.Name[..31] :
        _formStructure.Name;

    private UiEntity Entity => _form.Entity;

    public void ImportFromSubTableSheetEPPlus(
        ExcelWorksheet worksheet,
        OperationResult result,
        RelationsData relationsData,
        List<MainExcelRecord> mainExcelRecords,
        long maxRecords)
    {
        // خواندن هدرها با نسخه EPPlus
        _headerColumns = new ReadSheetHeader(Entity, _formStructure)
            .ReadEPPlus(worksheet, result, false);

        if (_headerColumns?.Count == 0)
            throw new ExcelImportException($"{Texts.InvalidExcelSheetHeader} {_formStructure.Name}");

        int records = ReadFromSheetEPPlus(
            worksheet,
            result,
            relationsData,
            mainExcelRecords,
            maxRecords,
            out int errorCount);

        result.AddSheetOverviewResult(_formStructure.Name, records, errorCount);
    }

    private int ReadFromSheetEPPlus(
        ExcelWorksheet worksheet,
        OperationResult result,
        RelationsData relationsData,
        List<MainExcelRecord> mainExcelRecords,
        long maxRecords,
        out int errorCount)
    {
        errorCount = 0;
        int totalRecords = 0;
        int startRow = worksheet.Dimension.Start.Row + 1; // شروع بعد از ردیف هدر

        for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
        {
            if (totalRecords >= maxRecords) break;

            bool success = ReadRecordFromSheetEPPlus(
                worksheet,
                row,
                result,
                relationsData,
                mainExcelRecords);

            totalRecords++;
            if (!success) errorCount++;
        }

        return totalRecords;
    }

    private bool ReadRecordFromSheetEPPlus(
        ExcelWorksheet worksheet,
        int row,
        OperationResult result,
        RelationsData relationsData,
        List<MainExcelRecord> mainExcelRecords)
    {
        ExcelRecord record = new();
        bool isValid = true;

        foreach (HeaderColumn item in _headerColumns.Values)
        {
            ExcelRange cell = worksheet.Cells[row, item.Index + 1]; // تبدیل به ایندکس ۱-بنیاد
            ExcelColumnValue value = ExcelColumnValue.GetColumnValueEPPlus(
                cell,
                item,
                relationsData,
                result,
                out bool isCellValid);

            if (!isCellValid) isValid = false;

            switch (item.FieldName)
            {
                case ExcelHeaderReservedIds.RelationshipKey:
                    record.RelationshipKey = value?.Value?.ToString();
                    break;
                case ExcelHeaderReservedIds.ParentKey:
                    record.ParentKey = value?.Value?.ToString();
                    break;
                case ExcelHeaderReservedIds.Delete:
                    record.IsDelete = Convert.ToInt16(value?.Value ?? 0) > 0;
                    break;
                case ExcelHeaderReservedIds.PrimaryKey:
                    record.AddColumn(item.Field, value);
                    record.PrimaryKeyValue = value?.Value;
                    record.IsUpdate = true;
                    break;
                default:
                    record.AddColumn(item.Field, value);
                    break;
            }
        }

        if (!isValid || string.IsNullOrEmpty(record.ParentKey))
            return false;

        MainExcelRecord parentRecord = mainExcelRecords
            .FirstOrDefault(r => r.RelationshipKey == record.ParentKey);

        if (parentRecord == null)
            return false;

        parentRecord.SubTableRecords ??= [];

        if (!parentRecord.SubTableRecords.ContainsKey(_parentRelation.Id))
            parentRecord.SubTableRecords[_parentRelation.Id] = [];

        parentRecord.SubTableRecords[_parentRelation.Id].Add(record);
        return true;
    }
}
