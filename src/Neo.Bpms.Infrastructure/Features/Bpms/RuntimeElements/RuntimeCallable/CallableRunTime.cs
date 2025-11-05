namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

/// <summary>
/// Base class for all elements runtime (both GlobalTaskRuntime and FlowNodeRuntime)
/// </summary>
public abstract class CallableRunTime
{
    /// <summary>
    /// Withdraw Obsolete
    /// </summary>
    /// <returns></returns>
    internal virtual void WithdrawObsolete()
    {
    }
}