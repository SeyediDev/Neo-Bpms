using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// can catch in all events, not in throw
/// </summary>
public class ConditionalEventDefinition(BpmnDefinitions parent, string id, BpmnExpression condition) : EventDefinition(parent, id, Event.eEventType.Condition), ICatchEventDefinition
{

    /// <summary>
    /// The Expression might be underspecified and provided in the form of natural language. 
    /// For executable Processes (isExecutable = true), if the trigger is Conditional, then a FormalExpression MUST be entered.
    /// </summary>
    public BpmnExpression condition = condition;
}