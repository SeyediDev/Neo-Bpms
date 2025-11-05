using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class SendTaskDefinition : TaskDefinition<SendTask>
{
    protected override SendTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new SendTask(flowElementsContainer, ElementId, Name);
    }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the send task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="messageRef">The message reference.</param>
    /// <returns></returns>
    protected SendTask AddSendTask(string actionId, string name, Message messageRef, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var task = new SendTask(process, actionId, name);
        AddTask(_currentLane, outputStateId, task);
        task.messageRef = messageRef;
        return task;
    }

    /// <summary>
    /// Adds the send task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="operationRef">The operation reference.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected SendTask AddSendTask(string actionId, string name, Operation operationRef, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var task = new SendTask(process, actionId, name);
        AddTask(_currentLane, outputStateId, task);
        task.operationRef = operationRef;
        return task;
    }
}
