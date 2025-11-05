using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    protected IntermediateThrowEvent currentIntermediateThrowEvent;
    /// <summary>
    /// Adds the intermediate throw.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="eventDefinitions">The event definitions</param>
    /// <returns></returns>
    protected ThrowEvent AddIntermediateThrow(string actionId, string name, object outputStateId,
        params EventDefinition[] eventDefinitions)
    {
        if (definitions == null || process == null) return null;

        currentIntermediateThrowEvent = new IntermediateThrowEvent(process, actionId, name);
        return AddThrowEvent(_currentLane, outputStateId, currentIntermediateThrowEvent, eventDefinitions);
    }

    /// <summary>
    /// Adds the end.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected EndEvent AddEnd(string actionId, string name, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var ev = new EndEvent(process, actionId, name);
        AddThrowEvent(_currentLane, outputStateId, ev);
        return ev;
    }

    /// <summary>
    /// Adds the end.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="eventDefinitions">The event definitions.</param>
    /// <returns></returns>
    protected EndEvent AddEnd(string actionId, string name, object outputStateId,
        params EventDefinition[] eventDefinitions)
    {
        if (definitions == null || process == null) return null;
        var ev = new EndEvent(process, actionId, name);
        AddThrowEvent(_currentLane, outputStateId, ev, eventDefinitions);
        return ev;
    }

    /// <summary>
    /// Adds the throw event.
    /// </summary>
    /// <param name="lane">The lane.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="throwEvent">The _event.</param>
    /// <param name="eventDefinitions">The event definitions.</param>
    /// <returns></returns>
    private ThrowEvent AddThrowEvent(Lane lane, object outputStateId, ThrowEvent throwEvent,
        params EventDefinition[] eventDefinitions)
    {
        _currentEvent = throwEvent;
        currentBaseElement = throwEvent;
        AddFlowNode(throwEvent, lane, outputStateId);
        if (eventDefinitions != null && eventDefinitions.Length > 0)
            AddEventDefinition(eventDefinitions);
        return throwEvent;
    }
}