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
