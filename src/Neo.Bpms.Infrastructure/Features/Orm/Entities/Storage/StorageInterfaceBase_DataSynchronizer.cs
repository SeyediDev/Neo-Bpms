using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.DataSynchronization;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

// ReSharper disable once UnusedTypeParameter
public abstract partial class StorageInterfaceBase<TEntity, TLogicModel, TKey, TNullableKey> :
    // ReSharper disable RedundantExtendsListEntry
    MemoryStorage<TLogicModel, TKey, TNullableKey>, IEntityChangedDriver, IDataSynchronizerItem<TKey>
    where TEntity : IStateBasedEntityWithKey<TKey>, new()
    where TLogicModel : IdentityBase<TNullableKey>, IBaseClassId<TNullableKey>, new()
{
    public ConcurrentDictionary<TKey, ConcurrentDictionary<string, IDataSynchronizerRequester<TKey>>> Requesters { get; set; }
    public virtual bool HasAnySyncRequest => Requesters?.Any() ?? false;
    public TimeSpan Interval => Entity.DoSynchronizationInterval; //todo load from Entity loader
    public DateTime LastSynchronize { get; set; }
    private bool _onSynchronization;

    public void DoSynchronization()
    {
        if (!ActiveDataSynchronizer)
            return;
        if (_onSynchronization)
            return;
        _onSynchronization = true;
        var q = EstablishQuery();
        var filter = SynchronizationFilter();
        if (!string.IsNullOrEmpty(filter))
            q.Where(SynchronizationFilter());
        //var @params = SynchronizationSelectFields();
        //q.SelectFields(@params.ToArray()); //read selected fields is not supported in snmp data source
        var requestedItems = q.ToDictionary<TKey, TLogicModel>(SynchronizationFilterValues());
        _onSynchronization = false;
        ResponseMethod(requestedItems);
    }

    protected abstract void ResponseMethod(Dictionary<TKey, TLogicModel> requestedItems);
    protected abstract string SynchronizationFilter();
    protected abstract List<string> SynchronizationSelectFields();
    protected abstract LocalParameters SynchronizationFilterValues();

    protected static void SendResponse(ConcurrentDictionary<string, IDataSynchronizerRequester<string>> requesters,
        TLogicModel requestItem,
        SyncItemState state, IList<string> @params)
    {
        foreach (var requester in requesters.Values)
        {
            requester.Response(requestItem, state, @params);
        }
    }

    protected static void SendBulkResponse(IEnumerable<IDataSynchronizerRequester<string>> requesters,
        Dictionary<string, TLogicModel> requestItem, SyncItemState state, IList<string> @params)
    {
        foreach (var requester in requesters.ToList())
        {
            requester.Response(requestItem.ToDictionary(k => k.Key, v => v.Value), state, @params);
        }
    }

    protected SyncItemState UpdateMemory(TLogicModel item)
    {
        if (item == null)
            return SyncItemState.NotSeen;
        Items ??= new ConcurrentDictionary<TKey, TLogicModel>();
        if (!Items.TryGetValue(FetchKeyValue(item.Id), out var oldItem))
        {
            Items.TryAdd(FetchKeyValue(item.Id), item);
            if (item is ICategorizedLogicModel catItem)
                AddToCategorizedItems(item, catItem);
            return SyncItemState.Insert;
        }

        var oldCategorizedItem = oldItem as ICategorizedLogicModel;
        var changeList = new List<ChangedItem>();
        var members = ReflectionField.Members(item.GetType());
        foreach (var member in members)
        {
            var value = ReflectionField.GetValue(item, member);
            if (value != ReflectionField.GetValue(oldItem, member))
            {
                changeList.Add(new ChangedItem
                {
                    MemberInfo = member,
                    NewValue = value
                });
            }
        }

        if (changeList.Count != 0 && oldCategorizedItem != null)
            RemoveFromCategorizedItems(oldItem, oldCategorizedItem);
        foreach (var changedItem in changeList)
            ReflectionField.SetValue(oldItem, changedItem.MemberInfo, changedItem.NewValue);
        if (changeList.Count != 0)
            SetRecord(oldItem);
        if (changeList.Count != 0 && oldCategorizedItem != null)
            AddToCategorizedItems(oldItem, oldCategorizedItem);

        return changeList.Count != 0 ? SyncItemState.Update : SyncItemState.Idle;
    }

    private class ChangedItem
    {
        public object NewValue;
        public MemberInfo MemberInfo;
    }
}
