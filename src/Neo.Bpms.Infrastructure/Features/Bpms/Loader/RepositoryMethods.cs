using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    public ProcessRunTime GetProcessRuntime(string processId)
    {
        if (string.IsNullOrEmpty(processId))
        {
            return null;
        }

        _ = processesRunTimes.TryGetValue(processId, out ProcessRunTime process);
        return process;
    }

    public ProcessRunTime GetProcessRuntimeByDbId(long? processId)
    {
        return processId == null || processId.Value == 0
            ? null
            : processesRunTimes.Values.FirstOrDefault(p => p.DbId == processId.Value);
    }

    public ProcessVersionRuntime GetProcessVersionRuntimeByDbId(long? processVersionId)
    {
        if (processVersionId == null || processVersionId.Value == 0)
        {
            return null;
        }

        _ = ProcessVersionRunTimes.TryGetValue(processVersionId.Value, out ProcessVersionRuntime processVersionRuntime);
        return processVersionRuntime;
    }

    public ProcessVersionRuntime GetProcessVersionRuntime(string processId, string versionId)
    {
        return GetProcessRuntime(processId)?.GetVersion(versionId);
    }

    public FlowNodeRunTime GetFlowNodeRunTime(long flowNodeId)
    {
        if (flowNodeId <= 0)
        {
            return null;
        }

        _ = FlowNodeRunTimes.TryGetValue(flowNodeId, out FlowNodeRunTime flowNodeRunTime);
        return flowNodeRunTime;
    }
}
