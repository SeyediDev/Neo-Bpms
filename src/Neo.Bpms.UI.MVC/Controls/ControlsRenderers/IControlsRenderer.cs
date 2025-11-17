namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public interface IControlsRenderer 
{
    HtmlString Render(ControlsRendererData data, FormField.Type? formFieldType = null);
    string CreateHtmlField(ControlsRendererData data, InputFieldDefinition field);
    string CreateControls(ControlsRendererData data, string parentControlId, FormField.Type? formFieldType = null);
    NeoStringBuilder CreateControl(ControlsRendererData data, string parentControlId, InputFieldDefinition fieldInfo);
    HtmlString RenderScripts(ControlsRendererData data);
}
