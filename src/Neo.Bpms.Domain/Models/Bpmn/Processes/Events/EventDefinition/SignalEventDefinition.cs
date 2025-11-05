using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// Catch in all events
/// Throw in all events
/// </summary>
public class SignalEventDefinition(BpmnDefinitions parent, string id, Signal signalRef) : EventDefinition(parent, id, Event.eEventType.Signal), ICatchEventDefinition, IThrowEventDefinition
{
    public Signal signalRef = signalRef;
    public override string Code => signalRef?.Name ?? base.Code;
}
