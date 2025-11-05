using Microsoft.AspNetCore.Html;
using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.UI.MVC.Controls.JsControls;
using Neo.Bpms.UI.MVC.Controls.JsControls.AdvancedUpload;

namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public partial class ControlsRenderer
{
    public virtual HtmlString RenderScripts(ControlsRendererData data)
    {
        NeoStringBuilder stringBuilder = new();
        foreach (InputFieldDefinition fieldDefinition in data.Structure.Fields)
        {
            switch (fieldDefinition.ControlType)
            {
                case eControlTypeId.AdvancedUpload:
                    stringBuilder += new AdvancedUpload(fieldDefinition, data).Render();
                    break;
                case eControlTypeId.Terminal:
                    stringBuilder += new Terminal(fieldDefinition, data).Render();
                    break;
                case eControlTypeId.BusinessDefinedControl:
                    stringBuilder +=
                        BusinessDefinedControls[fieldDefinition.PropertyValue(eControlPropertyId.BusinessControlId)].RenderScript(fieldDefinition, this);
                    break;
            }
        }

        return new HtmlString(stringBuilder.ToString());
    }
}
