namespace Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;
public interface IDataSynchronizerItem : IEntityReference
{
    bool HasAnySyncRequest { get; }
    TimeSpan Interval { get; }
    DateTime LastSynchronize { get; set; }
    void DoSynchronization();
    void ClearExpiredRequests();
}

public interface IDataSynchronizerItem<TKey> : IDataSynchronizerItem
{
    ConcurrentDictionary<TKey, ConcurrentDictionary<string, IDataSynchronizerRequester<TKey>>> Requesters { get; }
}

public interface IDataSynchronizerRequester<TKey>
{
    TKey Id { get; }
    RequesterLifecycle RequesterLifecycle { get; }
    TimeSpan Timeout { get; }
    DateTime RequestTime { get; set; }
    RequestType RequestType { get; set; }
    void Response<TLogic>(TLogic item, SyncItemState state, IList<string> @params);
    void Response<TLogic>(Dictionary<TKey, TLogic> itemList, SyncItemState state, IList<string> @params);
}

public enum RequestType
{
    Get,
    Walk
}

public enum RequesterLifecycle
{
    //Once, todo future feature
    //UntilFirstSync,
    Lifetime,
    DueTime
}

public enum SyncItemState
{
    Insert = 1,
    Update = 2,
    Idle = 3,
    NotSeen = 4
}