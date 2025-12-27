using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public abstract class Choice(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer) 
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    public virtual NeoStringBuilder RenderRelatedLinks(NeoStringBuilder result, bool recordBase)
    {
        var formIdentifier = new
        {
            Field.NamespaceId,
            Field.EntityId,
            Field.FormSubjectId,
        };
        return RenderRelatedLinksUrl(Field, result, recordBase, formIdentifier);
    }

    protected NeoStringBuilder RenderRelatedLinksUrl(InputFieldDefinition field, NeoStringBuilder result,
        bool recordBase, object formIdentifier)
    {
        if (!ControlsRendererData.Options.ShowFormLinkIconForChoices)
            return result;

        string flagColor = "blue";
        bool flagShow = false;
        if (field.FormFieldType == FormField.Type.FilterField && field.ControlType != eControlTypeId.ComboBox)
            flagShow = true;
        else if (field.FormFieldType == FormField.Type.Field && field.ControlType != eControlTypeId.MultipleSelectableCombo)
            flagShow = true;

        if (field.HasCreateForm && field.FormFieldType != FormField.Type.FilterField && field.FormFieldType != FormField.Type.ColumnField)
        {
            result = GenerateRelatedLink(result, formIdentifier, flagColor, 
                "Create", ViewTexts.Create, "", SvgFilter.Insert);
        }

        if (recordBase && field.HasDetailsForm && field.FormFieldType != FormField.Type.FilterField)
        {
            result = GenerateRelatedLink(result, formIdentifier, flagColor,
                "Details", ViewTexts.Details, "", SvgFilter.Info);
        }

        if (recordBase && field.HasEditForm && flagShow && field.FormFieldType != FormField.Type.FilterField)
        {
            result = GenerateRelatedLink(result, formIdentifier, flagColor,
                "Edit", ViewTexts.Edit, "", SvgFilter.Edit);
        }

        if (field.HasIndexForm )
        {
            result = GenerateRelatedLink(result, formIdentifier, flagColor,
                "Index", ViewTexts.Index, "", SvgFilter.List);
        }
        return result;
    }

    private NeoStringBuilder GenerateRelatedLink(
        NeoStringBuilder result, object formIdentifier, string flagColor, 
        string action, string title, string className, SvgFilter svgFilter)
    {
        string url = Url.Action(action, "Form", ControlsRendererData.Url, formIdentifier);
        result +=
            $"<a target=\"_blank\" href=\"{url}\" title=\"{title}\" " +
            $"class=\"choice-form-link {className}\" " +
            $"tabindex=\"-1\" onclick=\"event.stopPropagation();\">";
        result += svgFilter switch
        {
            SvgFilter.Insert => GetSvgTag("add-icon"),
            SvgFilter.Info => GetSvgTag("info"),
            SvgFilter.Edit => GetSvgTag("edit"),
            SvgFilter.Delete => GetSvgTag("delete"),
            SvgFilter.List => GetSvgTag("list"),
            _ => string.Empty,
        };
        result += "</a>";
        return result;
        string GetSvgTag(string svgPart)
            => $"<svg viewBox=\"0 0 16 16\">" +
               $"<use xlink:href=\"/Content/common-assets-includes/icons/svgSprite.svg#{svgPart}-{flagColor}\" />" +
               $"</svg>";
    }

    protected virtual List<string> GatherIdsList(ElasticObject record)
    {
        Entity entity = ProjectDefinition.Project.GetEntity(ControlsRendererData.Structure.NamespaceId,
            ControlsRendererData.Structure.EntityId);
        string[] fieldIds = Field.FieldName.Split('.');
        EntityField field = entity?.GetField(fieldIds[0]);
        string fieldId;
        if (fieldIds.Length > 1)
        {
            field = field.GetDotAssociatedFieldWithCompleteId(fieldIds,
                out fieldId);
        }
        else
            fieldId = field?.AssociationEntity?.Maps?[0].SourceField ?? field?.Id ?? string.Empty;

        string value = record?.GetString(fieldId);
        if (string.IsNullOrEmpty(value))
            value = record?.GetString(Field.FieldName);
        if (string.IsNullOrEmpty(value)) // todo value null
            value = CommonProperties.DefaultValue;
        List<string> idsList = !string.IsNullOrEmpty(value) ? value.Split(',').ToList() : [];
        if (string.IsNullOrEmpty(value) && record != null && field?.AssociationEntity?.Maps != null)
        {
            idsList = [];
            string rowIds = "";
            foreach (EntityRelationMap map in field.AssociationEntity.Maps)
            {
                string rowId;
                try
                {
                    rowId = record.GetString(map.SourceField);
                }
                catch (ArgumentNullException)
                {
                    throw new Exception($"فیلد مبدا در نگاشت فیلد {field.Id} تعریف نشده است.");
                }

                if (string.IsNullOrEmpty(rowId)) continue;
                if (!string.IsNullOrEmpty(rowIds)) rowIds += "#";
                rowIds += rowId;
            }

            if (!string.IsNullOrEmpty(rowIds))
                idsList.Add(rowIds);
        }

        return idsList;
    }

    protected bool IsSelected(List<string> idsList, string itemIds)
    {
        return (idsList?.Contains(itemIds) ?? false) && !string.IsNullOrEmpty(itemIds);
    }

    internal static IEnumerable<FormDataRow> PropertyOptions(InputFieldDefinition field)
    {
        return field.GetProperties(eControlPropertyId.Option)
            // 	.Where(p =>
            // {
            // 	if (Field.HasProperty(eControlPropertyId.FilterFormula))
            // 	{
            // 		var parsedFilter = Parser.Parse(Field.PropertyValue(eControlPropertyId.FilterFormula));
            // 		parsedFilter.Eval(null, new LocalParameters
            // 		{
            // 			{"q", ControlsRenderer.Record},
            // 			{Field.FieldName, p.Value}
            // 		});
            // 	}
            //
            // 	return true;
            // })
            .Select(p => new FormDataRow
            {
                Ids = p.Value.ToString(),
                DisplayValue = p.Value.ToString()
            });
    }

    protected List<FormDataRow> GetDataRows()
    {

        List<FormDataRow> result = [];

        if (ControlsRendererData.Structure.CombosData.TryGetValue(Field.FieldName, out ComboData value))
        {
            result = value.Rows;
        }
        else if (!Field.PropertyBoolean(eControlPropertyId.Required))
        {
            result = [new()];
        }

        result.AddRange(PropertyOptions(Field));

        // Fallback: در صفحات ReadOnly (Details/Delete) اگر دیتا لیست خالی بود،
        // مقدار فعلی رکورد را به عنوان یک گزینه نمایش بده تا حداقل نمایش درست شود
        if ((result == null || result.Count == 0) && ControlsRendererData.Record != null)
        {
            List<string> currentIds = GatherIdsList(ControlsRendererData.Record) ?? [];
            if (currentIds.Count > 0)
            {
                // تلاش برای پیدا کردن متن نمایشی (DisplayValue) از CombosData موجود
                List<FormDataRow> allKnownRows = ControlsRendererData.Structure.CombosData?.Values?
                    .SelectMany(cd => cd?.Rows ?? [])
                    .ToList() ?? [];

                List<FormDataRow> fallbackRows = [];
                foreach (string id in currentIds)
                {
                    string display = allKnownRows.FirstOrDefault(r => r?.Ids == id)?.DisplayValue ?? id;
                    fallbackRows.Add(new FormDataRow { Ids = id, DisplayValue = display });
                }

                result = fallbackRows;
            }
        }

        return result;
    }

    private enum SvgFilter
    {
        Insert,
        Info,
        Edit,
        Delete,
        List
    }

}
