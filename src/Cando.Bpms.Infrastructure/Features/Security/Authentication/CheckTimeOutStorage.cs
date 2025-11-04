using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

namespace Neo.Bpms.Infrastructure.Features.Security.Authentication;

internal static class StorageTools
{
    internal static string CheckTimeOutFilter =>
        $"((ISNULL({nameof(ICheckTimeOut.StartDate)},0)==0) OR ({nameof(ICheckTimeOut.StartDate)}<=serverdatetime()))" +
        $"AND ((ISNULL({nameof(ICheckTimeOut.EndDate)},0)==0) OR ({nameof(ICheckTimeOut.EndDate)}>=serverdatetime()))";

    internal static string CanBeDisableFilter => $"(IsNULL({nameof(ICanBeDisable.Disable)},0)==0)";

    public static bool CheckTimeOut<TLogicModel>(TLogicModel item)
        where TLogicModel : ICheckTimeOut
    {
        var time = new DateTime();
        return item.EndDate == null && item.EndDate < time ||
               item.StartDate == null && item.StartDate > time;
    }
    public static string AppendFilter(string filter1, string filter2)
    {
        return (string.IsNullOrEmpty(filter1) ? "" : $"({filter1}) And ") + filter2;
    }
}

public class CanBeDisableStorage<TEntity, TLogicModel>(string filter = null) : StorageInterface<TEntity, TLogicModel>(StorageTools.AppendFilter(filter, StorageTools.CanBeDisableFilter), null)
    where TEntity : IStateBasedEntity, ICanBeDisable, new()
    where TLogicModel : IdentityBase<long?>, ICanBeDisable, new()
{
    protected override bool IsNotInThisStorage(TLogicModel item)
    {
        return item.Disable || base.IsNotInThisStorage(item);
    }
}

public class CheckTimeOutStorage<TEntity, TLogicModel>(string filter = null) : CanBeDisableStorage<TEntity, TLogicModel>(StorageTools.AppendFilter(filter, StorageTools.CheckTimeOutFilter))
    where TEntity : IStateBasedEntity, ICheckTimeOut, new()
    where TLogicModel : IdentityBase<long?>, ICanBeDisable, ICheckTimeOut, new()
{
    protected override bool IsNotInThisStorage(TLogicModel item)
    {
        return StorageTools.CheckTimeOut(item) || base.IsNotInThisStorage(item);
    }
}

public class CanBeDisableStorageWithStringKey<TEntity, TLogicModel>(string filter = null)
    : StorageInterfaceWithStringKey<TEntity, TLogicModel>(StorageTools.AppendFilter(filter, StorageTools.CanBeDisableFilter))
    where TEntity : BpmsStateBasedEntityStringKey, ICanBeDisable, new()
    where TLogicModel : IdentityBase<string>, ICanBeDisable, new()
{
    protected override bool IsNotInThisStorage(TLogicModel item)
    {
        return item.Disable || base.IsNotInThisStorage(item);
    }
}

public class CheckTimeOutStorageWithStringKey<TEntity, TLogicModel>(string filter = null)
    : CanBeDisableStorageWithStringKey<TEntity, TLogicModel>(StorageTools.AppendFilter(filter, StorageTools.CheckTimeOutFilter))
    where TEntity : BpmsStateBasedEntityStringKey, ICheckTimeOut, new()
    where TLogicModel : IdentityBase<string>, ICheckTimeOut, new()
{
    protected override bool IsNotInThisStorage(TLogicModel item)
    {
        return StorageTools.CheckTimeOut(item) || base.IsNotInThisStorage(item);
    }
}
