namespace Neo.Bpms.UI.MVC.Helpers;

public static class NeoUrlHelper
{
    public static string GetBaseUrl(this HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}{request.PathBase}";
    }

}
