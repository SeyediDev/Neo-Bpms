using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class OperationsMaintainerTimer
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    private readonly Timer _timer;
    private readonly IList<IBPMNOperationRuntime> _operationRuntimes;

    public OperationsMaintainerTimer(IList<IBPMNOperationRuntime> operationRuntimes)
    {
        _operationRuntimes = operationRuntimes;
        _timer = new Timer(TryAllocationForIdleResources, new AutoResetEvent(false),
            TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5));
    }

    private void TryAllocationForIdleResources(object stateInfo)
    {
        try
        {
            foreach (IBPMNOperationRuntime operationRuntime in _operationRuntimes)
            {
                operationRuntime.BPMNOperationRuntime.RunTaskFromQueue(false);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
        }
    }
}
