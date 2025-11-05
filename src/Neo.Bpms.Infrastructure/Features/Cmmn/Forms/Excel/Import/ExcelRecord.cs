global using OfficeOpenXml;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

public abstract class ExcelRecordBase
{
    public Dictionary<string, ExcelColumnValue> ColumnValues { get; set; }
    public string RelationshipKey { get; set; }
    public bool IsDelete { get; set; }
    public bool IsUpdate { get; set; }
    public object PrimaryKeyValue { get; set; }
    public void AddColumn(EntityField field, ExcelColumnValue value)
    {
        ColumnValues ??= [];
        ColumnValues.Add(field.AssociationEntity != null ? field.Id : field.DbFieldName, value);
    }
}

public class ExcelRecord : ExcelRecordBase
{
    public string ParentKey { get; set; }
}

public class ExcelColumnValue
{
    internal HeaderColumn Header { get; set; }
    public object Value { get; set; }
    internal RelationId RelationId { get; set; }

    public static ExcelColumnValue GetColumnValueEPPlus(ExcelRange cell, // تغییر اصلی: استفاده از ExcelRange به جای IExcelDataReader
        HeaderColumn headerColumn, RelationsData relationsData, OperationResult result, out bool isValid)
    {
        isValid = true;
        string excelValue = cell.Text?.Trim();
        object value = null;

        if (string.IsNullOrEmpty(excelValue))
            return null;

        Type type = headerColumn.Field?.CSharpType;
        Domain.Entities.Cmmn.Relationship.Association associationEntity = headerColumn.Field?.AssociationEntity;
        string sheetName = cell.Worksheet.Name; // نام برگه از خود سلول
        int rowNumber = cell.Start.Row; // شماره ردیف

        try
        {
            if (type == typeof(string))
            {
                if (headerColumn.Field.MaxLen == 0 || excelValue.Length <= headerColumn.Field.MaxLen)
                    value = ImportUtils.Normalize(excelValue);
                else
                {
                    AddError();
                    isValid = false;
                }
            }
            else if (type == typeof(TimeSpan))
            {
                string[] parts = excelValue.Split(':');
                if (parts.Length is 2 or 3)
                    value = ImportUtils.GetTimeSpan(excelValue);
                else
                {
                    AddError($"{Texts.TimeFormat} {Texts.Invalid}");
                    isValid = false;
                }
            }
            else if (type == typeof(DateTime))
            {
                if (DateTime.TryParse(excelValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    value = dt;
                else if (double.TryParse(excelValue, out double oaDate))
                    value = DateTime.FromOADate(oaDate); // پشتیبانی از فرمت عددی اکسل
                else
                {
                    AddError($"{Texts.Date} {Texts.Invalid}");
                    isValid = false;
                }
            }
            else if (type == typeof(long))
            {
                if (long.TryParse(excelValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long lVal))
                    value = lVal;
                else
                {
                    AddError($"{Texts.Number} {Texts.Invalid}");
                    isValid = false;
                }
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(excelValue, NumberStyles.Any, CultureInfo.InvariantCulture, out int iVal))
                    value = iVal;
                else
                {
                    AddError($"{Texts.Number} {Texts.Invalid}");
                    isValid = false;
                }
            }
            else if (type == typeof(double))
            {
                if (double.TryParse(excelValue, NumberStyles.Number, CultureInfo.InvariantCulture, out double dVal))
                    value = dVal;
                else
                {
                    AddError($"{Texts.DecimalNumber} {Texts.Invalid}");
                    isValid = false;
                }
            }
            else if (type == typeof(bool))
            {
                if (TryParseBool(excelValue, out bool bVal))
                    value = bVal;
                else
                {
                    AddError($"{Texts.Boolean} {Texts.Invalid}");
                    isValid = false;
                }
            }
            else if (associationEntity != null)
            {
                value = excelValue; // مدیریت موجودیت‌های وابسته
            }
        }
        catch (Exception ex)
        {
            AddError($"خطای پردازش: {ex.Message}");
            isValid = false;
        }

        ExcelColumnValue excelColumnValue = new()
        {
            Header = headerColumn,
            Value = value ?? excelValue // بازگرداندن مقدار خام در صورت خطا
        };

        if (associationEntity != null)
        {
            relationsData.AddRelationsDataRequest(
                headerColumn,
                associationEntity,
                excelColumnValue
            );
        }

        return excelColumnValue;

        void AddError(string message = null)
        {
            result.AddError( sheetName, rowNumber, headerColumn, message ?? $"{Texts.MaxLength} {Texts.Invalid}");
        }
    }

    private static bool TryParseBool(string value, out bool result)
    {
        value = value.ToLower();
        if (value == "true" || value == "1" || value == "بله")
        {
            result = true;
            return true;
        }
        if (value == "false" || value == "0" || value == "خیر")
        {
            result = false;
            return true;
        }
        result = false;
        return false;
    }

    public void SetToRelationsData(EntityField field, RelationsData relationsData)
    {
        if (Value is string strVal)
            Value = ImportUtils.Normalize(strVal);

        Domain.Entities.Cmmn.Relationship.Association associationEntity = field?.AssociationEntity;
        if (associationEntity != null)
        {
            relationsData.AddRelationsDataRequest(
                new HeaderColumn { Field = field },
                associationEntity,
                this
            );
        }
    }
}
