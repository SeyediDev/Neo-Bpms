using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility : EntityConnection
{
    #region query execution
    public static T FirstOrDefault<T>(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.FirstOrDefault(filterValues, func, param);
    }

    public static T FirstOrDefault<T>(LocalParameters filterValues, params string[] filters)
        where T : new()
    {
        var q = New<T>();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            q.Where(filter);
        return q.FirstOrDefault<T>(filterValues);
    }

    public static T First<T>(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.FirstOrDefault(filterValues, func, param);
    }

    public static T First<T>(LocalParameters filterValues, params string[] filters)
        where T : new()
    {
        var q = New<T>();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            q.Where(filter);
        return q.FirstOrDefault<T>(filterValues);
    }

    public static List<T> ToList<T>(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.ToList(filterValues, func, param);
    }

    public static List<TDb> ToList<T, TDb>(string filter, LocalParameters filterValues = null,
        Func<TDb, object, bool> func = null, object param = null)
        where T : new()
        where TDb : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.ToList(filterValues, func, param);
    }

    public static void ForEach<T>(string filter, LocalParameters filterValues, Action<T> action)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        q.ForEach(filterValues, action);
    }

    public static void ParallelForEach<T>(string filter, LocalParameters filterValues, Action<T> action)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        q.ParallelForEach(filterValues, action);
    }

    public static List<T> ToList<T>(LocalParameters filterValues, params string[] filters)
        where T : new()
    {
        var q = New<T>();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            q.Where(filter);
        return q.ToList<T>(filterValues);
    }

    public static List<TDb> ToList<T, TDb>(LocalParameters filterValues, params string[] filters)
        where T : new()
        where TDb : new()
    {
        var q = New<T>();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            q.Where(filter);
        return q.ToList<TDb>(filterValues);
    }

    public static Dictionary<long, T> ToDictionary<T>(string filter, LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.ToDictionary<long, T>(filterValues, func, param);
    }

    public static Dictionary<long, T> ToDictionary<T>(LocalParameters filterValues, params string[] filters)
        where T : new()
    {
        var q = New<T>();
        foreach (var filter in filters ?? Enumerable.Empty<string>())
            q.Where(filter);
        return q.ToDictionary<long, T>(filterValues);
    }

    public static Dictionary<long, TDicClass> ToDictionaryClassById<TDicClass, T>(string filter,
        string dbRecordFieldName,
        LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new() where TDicClass : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.ToDictionaryClassById<TDicClass, T>(dbRecordFieldName, filterValues, func, param);
    }

    public static Dictionary<long, List<T>> ToDictionaryOfList<T>(string filter, string keyFieldId,
        LocalParameters filterValues = null)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.ToDictionaryOfList<T>(keyFieldId, filterValues);
    }

    public static Dictionary<long, Dictionary<string, T>> ToDictionaryOfDictionary<T>(string filter,
        string keyFieldId1, string keyFieldId2,
        LocalParameters filterValues = null)
        where T : new()
    {
        var q = New<T>();
        if (!string.IsNullOrEmpty(filter))
            q.Where(filter);
        return q.ToDictionaryOfDictionary<T>(keyFieldId1, keyFieldId2, filterValues);
    }

    public static QueryUtility New<T>(string name = null)
        where T : new()
    {
        return new QueryUtility(ProjectDefinition.Project.GetEntity<T>(), name ?? "QU.New.1");
    }

    public static QueryUtility New(string namespaceId, string entityId, string name = null)
    {
        return new QueryUtility(namespaceId, entityId, name ?? "QU.New.2");
    }

    public static QueryUtility New(Entity entity)
    {
        return new QueryUtility(entity, "QU.New.3");
    }

    public T FirstOrDefault<T>(LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        SetPage(1, 1);
        return ToList(filterValues, func, param).FirstOrDefault();
    }

    public T First<T>(LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        SetPage(1, 1);
        return ToList(filterValues, func, param).FirstOrDefault();
    }

    public List<T> ToList<T>(LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        var records = new List<T>();
        if (DataSource != null)
        {
            DataSource.toList(ref records, func, param);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public void ForEach<T>(LocalParameters filterValues, Action<T> action)
        where T : new()
    {
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.ForEach(action);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
    }

    public void ParallelForEach<T>(LocalParameters filterValues, Action<T> action)
        where T : new()
    {
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.ParallelForEach(action);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
    }

    public Dictionary<TKey, T> ToDictionary<TKey, T>(LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var records = new Dictionary<TKey, T>();
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.toDictionary(ref records, func, param);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public ConcurrentDictionary<TKey, T> ToConcurrentDictionary<TKey, T>(LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var records = new ConcurrentDictionary<TKey, T>();
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.ToConcurrentDictionary(ref records, func, param);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<long, T> ToDictionaryById<T>(LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        var records = new Dictionary<long, T>();
        SelectFieldsOfEntityIfNotSelected<T>();
        AddPkFields();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.ToDictionaryById(ref records, func, param);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<long, ElasticObject> ToDictionaryById(LocalParameters filterValues = null,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        var records = new Dictionary<long, ElasticObject>();
        SelectFieldsOfEntityIfNotSelected();
        AddPkFields();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.ToDictionaryById(ref records, func, param);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<long, TDicClass> ToDictionaryClassById<TDicClass, T>(string dbRecordFieldName,
        LocalParameters filterValues = null,
        Func<T, object, bool> func = null, object param = null)
        where T : new() where TDicClass : new()
    {
        var records = new Dictionary<long, TDicClass>();
        SelectFieldsOfEntityIfNotSelected<T>();
        AddPkFields();
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            DataSource.ToDictionaryClassById(dbRecordFieldName, ref records, func, param);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<long, List<T>> ToDictionaryOfList<T>(string keyFieldId,
        LocalParameters filterValues = null)
        where T : new()
    {
        var records = new Dictionary<long, List<T>>();
        SelectFieldsOfEntityIfNotSelected<T>();
        AddPkFields();
        SelectFields(keyFieldId);
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            var keyField = Entity.GetField(keyFieldId);
            DataSource.ToDictionaryOfList(keyField?.DbFieldName ?? keyFieldId, ref records);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<string, List<T>> ToStringDictionaryOfList<T>(string keyFieldId,
        LocalParameters filterValues = null)
        where T : new()
    {
        var records = new Dictionary<string, List<T>>();
        SelectFieldsOfEntityIfNotSelected<T>();
        AddPkFields();
        SelectFields(keyFieldId);
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            var keyField = Entity.GetField(keyFieldId);
            DataSource.ToStringDictionaryOfList(keyField?.DbFieldName ?? keyFieldId, ref records);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<long, Dictionary<string, T>> ToDictionaryOfDictionary<T>(string keyFieldId1, string keyFieldId2,
        LocalParameters filterValues = null)
        where T : new()
    {
        var records = new Dictionary<long, Dictionary<string, T>>();
        SelectFieldsOfEntityIfNotSelected<T>();
        SelectFields("Id", keyFieldId1, keyFieldId2);
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            var keyField1 = Entity.GetField(keyFieldId1);
            var keyField2 = Entity.GetField(keyFieldId2);
            DataSource.ToDictionaryOfDictionary(keyField1?.DbFieldName ?? keyFieldId1,
                keyField2?.DbFieldName ?? keyFieldId2, ref records);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }
    public Dictionary<long, Dictionary<long, T>> ToLongDictionaryOfLongDictionary<T>(string keyFieldId1, string keyFieldId2,
        LocalParameters filterValues = null)
        where T : new()
    {
        var records = new Dictionary<long, Dictionary<long, T>>();
        SelectFieldsOfEntityIfNotSelected<T>();
        SelectFields("Id", keyFieldId1, keyFieldId2);
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            var keyField1 = Entity.GetField(keyFieldId1);
            var keyField2 = Entity.GetField(keyFieldId2);
            DataSource.ToLongDictionaryOfLongDictionary(keyField1?.DbFieldName ?? keyFieldId1,
                keyField2?.DbFieldName ?? keyFieldId2, ref records);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public Dictionary<string, T> ToDictionaryByUnique<T>(string keyFieldId,
        LocalParameters filterValues = null)
        where T : new()
    {
        var records = new Dictionary<string, T>();
        SelectFieldsOfEntityIfNotSelected<T>();
        SelectFields("Id", keyFieldId);
        PreProcessQuery(filterValues);
        if (DataSource != null)
        {
            var keyField = Entity.GetField(keyFieldId);
            DataSource.ToDictionaryByUnique(keyField?.DbFieldName ?? keyFieldId, ref records);
            CommandTxt = DataSource.SqlCommand;
        }

        ReleaseQuery();
        return records;
    }

    public ElasticObject FirstOrDefault(LocalParameters filterValues = null,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        SetPage(1, 1);
        return ToList(filterValues, func, param).FirstOrDefault();
    }

    public List<ElasticObject> ToList(LocalParameters filterValues = null,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        var records = new List<ElasticObject>();
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        DataSource.ToList(ref records, func, param);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return records;
    }
    public async Task<List<ElasticObject>> ToListAsync(LocalParameters filterValues = null,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        var (b, records) = await DataSource.ToListAsync(func, param);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return records;
    }

    public void ForEach(LocalParameters filterValues, Action<ElasticObject> action)
    {
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        DataSource.ForEach(action);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
    }

    public void ParallelForEach(LocalParameters filterValues, Action<ElasticObject> action)
    {
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        DataSource.ParallelForEach(action);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
    }

    public List<TSelect> Select<T, TSelect>(LocalParameters filterValues, Func<T, TSelect> selector)
        where T : new()
    {
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        var list = DataSource.Select(selector);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return list;
    }

    public bool Any<T>(LocalParameters filterValues, Func<T, bool> predicate = null)
        where T : new()
    {
        AddPkFields();
        PreProcessQuery(filterValues);
        var result = DataSource.Any(predicate);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return result;
    }

    public bool All<T>(LocalParameters filterValues, Func<T, bool> predicate)
        where T : new()
    {
        AddPkFields();
        PreProcessQuery(filterValues);
        var result = DataSource.All(predicate);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return result;
    }

    public List<TSelect> Select<TSelect>(LocalParameters filterValues, Func<ElasticObject, TSelect> selector)
        where TSelect : new()
    {
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        var list = DataSource.Select(selector);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return list;
    }

    public bool Any(LocalParameters filterValues = null, Func<ElasticObject, bool> predicate = null)
    {
        AddPkFields();
        PreProcessQuery(filterValues);
        var result = DataSource.Any(predicate);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return result;
    }

    public bool All(LocalParameters filterValues, Func<ElasticObject, bool> predicate)
    {
        AddPkFields();
        PreProcessQuery(filterValues);
        var result = DataSource.All(predicate);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return result;
    }

    public string GetSqlCommand(LocalParameters filterValues = null)
    {
        PreProcessQuery(filterValues);
        CommandTxt = DataSource.GenerateQuery();
        return CommandTxt;
    }

    public bool GetDocuments(LocalParameters filterValues = null)
    {
        PreProcessQuery(filterValues);
        var t0 = DateTime.UtcNow.Ticks;
        var r = DataSource.openForRead();
        CommandTxt = DataSource.SqlCommand;
        QueryTime = DateTime.UtcNow.Ticks - t0;
        if (!r)
            ReleaseQuery();
        return r;
    }

    public bool GetDocuments(ElasticObject filterValues, LocalParameters fvs = null)
    {
        fvs ??= [];
        if (!fvs.ContainsKey("q") || filterValues != null)
        {
            filterValues ??= new ElasticObject();
            fvs.AddOrUpdate("q", filterValues);
        }

        return GetDocuments(fvs);
    }

    public bool GetNextDocument<T>(out T curRecord) where T : new()
    {
        if (CancellationToken.IsCancellationRequested)
        {
            curRecord = default;
            ReleaseQuery();
            return false;
        }
        var b = DataSource.ReadNext(out curRecord);
        if (!b || curRecord == null)
            ReleaseQuery();
        return b;
    }

    public ElasticObject GetRecord()
    {
        if (CancellationToken.IsCancellationRequested)
        {
            ReleaseQuery();
            return null;
        }
        var b = DataSource.ReadNext(out var curRecord);
        if (!b || curRecord == null)
            ReleaseQuery();
        return !b ? null : curRecord;
    }

    public bool Test<T>(T record, bool checkAllExists)
    {
        return DataSource.Test(record, checkAllExists);
    }

    public bool Test(ElasticObject record)
    {
        return DataSource.Test(record);
    }

    public IEnumerable<ElasticObject> GetRecords()
    {
        do
        {
            if (CancellationToken.IsCancellationRequested)
                break;
            var record = GetRecord();
            if (record == null)
                break;
            yield return record;
        } while (true);
    }

    public IEnumerable<T> GetRecordsT<T>()
        where T : new()
    {
        do
        {
            if (CancellationToken.IsCancellationRequested)
                break;
            var record = GetRecordT<T>();
            if (record == null)
                break;
            yield return record;
        } while (true);
    }

    public T GetRecordT<T>() where T : new()
    {
        if (CancellationToken.IsCancellationRequested)
        {
            ReleaseQuery();
            return default;
        }

        var b = DataSource.ReadNext(out T curRecord);
        if (!b || curRecord == null)
            ReleaseQuery();
        return curRecord;
    }
    #endregion query execution

    #region find record
    public T Find<T>(ExpressionNode filter, LocalParameters filterValues = null)
        where T : new()
    {
        AddFilter(filter);
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        T newRecord;
        if (!DataSource.find(null, out newRecord))
            newRecord = default;
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return newRecord;
    }

    public ElasticObject Find(ExpressionNode filter, LocalParameters filterValues)
    {
        AddFilter(filter);
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        var output = DataSource.find(null);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return output;
    }

    public static T Find<T>(long id) where T : new()
    {
        var q = New<T>();
        q.topRows = 1;
        return q.Find<T>("Id==" + id);
    }

    public T LoadViewModel<T>(long id) where T : new()
    {
        topRows = 1;
        return Find<T>("Id==" + id);
    }

    public T Find<T>(string filter, LocalParameters filterValues = null)
        where T : new()
    {
        AddFilter(filter);
        SelectFieldsOfEntityIfNotSelected<T>();
        PreProcessQuery(filterValues);
        if (!DataSource.find(null, out T newRecord))
            newRecord = default;
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return newRecord;
    }

    public ElasticObject Find(long id)
    {
        return Find("Id==" + id, null);
    }

    public T Load<T>(long id) where T : new()
    {
        return Find<T>("Id==" + id, null);
    }

    public ElasticObject Find(string filter, LocalParameters filterValues)
    {
        AddFilter(filter);
        SelectFieldsOfEntityIfNotSelected();
        PreProcessQuery(filterValues);
        var output = DataSource.find(null);
        CommandTxt = DataSource.SqlCommand;
        ReleaseQuery();
        return output;
    }
    #endregion

    public long QueryTime { get; set; }

    private bool _onlyRecordCount;

    public long GetRecordCount(ElasticObject filterValues, LocalParameters lp)
    {
        _onlyRecordCount = true;
        var recordCount = 0L;
        if (GetDocuments(filterValues, lp))
        {
            var r = GetRecord();
            if (r != null)
                recordCount = r.GetLong("recordCount");
        }

        ReleaseQuery();
        _onlyRecordCount = false;
        return recordCount;
    }
    public long GetRecordCount()
    {
        _onlyRecordCount = true;
        long recordCount = 0;
        if (GetDocuments())
        {
            var r = GetRecord();
            if (r != null)
                recordCount = r.GetLong("recordCount");
        }

        ReleaseQuery();
        _onlyRecordCount = false;
        return recordCount;
    }

    public static Dictionary<long, string> FetchStringList(string namespaceId, string entityId,
        string fieldId = "Name")
    {
        var q = new QueryUtility(namespaceId, entityId, "QU.FetchStringList")
            .SelectField("Id")
            .SelectField(fieldId);
        var dic = new Dictionary<long, string>();
        if (!q.GetDocuments(null, null)) return dic;
        while (true)
        {
            var r = q.GetRecord();
            if (r == null) break;
            dic.Add(r.GetLong("Id"), r.GetString(fieldId));
        }

        return dic;
    }
}
