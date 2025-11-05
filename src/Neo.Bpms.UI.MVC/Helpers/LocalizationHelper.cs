namespace Neo.Bpms.UI.MVC.Helpers;

public static class LocalizationExtensions
{
    public static string Parameters(this string resourceString, params object[] parameters)
    {
        return string.Format(resourceString, parameters);
    }
}