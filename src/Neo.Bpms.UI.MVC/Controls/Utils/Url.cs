namespace Neo.Bpms.UI.MVC.Controls.Utils;

public static class Url
{
    public static string Action(string action, string controller, IUrlHelper urlHelper, object urlObject)
    {
        return urlHelper.Action(action, controller,
        urlObject);
    }
    public static string Action(string action, string controller, IUrlHelper urlHelper)
    {
        return urlHelper.Action(action, controller);
    }
}