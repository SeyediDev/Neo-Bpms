using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;

public class CorrelationPropertyBinding(CorrelationSubscription correlationSubscription,
    string id, FormalExpression dataPath, CorrelationProperty correlationPropertyRef) : BaseElement(correlationSubscription, id)
{
    /// <summary>
    /// The FormalExpression that defines the extraction rule atop the Process context.
    /// </summary>
    public FormalExpression dataPath = dataPath;

    public CorrelationProperty correlationPropertyRef = correlationPropertyRef;
}
