namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class ConcurrentDictionaryExtensions
{
    public static ConcurrentDictionary<string, ConcurrentDictionary<TKey, T>> ToConcurrentDictionaryOfList<TKey, T>(this IList<T> items,
        Func<T, string> keySelector)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<string, ConcurrentDictionary<TKey, T>> newDic = new();
        foreach (T value in items)
        {
            newDic.AddOrGetItem(keySelector(value)).TryAdd(value.Id, value);
        }

        return newDic;
    }

    public static ConcurrentDictionary<long, ConcurrentDictionary<TKey, T>> ToConcurrentDictionaryOfList<TKey, T>(this IList<T> items,
        Func<T, long> keySelector)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<long, ConcurrentDictionary<TKey, T>> newDic = new();
        foreach (T value in items)
        {
            newDic.AddOrGetItem(keySelector(value)).TryAdd(value.Id, value);
        }

        return newDic;
    }
    public static ConcurrentDictionary<long?, ConcurrentDictionary<TKey, T>> ToConcurrentDictionaryOfList<TKey, T>(this IList<T> items,
        Func<T, long?> keySelector)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<long?, ConcurrentDictionary<TKey, T>> newDic = new();
        foreach (T value in items)
        {
            newDic.AddOrGetItem(keySelector(value)).TryAdd(value.Id, value);
        }

        return newDic;
    }

    public static ConcurrentDictionary<TKey, ConcurrentDictionary<TKey, T>> ToConcurrentDictionaryOfListWithKey<TKey, T>(this IList<T> items,
        Func<T, TKey> keySelector, Func<TKey> nullValueReplacement)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<TKey, ConcurrentDictionary<TKey, T>> newDic = new();
        foreach (T value in items)
        {
            newDic.AddOrGetItemWithKey(keySelector(value), nullValueReplacement).TryAdd(value.Id, value);
        }

        return newDic;
    }
}
