using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.GlobalTasks;

/// <summary>
/// A Global Task is a reusable, atomic Task definition that can be called from within any Process by a Call Activity.
/// 
/// There are different types of Tasks identified within BPMN to separate the types of inherent behavior that Tasks might represent. 
/// The types of Global Tasks are only a subset of standard Tasks types. Only GlobalUserTask, GlobalManualTask, GlobalScriptTask, and 
/// GlobalBusinessRuleTask are defined in BPMN.
/// </summary>
public class GlobalTask(BpmnDefinitions parent, string id, string name, GlobalTask.eGlobalTaskType taskType) : CallableElement(parent, id, name)
{
    public List<ResourceRole> resources = [];
    public eGlobalTaskType taskType = taskType;
    public enum eGlobalTaskType
    {
        //			Service,
        Script,
        BusinessRule,
        User,
        Manual,
    };
}