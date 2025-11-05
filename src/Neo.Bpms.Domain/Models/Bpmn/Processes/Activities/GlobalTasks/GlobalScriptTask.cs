using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.GlobalTasks;

public class GlobalScriptTask(BpmnDefinitions parent, string id, string name) 
    : GlobalTask(parent, id, name, eGlobalTaskType.Script)
{
    public ScriptTask.eScriptLanguage scriptLanguage;

    /// <summary>
    /// Defines the format of the script. This attribute value MUST be specified with a mime-type format. And it MUST be specified if a script is provided.
    /// </summary>
    public string scriptFormat;

    /// <summary>
    /// The modeler MAY include a script that can be run when the Task is performed. 
    /// If a script is not included, then the Task will act as the equivalent of an Abstract Task
    /// </summary>
    public string script;

    public void setScript(string script, ScriptTask.eScriptLanguage scriptLanguage)
    {
        this.script = script;
        this.scriptLanguage = scriptLanguage;
    }
}
