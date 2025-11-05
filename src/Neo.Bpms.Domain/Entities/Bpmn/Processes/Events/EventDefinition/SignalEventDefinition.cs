using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// Catch in all events
/// Throw in all events
/// </summary>
public class SignalEventDefinition(BpmnDefinitions parent, string id, Signal signalRef) : EventDefinition(parent, id, Event.eEventType.Signal), ICatchEventDefinition, IThrowEventDefinition
{
    public Signal signalRef = signalRef;
    public override string Code => signalRef?.Name ?? base.Code;
}