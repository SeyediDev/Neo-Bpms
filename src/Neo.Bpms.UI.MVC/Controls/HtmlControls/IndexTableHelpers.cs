using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

internal static class IndexTableHelpers
{

    internal static string GetTimeSpanValue(object value)
    {
        TimeSpan timespan = new(Convert.ToInt64(value));
        return timespan.ToTimeInputValue();
    }

    internal static string FetchRowId(TableDefinition table, ElasticObject row)
    {
        string rowIds = "";
        foreach (string key in table.KeyFields)
        {
            row.GetField(key, out object rowId);
            if (!string.IsNullOrEmpty(rowIds))
                rowIds += ",";
            //rowId can be null when  ConvertPostedElasticToRecord() is called
            rowIds += rowId?.ToString();
        }

        return rowIds;
    }

    internal static string FormLinkClass(string additionalClass) =>
        $"class=\"icon-link form-link table-form-link {additionalClass}\"";

    internal static string GetInputType(ColumnFieldDefinition col)
    {
        return col.FieldType == TVariableTypes.Double ||
               col.FieldType == TVariableTypes.Int ||
               col.FieldType == TVariableTypes.Long ||
               col.FieldType == TVariableTypes.Decimal ||
               col.FieldType == TVariableTypes.Short
            ? "number"
            : "text";
    }

    internal static string GetStyle(ColumnFieldDefinition col, bool isEditable)
    {
        return col.HasProperty(eControlPropertyId.WidthPercentage)
            ? Style($"width: {col.Property(eControlPropertyId.WidthPercentage)}%;")
            : !isEditable || col.ControlType == eControlTypeId.CheckBox
                ? ""
                : MinWidthStyle(col.ControlType == eControlTypeId.DatePicker ? 175 : 150);
        string MinWidthStyle(int width)
        {
            return Style($"min-width: {width}px;");
        }

        string Style(string content)
        {
            return $"style=\"{content}\"";
        }
    }

    internal static object GetUrlObject(TableDefinition table, CommonFormStructure structure,
        string recordId, string rowIds, string formId)
    {
        var urlObject =
            new
            {
                table.NamespaceId,
                EntityId = table.PropertyValue(eControlPropertyId.FormEntityId) ?? table.EntityId,
                table.FormSubjectId,
                FormId = formId,
                workItemFormId = structure.Form_ReportId,
                pFormId = structure.Form_ReportId,
                __parentNamespaceId = structure.NamespaceId,
                __parentEntityId = structure.EntityId,
                __parentFormSubjectId = structure.FormSubjectId,
                __subTableAssociationFieldId = table.TableDef.TableAssociation?.Id,
                __parentIds = recordId,
                ids = rowIds,
                bSubTable = true,
                isReturnable = true
            };
        return urlObject;
    }
}
