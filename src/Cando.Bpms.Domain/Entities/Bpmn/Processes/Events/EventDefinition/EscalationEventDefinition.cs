using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

/// <summary>
/// Catch only in start in event sub process, boundary
/// Throw in all (end, intermediate)
/// </summary>
public class EscalationEventDefinition : EventDefinition, ICatchEventDefinition, IThrowEventDefinition
{
    public EscalationEventDefinition(BpmnDefinitions parent, string id) : base(parent, id,
        Event.eEventType.Escalation)
    {
    }

    public EscalationEventDefinition(BpmnDefinitions parent, string id, Escalation escalationRef) : base(parent, id,
        Event.eEventType.Escalation)
    {
        this.escalationRef = escalationRef;
    }

    public Escalation escalationRef;
    public override string Code => escalationRef?.escalationCode ?? base.Code;
}
