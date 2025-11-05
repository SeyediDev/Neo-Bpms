namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.Conversations;

/// <summary>
/// A GlobalConversation is a reusable, atomic Conversation definition that can be called from within any Collaboration by a Call Conversation.
/// 
/// Since a GlobalConversation does not have any Flow Elements, it does not require MessageFlowAssociations, 
/// ParticipantAssociations, or ConversationAssociations or Artifacts.
/// It is basically a set of Participants, Message Flows, and CorrelationKeys intended for reuse. Also, the
/// Collaboration attribute choreographyRef is not applicable to GlobalConversation.
/// </summary>
public class GlobalConversation(Collaboration parent, string id, string name) : ConversationNode(parent, id, name)
{
}
