using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;
using Neo.Bpms.Domain.Models.Cmmn.Storage;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public abstract partial class StorageInterfaceBase<TEntity, TLogicModel, TKey, TNullableKey> :
    MemoryStorage<TLogicModel, TKey, TNullableKey>, IDataSynchronizerItem<TKey>, IEntityChangedDriver
    where TEntity : IStateBasedEntityWithKey<TKey>, new()
    where TLogicModel : IdentityBase<TNullableKey>, IBaseClassId<TNullableKey>, new()
{
    public string Filter { get; set; }
    public Action<QueryUtility> SpecificSelectFields { get; set; }
    public Action<QueryUtility> SpecificFilters { get; set; }
    public bool ActiveDataSynchronizer { get; }

    protected StorageInterfaceBase(string filter, bool bLoad = true, bool activeDataSynchronizer = false)
    {
        Filter = filter;
        ActiveDataSynchronizer = activeDataSynchronizer;
        EntityAddress = ProjectDefinition.Project.FetchEntityAddress<TEntity>();
        Entity = ProjectDefinition.Project.GetEntity<TEntity>();
        if (bLoad)
        {
            Load();
        }
    }

    protected override void ClearEvent()
    {
        ApplyUtility au = GetApplyUtility();
        ElasticObject record = new() { [nameof(BpmsStateBasedEntity.StateId)] = StateBaseEntityId.Backup.ToInt() };
        if (!au.UpdateWithFilter(Filter, record))
        {
            string filter = $"{Entity.KeyFields?.FirstOrDefault()?.Id} IN ({string.Join(",", Items.Keys)})";
            ApplyUtility au2 = GetApplyUtility();
            if (!au2.UpdateWithFilter(filter, record))
            {
                foreach (TLogicModel item in Items.Values)
                {
                    ElasticObject itemRecord = new()
                    {
                        [nameof(BpmsStateBasedEntity.Id)] = item.Id,
                        [nameof(BpmsStateBasedEntity.StateId)] = StateBaseEntityId.Backup.ToInt()
                    };
                    ApplyUtility itemAu = GetApplyUtility();
                    if (!itemAu.Update(itemRecord))
                    {
                        _ = AddError(-3, $"Error in remove records : {au.ErrorText}");
                    }
                }
            }
        }
    }

    protected override void LoadData()
    {
        if (Items != null)
        {
            return;
        }

        QueryUtility q = EstablishQuery();
        Items = q.ToConcurrentDictionary<TKey, TLogicModel>();
        if (new TLogicModel() is ICategorizedLogicModel)
        {
            CategorizedItems =
                new ConcurrentDictionary<string,
                    ConcurrentDictionary<string, ConcurrentDictionary<TNullableKey, TLogicModel>>>();
        }
    }

    protected QueryUtility EstablishQuery()
    {
        QueryUtility q = QueryUtility<TEntity>
            .Where(BpmsStateBasedEntity.ActiveFilter);
        if (!string.IsNullOrEmpty(Filter))
        {
            _ = q.Where(Filter);
        }

        _ = q.SelectFieldsOfEntity();
        SpecificSelectFields?.Invoke(q);
        SpecificFilters?.Invoke(q);
        return q;
    }

    protected override bool InsertData(TLogicModel newItem, out string errorText)
    {
        ApplyUtility au = GetApplyUtility();
        bool ret = au.Insert(newItem);
        errorText = au.ErrorText;
        return ret;
    }

    protected override bool UpdateData(TLogicModel inItem, out string errorText)
    {
        ApplyUtility au = GetApplyUtility();
        bool ret = au.Update(inItem);
        errorText = au.ErrorText;
        return ret;
    }

    protected override bool DeleteData(TLogicModel inItem, out string errorText)
    {
        ApplyUtility au = GetApplyUtility();
        ElasticObject record = new()
        {
            [nameof(BpmsStateBasedEntity.Id)] = inItem.Id,
            [nameof(BpmsStateBasedEntity.StateId)] = StateBaseEntityId.Backup.ToInt()
        };
        bool ret = au.Update(record);
        errorText = au.ErrorText;
        return ret;
    }

    private ApplyUtility GetApplyUtility()
    {
        ApplyUtility au = ApplyUtility<TEntity>.New();
        au.SetEntityChangedReporterConfig(true, Name);
        return au;
    }

    public void ClearExpiredRequests()
    {
        List<TKey> emptyList = [];
        if (Requesters?.IsEmpty ?? true)
        {
            return;
        }

        foreach (var requester in Requesters)
        {
            List<KeyValuePair<string, IDataSynchronizerRequester<TKey>>> listOfTimeout = requester.Value
                .Where(r => r.Value.RequesterLifecycle == RequesterLifecycle.DueTime && r.Value.RequestTime + r.Value.Timeout < DateTime.Now)
                .ToList();
            //foreach (KeyValuePair<string, IDataSynchronizerRequester<TKey>> to in listOfTimeout)
            //{
            //    _ = requester.Value.TryRemove(to);
            //}

            if (requester.Value.Values.Count == 0)
            {
                emptyList.Add(requester.Key);
            }
        }

        foreach (TKey to in emptyList)
        {
            _ = Requesters.TryRemove(to, out _);
        }

        if (Requesters.Values.Count == 0)
        {
            Requesters.Clear();
        }
    }
}
