using Neo.Bpms.Domain.Entities.Bpmn.Execution;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;
public static partial class DataStorage
{
    #region conversion functions

    private static long getScopeState(ProcessInstanceStateId eState)
    {
        return (long)eState;
    }

    private static long getActivityState(ActivityInstanceStateId eActivityState)
    {
        return (long)eActivityState;
    }

    private static string Serialize(Dictionary<string, ReceiveTokenInfo> receiveToken)
    {
        return string.Join(",", receiveToken.Select(rt => rt.Key + ":" + rt.Value.receiveTokenCount));
    }

    #endregion
}
