using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract class ServiceTaskDefinition : TaskDefinition<ServiceTask>
{
    protected override ServiceTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new ServiceTask(flowElementsContainer, ElementId, Name, Operation);
    }

    public Operation Operation { get; set; }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the service task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="operation">The operation.</param>
    /// <returns></returns>
    protected ServiceTask AddServiceTask(string actionId, string name, object outputStateId, Operation operation)
    {
        if (definitions == null || process == null) return null;
        var serviceTask = new ServiceTask(process, actionId, name, operation);
        AddTask(_currentLane, outputStateId, serviceTask);
        return serviceTask;
    }
}
