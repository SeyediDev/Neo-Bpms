using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Model.BPMN.Processes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;

/// <summary>
/// "Context-based Correlation:
/// Context-based correlation is a more expressive form of correlation on top of key-based correlation. 
/// In addition to implicitly populating the CorrelationKey instance from the first sent or received Message, 
/// another mechanism relates the CorrelationKey to the Process context. 
/// That is, a Process MAY provide a CorrelationSubscription that acts as the Process-specific counterpart to a specific CorrelationKey. 
/// In this way, a Conversation MAY additionally refer to explicitly updateable Process context data to determine whether or not a Message needs to be received. 
/// At runtime, the CorrelationKey instance holds a composite key that is dynamically calculated from the Process context and automatically updated whenever the underlying Data Objects or Properties change"
/// </summary>
public class CorrelationSubscription(Process process, string id, CorrelationKey correlationKey) : BaseElement(process, id)
{
    public CorrelationKey correlationKeyRef = correlationKey;

    /// <summary>
    /// The bindings to specific CorrelationProperties and FormalExpressions (extraction rules atop the Process context).
    /// </summary>
    public List<CorrelationPropertyBinding> correlationPropertyBinding;
}
