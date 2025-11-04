namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class ReportControl(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : BaseCandoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override CandoStringBuilder Render()
    {
        CandoStringBuilder result = new();

        result +=
            $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}"" {CommonProperties.GetStyle()} 
                    class=""{ControlsRendererData.ControlsClassString} {CommonProperties.WideColumnClasses} p-0 m-0 {CommonProperties.ShowHideRelatedClass} d-flex flex-column"">";
        RenderDesignIcons(result);

        result += $@"<iframe data-refresh-time=""{Field.PropertyValue(eControlPropertyId.RefreshEveryXMilliseconds)}""
                        class=""report-iframe implicit-iframe bg-white flex-grow-1"" src=""{IframeReportUrl}""></iframe>
            </div>";

        return result;
    }

    private string IframeReportUrl => Url.Action("IframeReport", "Report", ControlsRendererData.Url, UrlObject);

    protected override CandoStringBuilder RenderDesignIcons(CandoStringBuilder result)
    {
        base.RenderDesignIcons(result);
        if (ControlsRendererData.Options.IsDesignMode)
        {
            result += $@"<a class=""text-decoration-none"" href=""{Url.Action("Index", "Report", ControlsRendererData.Url, UrlObject)}"">
					<i class=""fa fa-link""></i>
					</a>";
        }
        return result;
    }

    private object UrlObject =>
        new
        {
            Field.NamespaceId,
            Field.EntityId,
            reportId = Field.FormId,
            configId = Field.PropertyValue(eControlPropertyId.ReportConfigurationId)
        };
}
