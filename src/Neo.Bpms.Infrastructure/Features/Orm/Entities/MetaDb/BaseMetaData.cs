using Neo.Bpms.Domain.Models.Cmmn;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.MetaDb;

public abstract class BaseMetaData
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    public ExceptionInfos Errors;
    public bool HasNotExistsTable;
    protected void AddError(string forField, string message, Exception e = null)
    {
        if (e != null)
        {
            ExceptionInfos.AddError(ref Errors, message, e);
            Logger.LogCritical(e, message);
        }
        else
        {
            ExceptionInfos.AddError(ref Errors, forField, message);
            Logger.LogError(message);
        }
    }
}
