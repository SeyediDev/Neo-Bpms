using Neo.Bpms.Domain.Entities.Cmmn.DataSynchronization;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public class ConcurrentDictionaryHandler<TKey, TIdentified>(string name, EntityAddress entityAddress) 
    : ConcurrentDictionary<TKey, TIdentified>,
    IEntityChangedDriver
    where TIdentified : IdentityBase<TKey>, new()
{
    public string Name { get; } = name;
    public EntityAddress EntityAddress { get; } = entityAddress;

    public void InsertDBReport<T>(T record)
    {
        TIdentified identityUser = FetchRecord(record, false);
        _ = TryAdd(identityUser.Id, identityUser);
    }

    public void UpdateDBReport<T>(T record)
    {
        TIdentified newUser = FetchRecord(record, true);
        TIdentified oldItem = GetItem(newUser.Id);
        _ = TryUpdate(newUser.Id, newUser, oldItem);
    }

    public void UpdateGroupDBReport(List<ExpressionNode> filters)
    {
        ReloadDBReport(); //todo
    }

    public void DeleteDBReport<T>(T record)
    {
        TIdentified identityUser = FetchRecord(record, true);
        _ = TryRemove(identityUser.Id, out _);
    }

    public void DeleteGroupDBReport(ExpressionNode deleteFilter)
    {
        ReloadDBReport(); //todo
    }

    public void ReloadDBReport()
    {
        Clear();
    }

    private TIdentified FetchRecord<T>(T record, bool loadOld)
    {
        switch (record)
        {
            case TIdentified item:
                return item;
            case ElasticObject eRecord:
                TIdentified oldItem = loadOld ? GetItem(eRecord["Id"] is TKey ? (TKey)eRecord["Id"] : default) : default;
                if (oldItem == null)
                {
                    return eRecord.To<TIdentified>();
                }

                ElasticObject eItem = ElasticObject.ToElastic(oldItem);
                _ = eItem.Merge(eRecord);
                return eItem.To<TIdentified>();
            default:
                return default;
        }
    }

    private TIdentified GetItem(TKey userId)
    {
        _ = TryGetValue(userId, out TIdentified user);
        return user;
    }

    public List<IDataSynchronizerRequester<TKey>> Requesters { get; set; }
    public virtual bool HasAnySyncRequest => Requesters?.Any() ?? false;
    public TimeSpan Interval { get; set; }
    public DateTime LastSynchronize { get; set; }

    public virtual void DoSynchronization()
    {
    }
}
