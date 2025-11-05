using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class ScriptTaskDefinition : TaskDefinition<ScriptTask>
{
    protected override ScriptTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new ScriptTask(flowElementsContainer, ElementId, Name);
    }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the script task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="script">The script.</param>
    /// <param name="scriptLanguage">The script language.</param>
    /// <returns></returns>
    protected ScriptTask AddScriptTask(string actionId, string name, string script,
        ScriptTask.eScriptLanguage scriptLanguage, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var task = new ScriptTask(process, actionId, name);
        task.setScript(script, scriptLanguage);
        AddTask(_currentLane, outputStateId, task);
        return task;
    }
}
