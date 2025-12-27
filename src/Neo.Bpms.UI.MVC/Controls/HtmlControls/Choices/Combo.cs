using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public abstract class Combo(IFormLogicHelper formLogicHelper, InputFieldDefinition field,
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : Choice(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    protected virtual void RenderHeader(NeoStringBuilder result, bool isRemoteData,
        bool isMultiple, List<string> idsList, bool isSubTable, string defaultValue = null)
    {
        if (!isRemoteData && !string.IsNullOrWhiteSpace(Field.PropertyValue(eControlPropertyId.NamespaceId))
            && !string.IsNullOrWhiteSpace(Field.PropertyValue(eControlPropertyId.EntityId)))
        {
            isRemoteData = true;
        }

        result.Append($"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip + "\" class=\"" +
              ControlsRendererData.ControlsClassString +
                CommonProperties.NarrowColumnClasses +
                CommonProperties.ShowHideRelatedClass +
                "\">");

        //RenderDesignIcons(result);
        result.Append("<div class=\"dWrapper\">");
        RenderBulkEditCheckbox(result);
        RenderLabel(result);
        result.Append($"<select id=\"field-{Field.FieldName}\"");
        if (isMultiple)
            result.Append(" multiple=\"true\" ");
        string selectName = isMultiple && ControlsRendererData.Options.IsFilter ? $"{Field.FieldName}[]" : Field.FieldName;
        string placeholder = Field.PropertyValue(eControlPropertyId.LabelName)
            ?? Field.PropertyValue(eControlPropertyId.EnLabelName)
            ?? Field.Label
            ?? Field.FieldName;
        string encodedPlaceholder = ControlsRendererData.Encoder.Encode(placeholder ?? string.Empty);
        string placeholderAttribute = string.IsNullOrWhiteSpace(encodedPlaceholder)
            ? string.Empty
            : $" data-placeholder=\"{encodedPlaceholder}\"";
        string allowClearAttribute = !CommonProperties.IsRequired ? " data-allow-clear=\"true\"" : string.Empty;

        result.Append(CommonProperties.ReadOnlyRelatedAttribute + " " +
              (isRemoteData ? $"data-isremote=\"true\" filter-formula=\"{Field.PropertyValue(eControlPropertyId.FilterFormula)}\"" : "") +
              (defaultValue != null ? $" data-default-value=\"{defaultValue}\"" : "") +
              (CommonProperties.IsRequired ? " required=\"required\" oninvalid=\"InvalidMsg(this);\" " : "") +
              placeholderAttribute + allowClearAttribute + " dir=\"" +
                  CommonProperties.Direction + "\" title=\"" + CommonProperties.Tooltip + "\" name=\"" + selectName +
                  (ControlsRendererData.Options.IsFilter ? "\" form=\"filter-form" : "") +
                  "\" class=\"cBox\" " +
                  GetCustomRemoteDataUrl(Field, ControlsRendererData.Url) +
                  LogicString);
        if (idsList != null && idsList.Count > 0)
            result.Append(" initValue=\"" + string.Join(",", idsList) + "\"");
        else if (!ControlsRendererData.Options.IsFilter && CommonProperties.IsRequired)
            result.Append(" nodatamandatory");
        if (isSubTable)
            result.Append(" subtable");
        result.Append(" >");
    }
    
    protected override void RenderInnerOfLabel(NeoStringBuilder result, InputFieldDefinition field)
    {
        RenderRelatedLinks(result, true);
    }

    internal static string GetCustomRemoteDataUrl(InputFieldDefinition field, IUrlHelper url)
    {
        string controller = field.PropertyValue(eControlPropertyId.RemoteDataController);
        string action = field.PropertyValue(eControlPropertyId.RemoteDataAction);
        return controller == null || action == null ? null : $"data-remote-url=\"{Url.Action(action, controller, url)}\"";
    }

    protected NeoStringBuilder RenderFooter(NeoStringBuilder result)
    {
        result.Append("</select>");
        // Note: Clear button is handled by Select2's data-allow-clear attribute
        result.Append("</div>");
        return result;
    }
    protected static NeoStringBuilder RenderFooterEnd(NeoStringBuilder result)
    {
        result.Append("</div>");
        return result;
    }
}
