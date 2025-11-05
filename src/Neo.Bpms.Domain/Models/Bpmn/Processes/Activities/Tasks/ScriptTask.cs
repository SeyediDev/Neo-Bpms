namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

/// <summary>
/// A Script Task is executed by a business process engine. 
/// The modeler or implementer defines a script in a language that the engine can interpret. 
/// When the Task is ready to start, the engine will execute the script. When the script is completed,
/// the Task will also be completed.
/// </summary>
public class ScriptTask(IFlowElementsContainer flowElementsContainer,
    string id, string name) : Task(flowElementsContainer, id, name, eActivityType.ScriptTask)
{
    #region not in BPMN

    public eScriptLanguage scriptLanguage;

    #endregion

    /// <summary>
    /// Defines the format of the script. This attribute value MUST be specified with a mime-type format. And it MUST be specified if a script is provided.
    /// </summary>
    public string scriptFormat;

    /// <summary>
    /// The modeler MAY include a script that can be run when the Task is performed. 
    /// If a script is not included, then the Task will act as the equivalent of an Abstract Task
    /// </summary>
    public string script;

    public void setScript(string script, eScriptLanguage scriptLanguage)
    {
        this.script = script;
        this.scriptLanguage = scriptLanguage;
    }

    public enum eScriptLanguage
    {
        CSharp,
        JS,
        VB,
        CPP
    }
}
