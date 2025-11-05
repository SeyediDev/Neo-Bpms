namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.GlobalTasks;

public class GlobalManualTask(BpmnDefinitions parent, string id, string name) : GlobalTask(parent, id, name, eGlobalTaskType.Manual)
{
    public bool bControlBySystem;
}