namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class ConcurrentDictionaryExtensions
{
    public static ConcurrentDictionary<string, ConcurrentDictionary<T, TKey>> ToConcurrentDictionaryOfList<T, TKey>(
        this IDictionary<string, T> dic, Func<T, string> keySelector)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<string, ConcurrentDictionary<T, TKey>> newDic = new();
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItem(keySelector(value)).TryAdd(value, value.Id);
        }

        return newDic;
    }

    public static ConcurrentDictionary<long, ConcurrentDictionary<T, TKey>> ToConcurrentDictionaryOfList<T, TKey>(
        this IDictionary<long, T> dic, Func<T, long> keySelector)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<long, ConcurrentDictionary<T, TKey>> newDic = new();
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItem(keySelector(value)).TryAdd(value, value.Id);
        }

        return newDic;
    }

    public static ConcurrentDictionary<long?, ConcurrentDictionary<T, TKey>> ToConcurrentDictionaryOfList<T, TKey>(
        this IDictionary<long?, T> dic, Func<T, long?> keySelector)
        where T : IBaseClassId<TKey>
    {
        ConcurrentDictionary<long?, ConcurrentDictionary<T, TKey>> newDic = new();
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItem(keySelector(value)).TryAdd(value, value.Id);
        }

        return newDic;
    }

    public static ConcurrentDictionary<TKey, List<T>> ToConcurrentDictionaryOfListWithKey<T, TKey>(
        this ConcurrentDictionary<TKey, T> dic,
        Func<T, TKey> keySelector, Func<TKey> nullValueReplacement)
    {
        ConcurrentDictionary<TKey, List<T>> newDic = new();
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItemWithKey(keySelector(value), nullValueReplacement).Add(value);
        }

        return newDic;
    }
}
