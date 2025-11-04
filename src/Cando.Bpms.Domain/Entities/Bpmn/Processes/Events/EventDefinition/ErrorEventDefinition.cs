using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

/// <summary>
/// Catch only in start in event sub process(interrupting), boundary(interrupting)
/// Throw only in end
/// </summary>
public class ErrorEventDefinition : EventDefinition, ICatchEventDefinition, IThrowEventDefinition
{
    public ErrorEventDefinition(BpmnDefinitions parent, string id)
        : base(parent, id, Event.eEventType.Error)
    {
    }

    public ErrorEventDefinition(BpmnDefinitions parent, string id, Error errorRef)
        : base(parent, id, Event.eEventType.Error)
    {
        error = errorRef;
    }

    public Error error;
    public override string Code => error?.errorCode ?? base.Code;
}
