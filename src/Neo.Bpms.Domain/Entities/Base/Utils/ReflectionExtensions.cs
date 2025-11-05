using System.Text.RegularExpressions;

namespace Neo.Bpms.Domain.Entities.Base.Utils;

public static class ReflectionExtensions
{
    public static string GetDisplayableName(this MemberInfo memberInfo)
    {
        return MakeSpaceDelimited(memberInfo.Name);
    }
    public static string GetDisplayableName(this Type type)
    {
        return MakeSpaceDelimited(type.Name);
    }
    public static string GetDisplayableName(this string name)
    {
        return MakeSpaceDelimited(name);
    }

    private static string MakeSpaceDelimited(string initialName)
    {
        return initialName == null
            ? throw new ArgumentNullException(nameof(initialName))
            : Regex.Replace(initialName, "([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))",
                "$1 ")
            .Replace('_', ' ');
    }
}