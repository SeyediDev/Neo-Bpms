namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;

public class CorrelationPropertyRetrievalExpression(CorrelationProperty correlationProperty, string id,
    FormalExpression messagePath, Message messageRef) : BaseElement(correlationProperty, id)
{
    /// <summary>
    /// The FormalExpression that defines how to extract a CorrelationProperty from the Message payload.
    /// </summary>
    public FormalExpression messagePath = messagePath;

    public Message messageRef = messageRef;
}
