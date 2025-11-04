namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

public static class QueryUtility<T> where T : new()
{
    public static QueryUtility New()
    {
        return QueryUtility.New<T>();
    }

    public static QueryUtility SelectFields(params string[] fieldList)
    {
        return New().SelectFields(fieldList);
    }

    public static QueryUtility Where(string filter, string orGroupId = null)
    {
        return New().Where(filter, orGroupId);
    }

    public static QueryUtility Wheres(params string[] filters)
    {
        var q = New();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            q.Where(filter);
        return q;
    }

    public static T FirstOrDefault(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
    {
        return QueryUtility.FirstOrDefault(filter, filterValues, func, param);
    }

    public static T FirstOrDefault(LocalParameters filterValues, params string[] filters)
    {
        return QueryUtility.FirstOrDefault<T>(filterValues, filters);
    }
    public static T First(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
    {
        return QueryUtility.First(filter, filterValues, func, param);
    }

    public static T First(LocalParameters filterValues, params string[] filters)
    {
        return QueryUtility.First<T>(filterValues, filters);
    }

    public static T Load(long id)
    {
        return QueryUtility.Find<T>(id);
    }
    public static TViewModel LoadViewModel<TViewModel>(long id)
        where TViewModel : new()
    {
        return New().LoadViewModel<TViewModel>(id);
    }

    public static List<T> ToList(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
    {
        return QueryUtility.ToList(filter, filterValues, func, param);
    }

    public static void ForEach(string filter, LocalParameters filterValues, Action<T> action)
    {
        QueryUtility.ForEach(filter, filterValues, action);
    }
    public static List<T> ToList(LocalParameters filterValues, params string[] filters)
    {
        return QueryUtility.ToList<T>(filterValues, filters);
    }
    public static List<TDb> ToList<TDb>(string filter, LocalParameters filterValues = null,
        Func<TDb, object, bool> func = null, object param = null)
        where TDb : new()
    {
        return QueryUtility.ToList<T, TDb>(filter, filterValues, func, param);
    }
    public static List<TDb> ToList<TDb>(LocalParameters filterValues, params string[] filters)
        where TDb : new()
    {
        return QueryUtility.ToList<T, TDb>(filterValues, filters);
    }
    public static List<TSelect> Select<TSelect>(string filter, LocalParameters filterValues, Func<T, TSelect> selector)
    {
        return Where(filter).Select(filterValues, selector);
    }
    public static bool Any(string filter, LocalParameters filterValues, Func<T, bool> predicate)
    {
        return Where(filter).Any(filterValues, predicate);
    }
    public static bool All(string filter, LocalParameters filterValues, Func<T, bool> predicate)
    {
        return Where(filter).All(filterValues, predicate);
    }

    public static Dictionary<long, T> ToDictionary(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
    {
        return QueryUtility.ToDictionary(filter, filterValues, func, param);
    }

    public static Dictionary<long, T> ToDictionary(LocalParameters filterValues, params string[] filters)
    {
        return QueryUtility.ToDictionary<T>(filterValues, filters);
    }

    public static Dictionary<long, TDicClass> ToDictionaryClassById<TDicClass>(string filter, string dbRecordFieldName,
        LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where TDicClass : new()
    {
        return QueryUtility.ToDictionaryClassById<TDicClass, T>(filter, dbRecordFieldName, filterValues, func, param);
    }
    public static Dictionary<long, List<T>> ToDictionaryOfList(string filter, string keyFieldId,
        LocalParameters filterValues = null)
    {
        return QueryUtility.ToDictionaryOfList<T>(filter, keyFieldId, filterValues);
    }
    public static Dictionary<long, Dictionary<string, T>> ToDictionaryOfDictionary(string filter, string keyFieldId1, string keyFieldId2,
        LocalParameters filterValues = null)
    {
        return QueryUtility.ToDictionaryOfDictionary<T>(filter, keyFieldId1, keyFieldId2, filterValues);
    }
    public static Dictionary<string, T> ToDictionaryByUnique(string filter, string keyFieldId,
        LocalParameters filterValues = null)
    {
        return Where(filter).ToDictionaryByUnique<T>(keyFieldId, filterValues);
    }
}
