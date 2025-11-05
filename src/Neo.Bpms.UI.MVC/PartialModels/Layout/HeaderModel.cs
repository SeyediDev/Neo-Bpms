using Microsoft.AspNetCore.Mvc.Razor;

namespace Neo.Bpms.UI.MVC.PartialModels.Layout;

public class HeaderModel
{
    public IdentityUser user { get; set; }
    public Func<dynamic, HelperResult> logo { get; set; }
    public string title { get; set; }
    public string titleImage { get; set; }
    public bool hasLanguageSelection { get; set; }
}