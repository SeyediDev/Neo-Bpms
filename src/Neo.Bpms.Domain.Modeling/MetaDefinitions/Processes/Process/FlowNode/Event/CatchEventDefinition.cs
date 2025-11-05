using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the start.
    /// </summary>
    /// <param name="id">The element identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="eventType">Type of the event.</param>
    /// <returns></returns>
    protected StartEvent AddStart(string id, string name, object outputStateId,
        Event.eEventType eventType)
    {
        if (definitions == null || process == null) return null;
        if (eventType != Event.eEventType.None)
            throw new Exception(eventType + " event must be declared");
        var ev = new StartEvent(process, id, name);
        AddCatchEvent(_currentLane, outputStateId, ev);
        return ev;
    }

    /// <summary>
    /// Adds the start.
    /// </summary>
    /// <param name="id">The element identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="eventDefinitions">The event definitions.</param>
    /// <returns></returns>
    protected StartEvent AddStart(string id, string name, object outputStateId = null,
        params EventDefinition[] eventDefinitions)
    {
        if (definitions == null || process == null) return null;

        var ev = new StartEvent(process, id, name);
        AddCatchEvent(_currentLane, outputStateId, ev, eventDefinitions);
        return ev;
    }

    /// <summary>
    /// cancelActivity is same as interrupting
    /// </summary>
    /// <param name="id">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="attachedToRef">The main element identifier.</param>
    /// <param name="cancelActivity">if set to <c>true</c> [cancel activity].</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="eventDefinitions">The event definitions</param>
    /// <returns></returns>
    protected BoundaryEvent AddBoundary(string id, string name, string attachedToRef,
        bool cancelActivity, object outputStateId, params EventDefinition[] eventDefinitions)
    {
        if (definitions == null || process == null) return null;
        if (string.IsNullOrEmpty(attachedToRef))
            throw new Exception("BoundaryEvent must define attached To Reference.");
        var ev = new BoundaryEvent(process, id, name, attachedToRef, cancelActivity);
        AddCatchEvent(_currentLane, outputStateId, ev, eventDefinitions);
        return ev;
    }

    protected IntermediateCatchEvent currentIntermediateCatchEvent;
    /// <summary>
    /// Adds the intermediate catch.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="eventDefinitions">The event definitions</param>
    /// <returns></returns>
    protected IntermediateCatchEvent AddIntermediateCatch(string actionId, string name, object outputStateId,
        params EventDefinition[] eventDefinitions)
    {
        if (definitions == null || process == null) return null;
        currentIntermediateCatchEvent = new IntermediateCatchEvent(process, actionId, name);
        AddCatchEvent(_currentLane, outputStateId, currentIntermediateCatchEvent, eventDefinitions);
        return currentIntermediateCatchEvent;
    }

    /// <summary>
    /// Adds the catch event.
    /// </summary>
    /// <param name="lane">The lane.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="catchEvent">The _event.</param>
    /// <param name="eventDefinitions">The event definitions.</param>
    /// <returns></returns>
    private void AddCatchEvent(Lane lane, object outputStateId, CatchEvent catchEvent,
        params EventDefinition[] eventDefinitions)
    {
        _currentEvent = catchEvent;
        currentBaseElement = catchEvent;
        AddFlowNode(catchEvent, lane, outputStateId);
        if (eventDefinitions != null && eventDefinitions.Length > 0)
            AddEventDefinition(eventDefinitions);
    }
}