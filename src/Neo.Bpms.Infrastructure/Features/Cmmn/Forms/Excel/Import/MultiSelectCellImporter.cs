using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

internal class MultiSelectCellImporter(FormField tableTableDef, EntityField field)
{
    public void ReadDataEPPlus(ExcelRange cell, // سلول مورد نظر از EPPlus
        MainExcelRecord parentRecord, HeaderColumn item, RelationsData relationsData)
    {
        if (parentRecord == null || cell == null)
            return;

        // خواندن مقدار سلول و تبدیل به آرایه
        string cellValue = cell.Text?.Trim();
        if (string.IsNullOrEmpty(cellValue))
            return;

        IEnumerable<string> values = cellValue.Split(',').Select(v => v.Trim());

        foreach (string value in values)
        {
            ExcelColumnValue excelColumnValue = new()
            {
                Value = value,
                Header = item,
                RelationId = null
            };

            excelColumnValue.SetToRelationsData(field, relationsData);

            ExcelRecord record = new();
            record.AddColumn(field, excelColumnValue);

            parentRecord.MultiComboRecords ??= [];

            if (!parentRecord.MultiComboRecords.ContainsKey(tableTableDef.Id))
            {
                parentRecord.MultiComboRecords[tableTableDef.Id] = [];
            }

            parentRecord.MultiComboRecords[tableTableDef.Id].Add(record);
        }
    }
}
