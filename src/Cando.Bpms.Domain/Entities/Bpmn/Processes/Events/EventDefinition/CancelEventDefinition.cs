using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// Catch only in boundary interrupting
/// Throw only in end
/// </summary>
public class CancelEventDefinition(BpmnDefinitions parent, string id) : EventDefinition(parent, id, Event.eEventType.Cancel), ICatchEventDefinition, IThrowEventDefinition
{
}