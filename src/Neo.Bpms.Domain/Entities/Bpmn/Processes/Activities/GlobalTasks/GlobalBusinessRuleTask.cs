using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.GlobalTasks;

public class GlobalBusinessRuleTask(BpmnDefinitions parent, string id, string name) : GlobalTask(parent, id, name,
    eGlobalTaskType.BusinessRule)
{
    /// <summary>
    /// This attribute specifies the technology that will be used to implement the Business Rule Task. 
    /// Valid values are "##unspecified" for leaving the implementation technology open, "##WebService" 
    /// for the Web service technology or a URI identifying any other technology or coordination protocol.
    /// The default technology for this task is unspecified.
    /// </summary>
    public string implementation = "##unspecified";

    //public List<Decision> decisions = new List<Decision>();
    //public void addDecision(Decision decision)
    //{
    //	decisions.Add(decision);
    //}
}