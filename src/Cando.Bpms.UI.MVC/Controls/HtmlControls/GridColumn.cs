namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class GridColumn(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, ControlsRenderer controlsRenderer, ISBVRRenderer sbvrRenderer) 
    : BaseCandoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override CandoStringBuilder Render()
    {
        CandoStringBuilder result = new();

        result += $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}""
                        class=""{ControlsRendererData.ControlsClassString} {(ControlsRendererData.Options.IsDesignMode ? "border" : "")} {CommonProperties.NarrowColumnClasses} {CommonProperties.ShowHideRelatedClass}"">";
        RenderDesignIcons(result);
        result += $@"<div class=""row"">";

        result += controlsRenderer.CreateControls(ControlsRendererData, Field.FieldName);

        result += "</div></div>";
        return result;
    }
}
