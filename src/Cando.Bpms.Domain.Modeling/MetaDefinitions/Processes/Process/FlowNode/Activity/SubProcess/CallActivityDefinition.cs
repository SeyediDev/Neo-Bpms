using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    protected CallActivity currentCallActivity;

    /// <summary>
    /// Adds the call activity.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="processOrGlobalTaskName">The process Or GlobalTask Name</param>
    /// <param name="callActivityTypeId">The call activity type</param>
    /// <returns></returns>
    protected CallActivity AddCallActivity(string actionId, string name, object outputStateId,
        string processOrGlobalTaskName,
        Activity.CallActivityTypeId callActivityTypeId = Activity.CallActivityTypeId.SubProcess)
    {
        currentCallActivity = new CallActivity(process, actionId, name, processOrGlobalTaskName, callActivityTypeId);
        AddActivity(_currentLane, outputStateId, currentCallActivity);
        return currentCallActivity;
    }

    /// <summary>
    /// Adds the call activity.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected CallActivity AddCallActivity<TProcess>(string actionId, string name, object outputStateId = null)
        where TProcess : ProcessDefinition
    {
        currentCallActivity = new CallActivity(process, actionId, name, typeof(TProcess).Name,
            Activity.CallActivityTypeId.SubProcess);
        AddActivity(_currentLane, outputStateId, currentCallActivity);
        return currentCallActivity;
    }
}
