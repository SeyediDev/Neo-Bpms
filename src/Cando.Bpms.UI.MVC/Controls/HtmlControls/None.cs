namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class None(IFormLogicHelper formLogicHelper, InputFieldDefinition field, ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer) : BaseCandoHtmlControl(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    public override CandoStringBuilder Render()
    {
        return ControlsRendererData.Options.IsInToolBox ? BlackBox() : new CandoStringBuilder();
    }

}
