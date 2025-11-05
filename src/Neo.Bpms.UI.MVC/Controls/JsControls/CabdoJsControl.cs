using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.UI.MVC.Controls.JsControls.BindingModels;

namespace Neo.Bpms.UI.MVC.Controls.JsControls;

public abstract class CabdoJsControl(
    InputFieldDefinition field, ControlsRendererData controlsRenderer) : BaseNeoControl(field, controlsRenderer)
{
    public abstract NeoStringBuilder Render();

    protected NeoStringBuilder RegisterControl(NeoStringBuilder stringBuilder,
        JsBindingControl control, string fieldName)
    {
        string bindingControlJsObject = JsonConvert.SerializeObject(control);
        stringBuilder.Append($"window.ControlBindingsManager.addJsControl({bindingControlJsObject});");
        return stringBuilder;
    }

    protected NeoStringBuilder JsError(string error)
    {
        NeoStringBuilder result = new();
        result.Append("console.error('" + error + "');");
        return result;
    }
}
