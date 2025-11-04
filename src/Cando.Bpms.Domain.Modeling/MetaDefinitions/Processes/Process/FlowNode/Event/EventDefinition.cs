using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    private Event _currentEvent;

    /// <summary>
    /// Adds the event definition.
    /// </summary>
    /// <param name="eventDefinitions">The event definitions.</param>
    /// <returns></returns>
    protected bool AddEventDefinition(params EventDefinition[] eventDefinitions)
    {
        if (_currentEvent == null) return false;
        _currentEvent.AddEventDefinitions(eventDefinitions);
        return true;
    }

    ///// <summary>
    ///// Adds the event definition event.
    ///// </summary>
    ///// <param name="eventDefinitions">The event definitions.</param>
    ///// <returns></returns>
    //protected bool AddEventDefinition(params string[] eventDefinitions)
    //{
    //	if (_currentEvent == null) return false;
    //	// _currentEvent.addEvent(eventDefinitions);
    //	return true;
    //}
}
