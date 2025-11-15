using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public class ComboBox(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : Combo(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        ElasticObject record = ControlsRendererData.Record;
        bool isMultiple = ControlsRendererData.Options.IsFilter &&
                         !Field.PropertyBoolean(eControlPropertyId.IsNotMultiple);
        List<string> idsList = GatherIdsList(record);
        bool isRemoteData = Field.GetProperty(eControlPropertyId.RemoteData)?.GetValueAsBoolean() ?? false;

        NeoStringBuilder result = new();
        
        // اگر multiple است و در حالت فیلتر است، از کنترل مدرن استفاده کن
        /*if (isMultiple && ControlsRendererData.Options.IsFilter)
        {
            RenderModernMultiSelect(result, idsList?.ToList(), isRemoteData);
        }
        else*/
        {
            RenderHeader(result, isRemoteData, isMultiple, idsList?.ToList(), false);
            RenderOptions(result, idsList);
            RenderFooter(result);
            RenderFooterEnd(result);
        }
        
        return result;
    }
    /*
    private void RenderModernMultiSelect(NeoStringBuilder result, List<string> idsList, bool isRemoteData)
    {
        // Container اصلی
        result.Append($"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip + "\" class=\"" +
                  ControlsRendererData.ControlsClassString +
                  CommonProperties.NarrowColumnClasses +
                  CommonProperties.ShowHideRelatedClass +
                  "\">");

        result.Append("<div class=\"dWrapper\">");
        result = RenderBulkEditCheckbox(result);
        result = RenderLabel(result);
        
        // کنترل مدرن اختصاصی (بدون select)
        RenderCustomModernMultiSelect(result, idsList, isRemoteData);
        
        result.Append("</div>");
        result.Append("</div>");
    }

    private void RenderCustomModernMultiSelect(NeoStringBuilder result, List<string> idsList, bool isRemoteData)
    {
        var selectedItems = GetSelectedItems(idsList);
        var allOptions = GetDataRows().ToList();
        
        // Hidden input برای نگهداری مقادیر انتخاب شده
        string id = string.Join(",", idsList ?? []);
        result.Append($"<input type=\"hidden\" id=\"field-{Field.FieldName}\" name=\"{Field.FieldName}[]\" " +
                  $"value=\"{id}\" " +
                  (CommonProperties.IsRequired ? " required=\"required\" " : "") +
                  LogicString + " />");
        
        // Container کنترل مدرن
        result.Append("<div class=\"modern-multi-select-control\" " +
                  $"data-field-name=\"{Field.FieldName}\" " +
                  $"data-is-remote=\"{isRemoteData.ToString().ToLower()}\" " +
                  (isRemoteData ? $"data-filter-formula=\"{Field.PropertyValue(eControlPropertyId.FilterFormula)}\"" : "") +
                  $"data-is-required=\"{CommonProperties.IsRequired.ToString().ToLower()}\" " +
                  $"data-direction=\"{CommonProperties.Direction}\" " +
                  $"data-tooltip=\"{CommonProperties.Tooltip}\" " +
                  GetCustomRemoteDataUrl(Field, ControlsRendererData.Url) +
                  ">");
        
        // Input container
        result.Append("<div class=\"modern-multi-select-input\" tabindex=\"0\">");
        
        // Selected items container
        result.Append("<div class=\"modern-multi-select-items\">");
        foreach (var item in selectedItems)
        {
            result.Append($"<div class=\"modern-multi-select-item\" data-value=\"{ControlsRendererData.Encoder.Encode(item.Ids)}\">");
            result.Append($"<span class=\"modern-multi-select-item-text\">{ControlsRendererData.Encoder.Encode(item.DisplayValue)}</span>");
            result.Append("<span class=\"modern-multi-select-item-remove\">×</span>");
            result.Append("</div>");
        }
        result.Append("</div>");
        
        // Search input
        result.Append("<input type=\"text\" class=\"modern-multi-select-search\" " +
                  "placeholder=\"جستجو...\" autocomplete=\"off\" />");
        
        // Dropdown arrow
        result.Append("<div class=\"modern-multi-select-arrow\">");
        result.Append("<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"currentColor\">");
        result.Append("<path d=\"M8 11L3 6h10l-5 5z\"/>");
        result.Append("</svg>");
        result.Append("</div>");
        
        result.Append("</div>"); // End input container
        
        // Dropdown menu
        result.Append("<div class=\"modern-multi-select-dropdown\">");
        result.Append("<div class=\"modern-multi-select-options\">");
        
        foreach (var option in allOptions)
        {
            var isSelected = selectedItems.Any(s => s.Ids == option.Ids);
            result.Append($"<div class=\"modern-multi-select-option {(isSelected ? "selected" : "")}\" " +
                      $"data-value=\"{ControlsRendererData.Encoder.Encode(option.Ids)}\">");
            result.Append("<div class=\"modern-multi-select-option-checkbox\"></div>");
            result.Append($"<div class=\"modern-multi-select-option-text\">{ControlsRendererData.Encoder.Encode(option.DisplayValue ?? "")}</div>");
            result.Append("</div>");
        }
        
        result.Append("</div>"); // End options
        result.Append("</div>"); // End dropdown
        
        result.Append("</div>"); // End control container
    }*/

    private List<FormDataRow> GetSelectedItems(List<string> idsList)
    {
        if (idsList == null || idsList.Count == 0)
            return [];
            
        var allOptions = GetDataRows().ToList();
        return allOptions.Where(option => idsList.Contains(option.Ids)).ToList();
    }

    private NeoStringBuilder RenderOptions(NeoStringBuilder result, List<string> idsList)
    {
        List<FormDataRow> dataRows = GetDataRows() ?? [];
        bool hasEmptyOption = dataRows.Any(row => string.IsNullOrEmpty(row?.Ids));
        if (!hasEmptyOption)
        {
            result.Append("<option value=\"\"></option>");
        }

        foreach (FormDataRow item in dataRows)
        {
            result.Append("<option");
            if (IsSelected(idsList, item.Ids))
                result.Append(" selected=\"selected\"");
            if (!string.IsNullOrEmpty(item.Ids))
                result.Append(" value=\"" + ControlsRendererData.Encoder.Encode(item.Ids) + "\"");
            result.Append(">" + ControlsRendererData.Encoder.Encode(item.DisplayValue ?? "") + "</option>");
        }

        return result;
    }
}
