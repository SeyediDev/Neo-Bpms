namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.Conversations;

/// <summary>
/// A Sub-Conversation is a ConversationNode that is a hierarchical division within the parent Collaboration. 
/// A Sub-Conversation is a graphical object within a Collaboration, but it also can be “opened up” to show the lowerlevel
/// details of the Conversation, which consist of Message Flows, Conversations, and/or other Sub-Conversations. 
/// The Sub-Conversation shares the Participants of its parent Conversation.
/// </summary>
public class SubConversation(Collaboration parent, string id, string name) : ConversationNode(parent, id, name)
{
    /// <summary>
    /// The ConversationNodes model aggregation relationship allows a Sub-Conversation to contain other ConversationNodes, 
    /// in order to group Message Flows of the Sub-Conversation and associate correlation information.
    /// </summary>
    public List<ConversationNode> conversationNodes = [];
}
