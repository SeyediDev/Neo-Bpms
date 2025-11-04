using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;

/// <summary>
/// This element controls when and which Events are thrown in case behavior of the Multi-Instance Activity is set to complex
/// </summary>
public class ComplexBehaviorDefinition(MultiInstanceLoopCharacteristics multiInstanceLoopCharacteristics,
    string id, FormalExpression condition, ImplicitThrowEvent implicitEvent) : BaseElement(multiInstanceLoopCharacteristics, id)
{
    /// <summary>
    /// This attribute defines a boolean Expression that when evaluated to true,cancels the remaining Activity instances and produces a token.
    /// </summary>
    public FormalExpression condition = condition;
    /// <summary>
    /// If the condition is true, this identifies the Event that will be thrown (to be caught 
    /// by a boundary Event on the Multi-Instance Activity).
    /// </summary>
    public ImplicitThrowEvent implicitEvent = implicitEvent;
}
