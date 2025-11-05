namespace Neo.Bpms.Domain.Extensions;

public static class StringUtils
{
    public static string NormalizeFarsi(this string s)
    {
        return string.IsNullOrEmpty(s) ? s : s.Replace('ي', 'ی').Replace('ك', 'ک').Trim();
    }
}