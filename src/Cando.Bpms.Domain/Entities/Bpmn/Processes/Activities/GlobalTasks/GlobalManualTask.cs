using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.GlobalTasks;

public class GlobalManualTask(BpmnDefinitions parent, string id, string name) : GlobalTask(parent, id, name, eGlobalTaskType.Manual)
{
    public bool bControlBySystem;
}