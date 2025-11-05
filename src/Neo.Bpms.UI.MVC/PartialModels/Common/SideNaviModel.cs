using Microsoft.AspNetCore.Mvc.Razor;

namespace Neo.Bpms.UI.MVC.PartialModels.Common;

public class SideNaviModel
{
    public List<string> titles { get; set; }
    public List<HelperResult> tabContents { get; set; }
}