using Task = Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.Task;


namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class TaskDefinition<TTask> : ActivityDefinition<TTask>
    where TTask : Task
{
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Sets Task Priority
    /// </summary>
    /// <param name="priorityLevel">The priority Level</param>
    protected void SetTaskPriority(double priorityLevel)
    {
        if (_curTask == null) throw new Exception("SetTaskPriority used incorrectly.");
        _curTask.priorityLevel = priorityLevel;
        _curTask.priorityLevelProperty = null;
    }

    /// <summary>
    /// Sets Task Priority Property
    /// </summary>
    /// <param name="priorityLevelProperty">The priority level property</param>
    protected void SetTaskPriorityProperty(string priorityLevelProperty)
    {
        if (_curTask == null) throw new Exception("SetTaskPriority used incorrectly.");
        _curTask.priorityLevel = 0;
        _curTask.priorityLevelProperty = priorityLevelProperty;
    }

    /// <summary>
    /// Adds the task.
    /// </summary>
    /// <param name="lane">The lane.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="task">The task.</param>
    /// <returns></returns>
    private void AddTask(Lane lane, object outputStateId, Task task)
    {
        if (AddActivity(lane, outputStateId, task) == null) return;
        _curTask = task;
    }

    private Task _curTask;
}