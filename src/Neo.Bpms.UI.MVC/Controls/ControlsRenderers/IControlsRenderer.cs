using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;
using Microsoft.AspNetCore.Html;

namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public interface IControlsRenderer 
{
    HtmlString Render(ControlsRendererData data, FormField.Type? formFieldType = null);
    string CreateHtmlField(ControlsRendererData data, InputFieldDefinition field);
    string CreateControls(ControlsRendererData data, string parentControlId, FormField.Type? formFieldType = null);
    CandoStringBuilder CreateControl(ControlsRendererData data, string parentControlId, InputFieldDefinition fieldInfo);
    HtmlString RenderScripts(ControlsRendererData data);
}
