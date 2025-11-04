namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class CandoImage(IFormLogicHelper formLogicHelper, InputFieldDefinition field, ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer) 
    : BaseCandoHtmlControl(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    public override CandoStringBuilder Render()
    {
        CandoStringBuilder result = new();

        result += $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}""
                        class=""{ControlsRendererData.ControlsClassString} {CommonProperties.ShowHideRelatedClass} m-0"">";
        RenderDesignIcons(result);
        result += $@"<img style=""width: 100%;"" src=""{ControlsRendererData.ModelAssetsRoot}/{Field.PropertyValue(eControlPropertyId.SourceFileAddress)}"" 
                              alt=""{Field.Alias}"" title=""{Field.Alias}"">";

        result += "</div>";
        return result;
    }
}
