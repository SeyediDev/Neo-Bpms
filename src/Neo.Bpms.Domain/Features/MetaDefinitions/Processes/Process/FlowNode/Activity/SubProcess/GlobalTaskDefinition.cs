using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.GlobalTasks;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    //private GlobalTask _currentGlobalTask;

    /// <summary>
    /// Adds the global task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns></returns>
    private void AddGlobalTask(GlobalTask task)
    {
        if (definitions == null || process == null)
        {
            return;
        }
        //_currentGlobalTask = task;
        currentBaseElement = task;
        if (task.taskType != GlobalTask.eGlobalTaskType.Manual)
        {
            //DataOperation operation = new DataOperation(definitions, actionId, name);
            //definitions.addDataOperation(operation);
            //currentGlobalTask.dataOperation = operation;
            //operation.outputStateId = outputStateId;
        }
    }

    /// <summary>
    /// Adds the global user task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    protected GlobalUserTask AddGlobalUserTask(string actionId, string name)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        GlobalUserTask task = new(definitions, actionId, name);
        AddGlobalTask(task);
        return task;
    }

    /// <summary>
    /// Adds the global manual task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="bControlBySystem">if set to <c>true</c> [b control by system].</param>
    /// <returns></returns>
    protected GlobalManualTask AddGlobalManualTask(string actionId, string name, bool bControlBySystem)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        GlobalManualTask task = new(definitions, actionId, name) { bControlBySystem = bControlBySystem };
        AddGlobalTask(task);
        return task;
    }

    /// <summary>
    /// Adds the global script task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="script">The script.</param>
    /// <param name="scriptLanguage">The script language.</param>
    /// <returns></returns>
    protected GlobalScriptTask AddGlobalScriptTask(string actionId, string name, string script,
        ScriptTask.eScriptLanguage scriptLanguage)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        GlobalScriptTask task = new(definitions, actionId, name);
        task.setScript(script, scriptLanguage);
        AddGlobalTask(task);
        return task;
    }

    /// <summary>
    /// Adds the global business rule task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    protected GlobalBusinessRuleTask AddGlobalBusinessRuleTask(string actionId, string name)
    //, params Decision[] decisions
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        GlobalBusinessRuleTask task = new(definitions, actionId, name);
        //foreach (var decision in decisions)
        //{
        //	task.addDecision(decision);
        //}
        AddGlobalTask(task);
        return task;
    }

    //bool SetEntityMatchFieldId(string entityMatchFieldId) { }
}
