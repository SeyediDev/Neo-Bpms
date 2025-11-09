using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class ReceiveTaskDefinition : TaskDefinition<ReceiveTask>
{
    protected sealed override ReceiveTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new ReceiveTask(flowElementsContainer, ElementId, Name);
    }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the receive task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="messageRef">The message reference.</param>
    /// <param name="instantiate">if set to <c>true</c> [instantiate].</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected ReceiveTask AddReceiveTask(string actionId, string name, Message messageRef,
        bool instantiate, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var receiveTask = new ReceiveTask(process, actionId, name);
        AddTask(_currentLane, outputStateId, receiveTask);
        receiveTask.messageRef = messageRef;
        receiveTask.instantiate = instantiate;
        return receiveTask;
    }

    /// <summary>
    /// Adds the receive task.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="operationRef">The operation reference.</param>
    /// <param name="instantiate">if set to <c>true</c> [instantiate].</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected ReceiveTask AddReceiveTask(string actionId, string name, Operation operationRef,
        bool instantiate, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var receiveTask = new ReceiveTask(process, actionId, name);
        AddTask(_currentLane, outputStateId, receiveTask);
        receiveTask.operationRef = operationRef;
        receiveTask.instantiate = instantiate;
        return receiveTask;
    }
}
