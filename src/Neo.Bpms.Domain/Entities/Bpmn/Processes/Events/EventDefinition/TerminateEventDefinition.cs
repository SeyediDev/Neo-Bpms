using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

/// <summary>
/// Throw only in end
/// </summary>
public class TerminateEventDefinition(BpmnDefinitions parent, string id) : EventDefinition(parent, id,
    Event.eEventType.Terminate), IThrowEventDefinition
{
}
