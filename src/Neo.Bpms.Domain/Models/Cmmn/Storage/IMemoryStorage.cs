namespace Neo.Bpms.Domain.Models.Cmmn.Storage;

public interface IMemoryStorageBase
{
    ConcurrentDictionary<string, List<string>> Errors { get; }
    void AfterLoad();
}
public interface IMemoryStorage<TKey, TNullableKey, TLogicModel> : IMemoryStorageBase
    where TLogicModel : IdentityBase<TNullableKey>, IBaseClassId<TNullableKey>, new()
{
    ConcurrentDictionary<TKey, TLogicModel> Items { get; }

    void Load();
    bool Save(IList<TLogicModel> inItems);
    bool Save(TLogicModel inItem);
    bool Insert(IList<TLogicModel> inItems);
    bool Insert(TLogicModel inItem);
    bool Delete(TLogicModel inItem);
    void Clear();
    bool GetItem(TNullableKey keyId, out TLogicModel item);
    TLogicModel GetItem(TNullableKey keyId);
    TLogicModel FirstOrDefault(Func<TLogicModel, bool> func);
    IEnumerable<TLogicModel> Where(Func<TLogicModel, bool> func);

    TLogicModel GetOrAdd(string name, string key, Func<string, TLogicModel> createNewItem);
    TLogicModel GetCategorizedItem(string name, string key);
    TLogicModel GetCategorizedItem(string name, string key, Func<TLogicModel, bool> func);
    IEnumerable<TLogicModel> GetCategorizedItems(string name, TNullableKey id);
    IEnumerable<TLogicModel> GetCategorizedItems(string name, string key);
    IEnumerable<TLogicModel> GetCategorizedItems(string name, string key, Func<TLogicModel, bool> func);

    //ConcurrentDictionary<TKey, List<TLogicModel>> ToDictionaryOfList(Func<TLogicModel, TKey> keySelector);
    void ClearItem(TLogicModel item);
}