using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.Conversations;

/// <summary>
/// A ConversationAssociation is used within Collaborations and Choreographies to apply a reusable Conversation to the Message Flows of those diagrams.
/// 
/// A ConversationAssociation is used when a diagram references a Conversation to provide Message
/// correlation information and/or to logically group Message Flows. It is used when:
/// • A Collaboration references a Choreography for inclusion between the Collaboration's Pools (Participants).
/// The ConversationNodes of the Choreography (the inner diagram) need to be mapped to the ConversationNodes of the Collaboration (the outer diagram).
/// </summary>
public class ConversationAssociation(Collaboration collaboration, string id) : BaseElement(collaboration, id)
{
    /// <summary>
    /// This attribute defines the ConversationNodes of the referenced element 
    /// (e.g., a Choreography to be used in a Collaboration) that will be mapped to the parent element 
    /// (e.g., the Collaboration).
    /// </summary>
    public ConversationNode innerConversationNodeRef;

    /// <summary>
    /// This attribute defines the ConversationNodes of the parent element 
    /// (e.g., a Collaboration references a Choreography) that will be mapped
    /// to the referenced element (e.g., the Choreography).
    /// </summary>
    public List<ConversationNode> outerConversationNodeRef;
}