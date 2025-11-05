using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract class BusinessRuleTaskDefinition : TaskDefinition<BusinessRuleTask>
{
    protected sealed override BusinessRuleTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new BusinessRuleTask(flowElementsContainer, ElementId, Name);
    }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the business rule task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected BusinessRuleTask AddBusinessRuleTask(string actionId, string name, object outputStateId = null)
    //, params Decision[] decisions
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        BusinessRuleTask task = new(process, actionId, name);
        //foreach (var decision in decisions)
        //{
        //	task.addDecision(decision);
        //}
        AddTask(_currentLane, outputStateId, task);
        return task;
    }
}
