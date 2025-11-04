using Microsoft.AspNetCore.Html;

namespace Neo.Bpms.UI.MVC.PartialModels.Layout;

public class BodyModel
{
    public string containerClass { get; set; }
    public Func<IHtmlContent> bodyRenderFunc { get; set; }
    public string PaddingBottom { get; set; }
}