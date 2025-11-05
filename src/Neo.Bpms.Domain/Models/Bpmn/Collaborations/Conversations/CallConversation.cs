namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.Conversations;

/// <summary>
/// A Call Conversation identifies a place in the Conversation (Collaboration) where a global Conversation or a GlobalConversation is used.
/// The ConversationNode attribute messageFlowRef doesn’t apply to Call Conversations
/// </summary>
public class CallConversation(Collaboration parent, string id, string name) : ConversationNode(parent, id, name), IParticipantAssociationContainer
{
    /// <summary>
    /// The element to be called, which MAY be either a Collaboration or a GlobalConversation. 
    /// The called element MUST NOT be a Choreography or a GlobalChoreographyTask (which are subtypes of Collaboration)
    /// </summary>
    public Collaboration calledCollaborationRef;

    /// <summary>
    /// This attribute provides a list of mappings from the Participants of a referenced GlobalConversation or 
    /// Conversation to the Participants of the parent Conversation
    /// </summary>
    public List<ParticipantAssociation> participantAssociations { get; set; }
}