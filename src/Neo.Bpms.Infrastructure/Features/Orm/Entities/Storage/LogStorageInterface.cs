using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public class LogStorageInterface<TEntity> : ILogStorageInterface<TEntity>
    where TEntity : BpmsBaseEntity, new()
{
    public ConcurrentDictionary<string, List<string>> Errors { get; }

    public LogStorageInterface()
    {
        Errors = new ConcurrentDictionary<string, List<string>>();
    }

    public void AfterLoad()
    {
        //todo
    }

    public bool Insert(IList<TEntity> newItems)
    {
        var b = true;
        foreach (var newItem in newItems)
        {
            if (!Insert(newItem))
                b = false;
        }

        return b;
    }

    public virtual bool Insert(TEntity newItem)
    {
        var au = ApplyUtility<TEntity>.New();
        var b = au.Insert(newItem);
        if (b && newItem.Id > 0)
        {
        }
        else
            return AddError(-1, $"Error in insert : {au.ErrorText}");

        return true;
    }

    protected bool AddError(long? errorId, string errorText)
    {
        var errorItems = Errors.AddOrGetItem(errorId?.ToString() ?? "");
        if (errorId == null || errorId == 0 || errorItems.Count == 0)
            errorItems.Add($"Error in {typeof(TEntity).Name}. {errorText}");
        return false;
    }
}
