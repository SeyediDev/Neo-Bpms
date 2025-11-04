namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class DictionaryExtensions
{
    public static T AddOrGetItem<T>(this IDictionary<string, T> dic, string key)
        where T : new()
    {
        return AddOrGetItemFromDic(dic, key, () => "");
    }

    public static T AddOrGetItem<T>(this IDictionary<long, T> dic, long key)
        where T : new()
    {
        return AddOrGetItemFromDic(dic, key, () => 0);
    }

    public static T AddOrGetItem<T>(this IDictionary<long?, T> dic, long? key)
        where T : new()
    {
        return AddOrGetItemFromDic(dic, key, () => 0);
    }

    public static T AddOrGetItemWithKey<T, TKey>(this IDictionary<TKey, T> dic, TKey key, Func<TKey> nullValueReplacement)
        where T : new()
    {
        return AddOrGetItemFromDic(dic, key, nullValueReplacement);
    }

    private static T AddOrGetItemFromDic<T, TKey>(this IDictionary<TKey, T> dic, TKey key, Func<TKey> nullValueReplacement)
        where T : new()
    {
        if (TryGetValue(dic, ref key, nullValueReplacement, out T? item))
        {
            return item;
        }

        item = new T();
        if (dic is ConcurrentDictionary<TKey, T> cDic)
        {
            _ = cDic.TryAdd(key, item);
            return cDic[key];
        }

        dic.Add(key, item);
        return item;
    }
    public static void Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, TValue item)
        where TValue : IBaseClassId<TKey>
    {
        dic.TryAdd(item.Id, item);
    }

}
