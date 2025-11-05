using Neo.Bpms.Domain.Extensions;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class None(IFormLogicHelper formLogicHelper, InputFieldDefinition field, ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer) : BaseNeoHtmlControl(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        return ControlsRendererData.Options.IsInToolBox ? BlackBox() : new NeoStringBuilder();
    }

}
