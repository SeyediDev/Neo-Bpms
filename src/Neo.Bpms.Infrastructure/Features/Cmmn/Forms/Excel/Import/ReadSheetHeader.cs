using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

internal partial class ReadSheetHeader(Entity entity, CommonFormStructure structure)
{
    internal Dictionary<string, HeaderColumn> ReadEPPlus(ExcelWorksheet worksheet, OperationResult result, bool isMainSheet)
    {
        Dictionary<string, HeaderColumn> columnIndexes = [];
        bool headerRowFound = false;
        int startRow = 1; // ردیف شروع جستجو برای هدر

        // جستجو برای ردیف هدر
        for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
        {
            columnIndexes.Clear();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                string cellValue = worksheet.Cells[row, col].Text?.Trim();
                if (string.IsNullOrWhiteSpace(cellValue))
                    continue;

                string headerId = ImportUtils.Normalize(cellValue);
                switch (headerId)
                {
                    case ExcelHeaderReservedIds.RelationshipKey:
                        AddHeader(columnIndexes, headerId, col, null);
                        break;
                    case ExcelHeaderReservedIds.Delete:
                        AddHeader(columnIndexes, headerId, col, null);
                        break;
                    case ExcelHeaderReservedIds.ParentKey:
                        AddHeader(columnIndexes, headerId, col, null);
                        break;
                    case ExcelHeaderReservedIds.PrimaryKey:
                        HandlePrimaryKey(columnIndexes, worksheet.Name, col);
                        break;
                    default:
                        HandleCustomHeader(columnIndexes, headerId, col, worksheet.Name);
                        break;
                }
            }

            if (columnIndexes.Count > 0)
            {
                headerRowFound = true;
                break;
            }
        }

        return headerRowFound ? columnIndexes : throw new ExcelImportException("سرستون‌ها یافت نشدند");
    }

    private static void AddHeader(Dictionary<string, HeaderColumn> dict, string headerId, int columnIndex, EntityField field)
    {
        dict[headerId] = new HeaderColumn
        {
            Index = columnIndex - 1, // تطابق با ایندکس‌های مبتنی بر ۰
            Field = field,
            FieldName = headerId
        };
    }

    private void HandlePrimaryKey(Dictionary<string, HeaderColumn> columnIndexes, string sheetName, int columnIndex)
    {
        if (structure.KeyFields.Count != 1 || structure.KeyFields.FirstOrDefault() == null)
        {
            string errorMessage = structure.KeyFields.Count != 0
                ? $"Entity {structure.EntityId} has multiple primary keys"
                : $"Entity {structure.EntityId} has no primary key";
            throw new ExcelImportException(errorMessage);
        }

        AddHeader(columnIndexes,
            ExcelHeaderReservedIds.PrimaryKey,
            columnIndex,
            entity.KeyFields.First());
    }

    private void HandleCustomHeader(Dictionary<string, HeaderColumn> columnIndexes, string headerId, int columnIndex, string sheetName)
    {
        string[] headerParts = headerId.Split('.');
        EntityField entityField = entity.GetField(headerParts[0]) ??
            entity.entityFields.Values.FirstOrDefault(f => f.Name == headerId);

        InputFieldDefinition formField = structure.Fields.FirstOrDefault(ff =>
            ff.FieldName.Equals(headerId, StringComparison.OrdinalIgnoreCase)) ?? throw new ExcelImportException($"سرستون ناشناخته در صفحه {sheetName}: {headerId}");
        AddHeader(columnIndexes, headerId, columnIndex, entityField);
    }
}
