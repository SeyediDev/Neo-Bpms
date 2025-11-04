using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.MessageFlows;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.Conversations;

/// <summary>
/// Conversation Links are used to connect ConversationNodes to and from Participants
/// Conversation Links MUST be drawn with double thin lines
/// Processes can appear in the Participants (Pools) of Conversation diagrams
/// 
/// Conversation Links for Call Conversations show the names of Participants in nested Collaboration or global Collaborations, as identified by ParticipantAssociations
/// </summary>
public class ConversationLink(Collaboration collaboration, string id, string name, InteractionNode sourceRef, InteractionNode targetRef) : BaseElement(collaboration, id, name)
{
    //		public string name;
    /// <summary>
    /// The InteractionNode that the Conversation Link is connecting from. 
    /// A Conversation Link MUST connect to exactly one ConversationNode. 
    /// If the sourceRef is not a ConversationNode, then the targetRef MUST be a ConversationNode
    /// </summary>
    public InteractionNode sourceRef = sourceRef;

    /// <summary>
    /// The InteractionNode that the Conversation Link is connecting to. 
    /// A Conversation Link MUST connect to exactly one ConversationNode. If the targetRef is not a ConversationNode, 
    /// then the sourceRef MUST be a ConversationNode
    /// </summary>
    public InteractionNode targetRef = targetRef;
}