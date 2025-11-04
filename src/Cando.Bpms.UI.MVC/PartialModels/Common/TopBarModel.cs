using Microsoft.AspNetCore.Mvc.Razor;

namespace Neo.Bpms.UI.MVC.PartialModels.Common;

public class TopBarModel
{
    public string Title { get; set; }
    public string PageIcon { get; set; }
    public HelperResult Tools { get; set; }
    public string FilteredBy { get; set; }
    public string IconsContainerClass { get; set; }
}