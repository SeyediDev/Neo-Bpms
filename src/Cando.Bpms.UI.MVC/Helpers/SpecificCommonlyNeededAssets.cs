namespace Neo.Bpms.UI.MVC.Helpers;

public static class SpecificCommonlyNeededAssets
{
    private static readonly Dictionary<string, string> CommonSpecificResources = [];

    public static void SetNeededResources(string loginImageRoute, string reportHeaderLogoRoute)
    {
        CommonSpecificResources.Add("login-logo", loginImageRoute);
        CommonSpecificResources.Add("report-header-logo", reportHeaderLogoRoute);
    }

    public static string GetImage(string imageName)
    {
        return CommonSpecificResources.TryGetValue(imageName, out string value) ? value : $"~/Content/images/{imageName}.png";
    }
    
    public static string GetImageSvg(string imageName)
    {
        return CommonSpecificResources.TryGetValue(imageName, out string value) ? value : $"~/Content/images/{imageName}.svg";
    }
}
