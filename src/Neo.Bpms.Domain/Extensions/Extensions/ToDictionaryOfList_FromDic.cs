namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class DictionaryExtensions
{
    public static Dictionary<string, List<T>> ToDictionaryOfList<T>(this IDictionary<string, T> dic,
        Func<T, string> keySelector)
    {
        Dictionary<string, List<T>> newDic = [];
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItem(keySelector(value)).Add(value);
        }

        return newDic;
    }

    public static Dictionary<long, List<T>> ToDictionaryOfList<T>(this IDictionary<long, T> dic,
        Func<T, long> keySelector)
    {
        Dictionary<long, List<T>> newDic = [];
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItem(keySelector(value)).Add(value);
        }

        return newDic;
    }

    public static Dictionary<long?, List<T>> ToDictionaryOfList<T>(this IDictionary<long?, T> dic,
        Func<T, long?> keySelector)
    {
        Dictionary<long?, List<T>> newDic = [];
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItem(keySelector(value)).Add(value);
        }

        return newDic;
    }

    public static Dictionary<TKey, List<T>> ToDictionaryOfListWithKey<T, TKey>(this IDictionary<TKey, T> dic,
        Func<T, TKey> keySelector, Func<TKey> nullValueReplacement)
    {
        Dictionary<TKey, List<T>> newDic = [];
        foreach (T value in dic.Values)
        {
            newDic.AddOrGetItemWithKey(keySelector(value), nullValueReplacement).Add(value);
        }

        return newDic;
    }
}
