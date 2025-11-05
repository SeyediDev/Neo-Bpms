using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

/// <summary>
/// Catch in all events
/// Throw in all events
/// </summary>
public class MessageEventDefinition(BpmnDefinitions parent, string id, Message messageRef, Operation operationRef) 
    : EventDefinition(parent, id, Event.eEventType.Message), ICatchEventDefinition, IThrowEventDefinition, IMessageContainer
{

    /// <summary>
    /// The Message MUST be supplied (if the isExecutable attribute of the Process is set to true).
    /// </summary>
    public Message messageRef { get; set; } = messageRef;

    /// <summary>
    /// attribute specifies the Operation that is used by the Message Event. It MUST be specified for executable Processes.
    /// </summary>
    public Operation operationRef { get; set; } = operationRef;

    public override string Code => messageRef?.Name ?? base.Code;
}
