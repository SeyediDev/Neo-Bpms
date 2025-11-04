namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public abstract class Combo(IFormLogicHelper formLogicHelper, InputFieldDefinition field,
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : Choice(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    protected virtual void RenderHeader(CandoStringBuilder result, bool isRemoteData,
        bool isMultiple, List<string> idsList, bool isSubTable, string defaultValue = null)
    {
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
        result.Append(CommonProperties.ReadOnlyRelatedAttribute + " " +
              (isRemoteData ? $"data-isremote=\"true\" filter-formula=\"{Field.PropertyValue(eControlPropertyId.FilterFormula)}\"" : "") +
              (defaultValue != null ? $" data-default-value=\"{defaultValue}\"" : "") +
              (CommonProperties.IsRequired ? " required=\"required\" oninvalid=\"InvalidMsg(this);\" " : "") + " dir=\"" +
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
    
    protected override void RenderInnerOfLabel(CandoStringBuilder result, InputFieldDefinition field)
    {
        RenderRelatedLinks(result, true);
    }

    internal static string GetCustomRemoteDataUrl(InputFieldDefinition field, IUrlHelper url)
    {
        string controller = field.PropertyValue(eControlPropertyId.RemoteDataController);
        string action = field.PropertyValue(eControlPropertyId.RemoteDataAction);
        return controller == null || action == null ? null : $"data-remote-url=\"{Url.Action(action, controller, url)}\"";
    }

    protected CandoStringBuilder RenderFooter(CandoStringBuilder result)
    {
        result.Append("</select>");
        
        // اضافه کردن دکمه Clear برای کمبوها (فقط اگر ReadOnly نباشد)
        if (!CommonProperties.IsReadOnly && !ControlsRendererData.Options.IsFilter)
        {
            result.Append($"<button type=\"button\" class=\"combo-clear-btn\" " +
                     $"onclick=\"clearComboValue('{Field.FieldName}')\" " +
                     $"title=\"پاک کردن\">" +
                     $"<i class=\"fa fa-times\"></i>" +
                     $"</button>");
        }
        
        result.Append("</div>");
        return result;
    }
    protected static CandoStringBuilder RenderFooterEnd(CandoStringBuilder result)
    {
        result.Append("</div>");
        return result;
    }
}
