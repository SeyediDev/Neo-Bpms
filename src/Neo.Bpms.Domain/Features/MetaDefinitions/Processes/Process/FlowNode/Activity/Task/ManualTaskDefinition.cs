namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class ManualTaskDefinition : TaskDefinition<ManualTask>
{
    protected sealed override ManualTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new ManualTask(flowElementsContainer, ElementId, Name);
    }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the manual task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="bControlBySystem">if set to <c>true</c> [b control by system].</param>
    /// <returns></returns>
    protected ManualTask AddManualTask(string actionId, string name, object outputStateId, bool bControlBySystem)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        ManualTask task = new(process, actionId, name) { ControlBySystem = bControlBySystem };
        AddTask(_currentLane, outputStateId, task);
        return task;
    }
}
