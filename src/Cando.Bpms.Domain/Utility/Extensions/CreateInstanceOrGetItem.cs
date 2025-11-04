namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class DictionaryExtensions
{
    public static T CreateInstanceOrGetItem<T>(this IDictionary<string, T> dic, string key,
        Func<object, T> createInstance, object createInstanceParam = null)
    {
        return CreateInstanceOrGetItemFromDic(dic, key, () => "", createInstance, createInstanceParam);
    }
    public static T CreateInstanceOrGetItemWithKey<T, TKey>(this IDictionary<TKey, T> dic,
        TKey key, Func<TKey> nullValueReplacement,
        Func<object, T> createInstance, object createInstanceParam = null)
    {
        return CreateInstanceOrGetItemFromDic(dic, key, nullValueReplacement, createInstance, createInstanceParam);
    }

    private static T CreateInstanceOrGetItemFromDic<TKey, T>(this IDictionary<TKey, T> dic,
        TKey key, Func<TKey> nullValueReplacement,
        Func<object, T> createInstance, object createInstanceParam = null)
    {
        if (TryGetValue(dic, ref key, nullValueReplacement, out T? item))
        {
            return item;
        }

        item = createInstance(createInstanceParam);
        dic.Add(key, item);
        return item;
    }
    public static bool TryGetValue<TKey, T>(IDictionary<TKey, T> dic, ref TKey key, Func<TKey> nullValueReplacement,
        out T? item)
    {
        key ??= nullValueReplacement();
        return dic.TryGetValue(key, out item);
    }
}
