namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class DatePicker(IFormLogicHelper formLogicHelper, InputFieldDefinition field, ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        NeoStringBuilder result = new();
        DateTime? value = ControlsRendererData.Record?.GetNullableDateTime(Field.FieldName);

        result.Append($"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip + "\" class=\"" +
                  ControlsRendererData.ControlsClassString +
                         CommonProperties.NarrowColumnClasses +
                         CommonProperties.ShowHideRelatedClass +
                         "\">");
        RenderDesignIcons(result);
        result.Append("<div class=\"dWrapper\">");
        RenderBulkEditCheckbox(result, true);
        RenderLabel(result);
        result.Append($@"<input {CommonProperties.ReadOnlyRelatedAttribute} 
                      {(CommonProperties.IsRequired ? "required=\"required\" oninvalid=\"InvalidMsg(this);\" " : "")}
                      dir=""{CommonProperties.Direction}"" title=""{CommonProperties.Tooltip}"" 
                             type=""text"" id=""field-{Field.FieldName}"" autocomplete=""off""
                             associated-hidden-name=""{Field.FieldName}"" value=""{GetInitialValue(value)}""
                             class=""form-control dateField {ControlsRendererData.Options.CalendarType}"" {LogicString} />");
        result.Append($@"<input type=""hidden"" id=""{Field.FieldName}"" name=""{Field.FieldName}""  
                             {CommonProperties.ReadOnlyRelatedAttribute} value=""{GetHiddenInitialValue(value)}"" />");

        result.Append("</div></div>");

        return result;
    }


    private string GetInitialValue(DateTime? dt)
    {
        return dt?.ToIso8601();
    }
    private string GetHiddenInitialValue(DateTime? dt)
    {
        return dt?.ToHtmlInputValue("miladi");
    }
}
