using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;

/// <summary>
/// represents a composite key out of one or many CorrelationProperties that essentially specify extraction Expressions atop Messages
/// </summary>
/// <value>
/// Correlation facilitates the association of a Message to a Send Task or Receive Task1 often in the context of a Conversation, which is also known as instance routing.
/// Correlations describe a set of predicates on a Message (generally on the application payload) that need to be satisfied in order for that Message to be associated to a distinct Send Task or Receive Task.
/// By the same token, each Send Task and each Receive Task participates in one or many Conversations.
/// 
/// plain, key-based correlation:
/// Messages that are exchanged within a Conversation are logically correlated by means of one or more common CorrelationKeys. That is, any Message that is sent or received within this Conversation needs to carry the value of at least one of these CorrelationKey instances within its payload
/// 
/// context-based correlation:
/// The Process context (i.e., its Data Objects and Properties) can dynamically influence the matching criterion. a CorrelationKey can be complemented by a Process-specific CorrelationSubscription
/// 
/// </value>
/// <example>
/// At runtime, the correlation mechanism works as follows: When a Process instance is created the CorrelationKey
/// instances of all Conversations are initialized with some initial values that specify to correlate any incoming Message
/// for these Conversations. A SubscriptionProperty is updated whenever any of the Data Objects or
/// Properties changes that are referenced from the respective FormalExpression. As a result, incoming Messages
/// are matched against the now populated CorrelationKey instance. Later in the Process run, the
/// SubscriptionProperties can, again, change and implicitly change the correlation criterion. Alternatively, the
/// established mechanism of having the first Send Task or Receive Task populate the CorrelationKey instance applies.
/// </example>
public class CorrelationKey(ICorrelationKeyContainer correlationKeyContainer, string id, string name) : BaseElement(correlationKeyContainer as BaseElement, id, name)
{
    //public string name;

    /// <summary>
    /// representing the partial keys of this CorrelationKey.
    /// </summary>
    public List<CorrelationProperty> correlationPropertyRef;
}

public interface ICorrelationKeyContainer
{
    List<CorrelationKey> correlationKeys { get; set; }
}
