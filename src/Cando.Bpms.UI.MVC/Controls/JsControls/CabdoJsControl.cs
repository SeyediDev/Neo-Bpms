using Neo.Bpms.UI.MVC.Controls.JsControls.BindingModels;

namespace Neo.Bpms.UI.MVC.Controls.JsControls;

public abstract class CabdoJsControl(
    InputFieldDefinition field, ControlsRendererData controlsRenderer) : BaseCandoControl(field, controlsRenderer)
{
    public abstract CandoStringBuilder Render();

    protected CandoStringBuilder RegisterControl(CandoStringBuilder stringBuilder,
        JsBindingControl control, string fieldName)
    {
        string bindingControlJsObject = JsonConvert.SerializeObject(control);
        stringBuilder.Append($"window.ControlBindingsManager.addJsControl({bindingControlJsObject});");
        return stringBuilder;
    }

    protected CandoStringBuilder JsError(string error)
    {
        CandoStringBuilder result = new();
        result.Append("console.error('" + error + "');");
        return result;
    }
}
