using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public abstract class MemoryStorage<TLogicModel, TKey, TNullableKey> :
    IMemoryStorage<TKey, TNullableKey, TLogicModel>
    where TLogicModel : IdentityBase<TNullableKey>, IBaseClassId<TNullableKey>, new()
{
    public ConcurrentDictionary<TKey, TLogicModel> Items { get; protected set; }

    public ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<TNullableKey, TLogicModel>>>
        CategorizedItems
    { get; protected set; }

    public ConcurrentDictionary<string, List<string>> Errors { get; set; }
    private readonly object _categorizedItemsLock = new();

    protected MemoryStorage()
    {
        Errors = new ConcurrentDictionary<string, List<string>>();
    }

    public void Clear()
    {
        ClearEvent();
        AfterClear();
    }

    protected abstract void ClearEvent();

    public bool GetItem(TNullableKey keyId, out TLogicModel item)
    {
        item = GetItem(keyId);
        return item != null;
    }

    public TLogicModel GetItem(TNullableKey keyId)
    {
        var item = GetItemPrivateCall(keyId);
        //			if (item == null)
        //				AddError(keyId, $"Invalid request id {keyId}.");
        return item;
    }

    public TLogicModel FirstOrDefault(Func<TLogicModel, bool> func)
    {
        return Items.Values.FirstOrDefault(func);
    }

    public IEnumerable<TLogicModel> Where(Func<TLogicModel, bool> func)
    {
        return Items.Values.Where(func);
    }

    public TLogicModel GetCategorizedItem(string name, string key)
    {
        var catItems = FetchCategorizedItems(name);
        var items = catItems?.GetItem(key ?? "");
        return items?.FirstOrDefault().Value;
    }

    private ConcurrentDictionary<string, ConcurrentDictionary<TNullableKey, TLogicModel>> FetchCategorizedItems(
        string name)
    {
        if (CategorizedItems == null)
        {
            throw new ArgumentNullException(typeof(TLogicModel).Name, name);
        }

        lock (_categorizedItemsLock)
        {
            return CategorizedItems.CreateInstanceOrGetItem(name,
                o => Items.Values.ToList()
                    .ToConcurrentDictionaryOfList<TNullableKey, TLogicModel>(item =>
                    {
                        var categorizedLogicModel = item as ICategorizedLogicModel;
                        return categorizedLogicModel?.CategorizedKey(name);
                    }));
        }
    }

    public TLogicModel GetCategorizedItem(string name, string key, Func<TLogicModel, bool> func)
    {
        var catItems = FetchCategorizedItems(name);
        var items = catItems?.GetItem(key ?? "");
        return items?.Values.FirstOrDefault(func);
    }

    public IEnumerable<TLogicModel> GetCategorizedItems(string name, TNullableKey id)
    {
        return GetCategorizedItems(name, id?.ToString());
    }

    public IEnumerable<TLogicModel> GetCategorizedItems(string name, string key)
    {
        var catItems = FetchCategorizedItems(name);
        return catItems?.GetItem(key)?.Values ?? Enumerable.Empty<TLogicModel>();
    }

    public IEnumerable<TLogicModel> GetCategorizedItems(string name, string key, Func<TLogicModel, bool> func)
    {
        var catItems = FetchCategorizedItems(name);
        return catItems?.GetItem(key)?.Values.Where(func) ?? [];
    }

    // public ConcurrentDictionary<TKey, List<TLogicModel>> ToDictionaryOfList(Func<TLogicModel, TKey> keySelector)
    // {
    // 	return Items.ToConcurrentDictionaryOfListWithKey(keySelector, NullValueReplacement);
    // }

    protected abstract TKey NullValueReplacement();


    public void ClearItem(TLogicModel item)
    {
        //todo
    }

    public TLogicModel GetOrAdd(string name, string key, Func<string, TLogicModel> createNewItem)
    {
        var item = GetCategorizedItem(name, key);
        if (item == null)
        {
            item = createNewItem(key);
            Insert(item);
        }

        return item;
    }

    public void Load()
    {
        LoadData();
    }

    protected abstract void LoadData();

    public void AfterLoad()
    {
        if (Items == null) return;
        foreach (var item in Items.Values)
            SetRecord(item);
    }

    public virtual bool Save(IList<TLogicModel> inItems)
    {
        foreach (var inItem in inItems)
        {
            if (!Save(inItem))
                return false;
        }

        return true;
    }

    public bool Insert(IList<TLogicModel> newItems)
    {
        var ret = true;
        foreach (var newItem in newItems)
        {
            if (!Insert(newItem))
                ret = false;
        }

        return ret;
    }

    public virtual bool Insert(TLogicModel newItem)
    {
        if (InsertData(newItem, out var errorText))
            AfterInsert(newItem);
        else
            return AddError(-1, $"Error in insert : {errorText}");
        return true;
    }

    protected abstract bool InsertData(TLogicModel newItem, out string errorText);

    public virtual bool Save(TLogicModel inItem)
    {
        var oldItem = GetItemPrivateCall(inItem.Id);
        if (oldItem == null || inItem.Id == null)
            return Insert(inItem);
        if (!UpdateData(inItem, out var errorText))
            return AddError(-2, $"Error in update record {inItem.Id} : {errorText}");
        AfterUpdate(inItem, oldItem);
        return true;
    }

    protected abstract bool UpdateData(TLogicModel inItem, out string errorText);

    public virtual bool Delete(TLogicModel inItem)
    {
        if (inItem.Id == null) return false;
        if (!DeleteData(inItem, out var errorText))
            return AddError(-3, $"Error in remove record {inItem.Id} : {errorText}");
        AfterDelete(inItem);
        return true;
    }

    protected abstract bool DeleteData(TLogicModel inItem, out string errorText);

    protected void AddToCategorizedItems(TLogicModel inItem, ICategorizedLogicModel catItem)
    {
        CategorizedItems ??=
            new ConcurrentDictionary<string,
                ConcurrentDictionary<string, ConcurrentDictionary<TNullableKey, TLogicModel>>>();
        foreach (var catItems in CategorizedItems)
        {
            var key = CategorizeKey(catItems.Key, catItem);
            var items = catItems.Value.AddOrGetItem(key);
            items.TryAdd(inItem.Id, inItem);
        }
    }

    private static string CategorizeKey(string name, ICategorizedLogicModel catItem)
    {
        return catItem.CategorizedKey(name);
    }

    protected void AfterInsert(TLogicModel newItem)
    {
        if (CheckKeyIsEmpty(newItem)) return;
        SetRecord(newItem);
        Items ??= new ConcurrentDictionary<TKey, TLogicModel>();
        Items.TryAdd(FetchKeyValue(newItem), newItem);
        if (newItem is ICategorizedLogicModel catItem)
            AddToCategorizedItems(newItem, catItem);
    }

    protected void AfterUpdate(TLogicModel newItem, TLogicModel oldItem)
    {
        if (CheckKeyIsEmpty(newItem)) return;
        SetRecord(newItem);
        var oldCategorizedItem = oldItem as ICategorizedLogicModel;
        if (oldCategorizedItem != null)
            RemoveFromCategorizedItems(oldItem, oldCategorizedItem);
        if (!oldItem.Equals(newItem))
            ReflectionField.Copy(oldItem, newItem);
        if (oldCategorizedItem != null)
            AddToCategorizedItems(oldItem, oldCategorizedItem);
    }

    public abstract void SetRecord(TLogicModel item);

    protected void AfterDelete(TLogicModel item)
    {
        if (item == null || item.Id == null) return;
        Items.TryRemove(FetchKeyValue(item), out _);
        if (item is ICategorizedLogicModel catItem)
            RemoveFromCategorizedItems(item, catItem);
    }

    protected void RemoveFromCategorizedItems(TLogicModel item, ICategorizedLogicModel catItem)
    {
        foreach (var catItems in CategorizedItems)
        {
            var key = CategorizeKey(catItems.Key, catItem);
            if (catItems.Value.TryGetValue(key, out var items))
                items.TryRemove(item.Id, out _);
            if (items != null && items.IsEmpty)
                catItems.Value.TryRemove(key, out _);
        }
    }

    protected void AfterClear()
    {
        Items.Clear();
        CategorizedItems?.Clear();
        Errors?.Clear();
    }

    protected bool AddError(TNullableKey errorId, string errorText)
    {
        var errorItems = Errors.AddOrGetItem(errorId?.ToString() ?? "");
        if (CheckKeyIsEmpty(errorId) || errorItems.Count == 0)
            errorItems.Add($"Error in {typeof(TLogicModel).Name}. {errorText}");
        return false;
    }

    protected bool AddError(long errorId, string errorText)
    {
        return AddError(errorId.ToString(), errorText);
    }

    protected bool AddError(string errorId, string errorText)
    {
        var errorItems = Errors.AddOrGetItem(errorId);
        if (string.IsNullOrEmpty(errorId) || errorItems.Count == 0)
            errorItems.Add($"Error in {typeof(TLogicModel).Name}. {errorText}");
        return false;
    }

    protected abstract bool CheckKeyIsEmpty(TNullableKey keyId);
    protected abstract TKey FetchKeyValue(TNullableKey keyId);
    private TLogicModel GetItemPrivateCall(TNullableKey keyId)
    {
        if (CheckKeyIsEmpty(keyId))
            return default;
        Items.TryGetValue(FetchKeyValue(keyId), out var item);
        return item;
    }

    private bool CheckKeyIsEmpty(TLogicModel item)
    {
        return item == null || CheckKeyIsEmpty(item.Id);
    }

    private TKey FetchKeyValue(TLogicModel item)
    {
        return FetchKeyValue(item.Id);
    }
}
