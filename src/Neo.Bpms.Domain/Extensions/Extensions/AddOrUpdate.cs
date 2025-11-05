namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class DictionaryExtensions
{
    public static bool AddOrUpdateItem<T>(this IDictionary<string, T> dic, string key, T item)
        where T : new()
    {
        return dic.AddOrUpdateDic(key, item, () => "");
    }

    public static bool AddOrUpdateItem<T>(this IDictionary<long, T> dic, long key, T item)
        where T : new()
    {
        return dic.AddOrUpdateDic(key, item, () => 0);
    }

    public static bool AddOrUpdateItem<T>(this IDictionary<long?, T> dic, long? key, T item)
        where T : new()
    {
        return dic.AddOrUpdateDic(key, item, () => 0);
    }

    public static bool AddOrUpdateWithKey<T, TKey>(this IDictionary<TKey, T> dic, TKey key, T item, Func<TKey> nullValueReplacement)
        where T : new()
    {
        return dic.AddOrUpdateDic(key, item, nullValueReplacement);
    }

    private static bool AddOrUpdateDic<T, TKey>(this IDictionary<TKey, T> dic, TKey key, T item, Func<TKey> nullValueReplacement)
        where T : new()
    {
        key ??= nullValueReplacement();
        if (!dic.ContainsKey(key))
        {
            dic.Add(key, item);
            return true;
        }

        dic[key] = item;
        return false;
    }
}
