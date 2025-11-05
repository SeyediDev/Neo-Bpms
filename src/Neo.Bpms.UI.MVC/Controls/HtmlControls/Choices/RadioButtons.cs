using Neo.Bpms.Domain.Extensions;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public class RadioButtons(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer) 
    : Choice(formLogicHelper, field, controlsRendererData, sbvrRenderer) // todo not a good abstraction(?)
{
    public override NeoStringBuilder Render()
    {
        ElasticObject record = ControlsRendererData.Record;

        List<string> idsList = GatherIdsList(record);

        NeoStringBuilder result = new();

        RenderTopDiv(result);
        RenderDesignIcons(result);
        result += "<div class=\"dWrapper\">";
        RenderBulkEditCheckbox(result);
        result += "<div>";
        RenderLabel(result);
        result += "</div>";
        //RenderRelatedLinks(result, true);

        result += @"<div class=""btn-group btn-group-toggle"" data-toggle=""buttons"">";

        RenderOptions(result, idsList);

        result += "</div></div></div>";
        //            result += "</div></div>";

        return result;
    }

    private NeoStringBuilder RenderTopDiv(NeoStringBuilder result)
    {
        result += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip + "\" class=\"" +
                  ControlsRendererData.ControlsClassString +
                  CommonProperties.NarrowColumnClasses +
                  CommonProperties.ShowHideRelatedClass +
                  "\">";
        return result;
    }

    protected virtual NeoStringBuilder RenderOptions(NeoStringBuilder result, List<string> idsList)
    {
        foreach (FormDataRow item in GetDataRows())
        {
            if (string.IsNullOrEmpty(item.Ids)) continue;
            result.Append(RenderOption(idsList, item.DisplayValue, item.Ids));
        }

        if (!CommonProperties.IsRequired) // todo test
            result.Append(RenderOption(idsList, ViewTexts.None, ""));

        return result;
    }

    protected string RenderOption(List<string> idsList, string displayValue, string idsValue,
        string selectedId = null, bool isDefaultValue = false)
    {
        bool isSelected = IsSelected(idsList, idsValue, selectedId);
        string selectedClass = isSelected ? "btn-primary" : "btn-outline-secondary";
        
        return
            $@"<label class=""btn btn-sm {selectedClass}"" style=""margin: 2px; border-radius: 6px; transition: all 0.2s ease;"">
	                  <input type=""radio"" 
{(CommonProperties.IsRequired ? "required =\"required\" oninvalid=\"InvalidMsg(this);\" " : "")}
dir=""{CommonProperties.Direction}"" title=""{CommonProperties.Tooltip}"" name=""{Field.FieldName}""
value=""{ControlsRendererData.Encoder.Encode(idsValue ?? "")}"" {(isSelected ? "checked=\"checked\"" : "")} 
{(isDefaultValue ? "data-default-value=\"checked\"" : "")} 
onchange=""fixBooleanRadioButtons('{Field.FieldName}')"" />
                {displayValue}</label>";
    }

    private static bool IsSelected(List<string> idsList, string itemIds, string selectedId)
    {
        // For BooleanRadioButtons, selectedId is passed directly
        if (!string.IsNullOrEmpty(selectedId))
        {
            return selectedId == itemIds;
        }
        
        // Fallback to original logic for other radio button types
        return (idsList?.Contains(itemIds) ?? false) && !string.IsNullOrEmpty(itemIds);
    }
}
