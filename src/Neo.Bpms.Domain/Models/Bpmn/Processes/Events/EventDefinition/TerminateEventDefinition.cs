using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;

/// <summary>
/// Throw only in end
/// </summary>
public class TerminateEventDefinition(BpmnDefinitions parent, string id) : EventDefinition(parent, id,
    Event.eEventType.Terminate), IThrowEventDefinition
{
}
