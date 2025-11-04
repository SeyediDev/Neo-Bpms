namespace Neo.Bpms.Domain.Utility.Extensions;

public static partial class ListExtensions
{
    public static Dictionary<string, List<T>> ToDictionaryOfList<T>(this IList<T> items,
        Func<T, string> keySelector)
    {
        Dictionary<string, List<T>> newDic = [];
        foreach (T value in items)
        {
            newDic.AddOrGetItem(keySelector(value)).Add(value);
        }

        return newDic;
    }

    public static Dictionary<long, List<T>> ToDictionaryOfList<T>(this IList<T> items,
        Func<T, long> keySelector)
    {
        Dictionary<long, List<T>> newDic = [];
        foreach (T value in items)
        {
            newDic.AddOrGetItem(keySelector(value)).Add(value);
        }

        return newDic;
    }
    public static Dictionary<long?, List<T>> ToDictionaryOfList<T>(this IList<T> items,
        Func<T, long?> keySelector)
    {
        Dictionary<long?, List<T>> newDic = [];
        foreach (T value in items)
        {
            newDic.AddOrGetItem(keySelector(value)).Add(value);
        }

        return newDic;
    }

    public static Dictionary<TKey, List<T>> ToDictionaryOfListWithKey<T, TKey>(this IList<T> items,
        Func<T, TKey> keySelector, Func<TKey> nullValueReplacement)
    {
        Dictionary<TKey, List<T>> newDic = [];
        foreach (T value in items)
        {
            newDic.AddOrGetItemWithKey(keySelector(value), nullValueReplacement).Add(value);
        }

        return newDic;
    }
}
