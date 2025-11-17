using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class FormIframe(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer) 
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        NeoStringBuilder result = new();

        result +=
            $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}"" {CommonProperties.GetStyle()} 
                    class=""{ControlsRendererData.ControlsClassString} {CommonProperties.WideColumnClasses} p-0 m-0 {CommonProperties.ShowHideRelatedClass} d-flex flex-column"">";
        RenderDesignIcons(result);

        result += $@"<iframe class=""implicit-iframe bg-white flex-grow-1"" src=""{IframeFormUrl}""></iframe>
            </div>";

        return result;
    }

    private string IframeFormUrl => Url.Action("IframeForm", "Form", ControlsRendererData.Url, UrlObject);

    protected override NeoStringBuilder RenderDesignIcons(NeoStringBuilder result)
    {
        base.RenderDesignIcons(result);

        if (ControlsRendererData.Options.IsDesignMode)
        {
            string href;
            if (!string.IsNullOrEmpty(UrlObject.ProcessId))
            {
                href = Url.Action("Form", "Process", ControlsRendererData.Url, new
                {
                    taskId = UrlObject.TaskId,
                    __processId = UrlObject.ProcessId
                });
            }
            else
            {
                href = Url.Action("Redirect", "Form", UrlObject);
            }
            result += $@"<a class=""text-decoration-none"" href=""{href}"">
					<i class=""fa fa-link""></i>
					</a>";
        }
        return result;
    }

    private dynamic UrlObject =>
        new
        {
            Field.NamespaceId,
            Field.EntityId,
            Field.FormId,
            ProcessId = Field.PropertyValue(eControlPropertyId.ProcessId),
            TaskId = Field.PropertyValue(eControlPropertyId.TaskId)
        };
}
