using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Microsoft.Extensions.Configuration;

namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public partial class ControlsRenderer(ICmmnDocument cmmnFileManager,
    IFormLogicHelper formLogicHelper, FormStructRoutines formStructRoutines,
    ILogger<ControlsRenderer> logger, IConfiguration configuration, ISBVRRenderer sbvrRenderer): IControlsRenderer
{
    private readonly ISBVRRenderer _sbvrRenderer = sbvrRenderer;
    /// <summary>
    /// create Html DOM Elements upon request from structure that has been passed to this method.
    /// </summary>
    /// <returns></returns>
    public HtmlString Render(ControlsRendererData data, FormField.Type? formFieldType = null)
    {
        return new HtmlString(CreateControlsWithLayout(data, formFieldType));
    }

    /// <summary>
    /// Create html of one field
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public string CreateHtmlField(ControlsRendererData data, InputFieldDefinition field)
    {
        return CreateControl(data, null, field).ToString();
    }

    /// <summary>
    /// create and apply properties to HTML DOM Elements
    /// </summary>
    /// <param name="formFieldType"></param>
    /// <returns></returns>
    private string CreateControlsWithLayout(ControlsRendererData data, FormField.Type? formFieldType = null)
    {
        string str = "";
        if (data.Structure.HasTemplate && !data.Options.IsDesignMode)
        {
            str += RenderTemplate(data);
        }
        else
        {
            str = $@"<div id=""main-part"" class=""form-element sortable-container row m-0"">{CreateControls(data, "", formFieldType)}</div>";
        }

        return str;
    }
}
