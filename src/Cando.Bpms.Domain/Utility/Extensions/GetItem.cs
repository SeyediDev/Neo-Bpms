namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class DictionaryExtensions
{
    public static T GetItemIfKeyIsPositive<T>(this IDictionary<long, T> dictionary, long? key)
    {
        return GetItemIfKeyIsPositive(dictionary, key ?? 0);
    }

    public static T GetItemIfKeyIsPositive<T>(this IDictionary<long, T> dictionary, long key)
    {
        return key <= 0 ? default : GetItem(dictionary, key);
    }

    public static T GetItem<T>(this IDictionary<string, T> dictionary, string key)
    {
        return GetItemFromDic(dictionary, key, () => "");
    }

    public static T GetItem<T>(this IDictionary<long?, T> dictionary, long? key)
    {
        return GetItemFromDic(dictionary, key, () => 0);
    }

    public static T GetItem<T>(this IDictionary<long, T> dictionary, long? key)
    {
        return GetItemFromDic(dictionary, key ?? 0, () => 0);
    }

    public static T GetItem<T>(this IDictionary<long, T> dictionary, long key)
    {
        return GetItemFromDic(dictionary, key, () => 0);
    }

    public static T GetItemWithKey<T, TKey>(this IDictionary<TKey, T> dictionary, TKey key, Func<TKey> nullValueReplacement)
    {
        return GetItemFromDic(dictionary, key, nullValueReplacement);
    }

    private static T GetItemFromDic<T, TKey>(this IDictionary<TKey, T> dictionary, TKey key, Func<TKey> nullValueReplacement)
    {
        if (dictionary == null)
        {
            return default;
        }

        _ = TryGetValue(dictionary, ref key, nullValueReplacement, out T? item);
        return item;
    }
}
