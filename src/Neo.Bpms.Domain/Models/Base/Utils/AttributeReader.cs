namespace Neo.Bpms.Domain.Models.Base.Utils;

public static class AttributeReader
{
    public static string GetAttributeString<T>(Type type)
    {
        return GetAttribute<T>(type)?.ToString();
    }
    public static T GetAttribute<T>(Type type)
    {
        object[] attributes = type.GetCustomAttributes(typeof(T), false);
        return attributes.Length == 0 ? default : (T)attributes[0];
    }
    public static List<T> GetAttributes<T>(Type type)
    {
        object[] arr = type.GetCustomAttributes(typeof(T), false);
        return [.. arr.Select(at => (T)at)];
    }
}