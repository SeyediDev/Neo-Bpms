namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class BpmnDefinitionsDefinition
{
    /// <summary>
    /// Defines Process
    /// </summary>
    /// <typeparam name="T">Process</typeparam>
    /// <param name="isActiveLog">if set to <c>true</c> activate log for process</param>
    /// <returns></returns>
    protected bool DefineProcess<T>(bool isActiveLog = false) where T : ProcessDefinition, new()
    {
        Process process = DefineOneProcess<T>(isActiveLog);
        currentBaseElement = process;
        return process != null;
    }

    public static Process DefineOneProcess<T>(bool isActiveLog)
        where T : ProcessDefinition, new()
    {
        T t = new();
        Type processDefinitionType = t.GetType();
        Type[] types = [];
        ConstructorInfo cons = processDefinitionType.GetConstructor(types);
        object[] parameters = [];
        if (cons?.Invoke(parameters) is not ProcessDefinition processDefinition)
        {
            return null;
        }

        BpmnDefinitions processDefinitions = processDefinition.DefineAll();
        Process process = processDefinitions.Process;
        process.auditing ??= new Auditing(process, process.Id + ".Auditing");
        process.auditing.generateTraceLog = isActiveLog;
        process.auditing.saveInstances = true; //todo

        BusinessProcessVersion.FetchVersionId(processDefinitionType.Name, out _, out var versionId);
        ProjectDefinition.Project.AddProcessDefinition(processDefinitions, versionId);
        return process;
    }
}
