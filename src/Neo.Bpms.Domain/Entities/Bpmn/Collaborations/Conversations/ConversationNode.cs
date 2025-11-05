using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.MessageFlows;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.Conversations;

/// <remarks>
/// The Conversation diagram is particular usage of and an informal description of a Collaboration diagram. 
/// In general,it is a simplified version of Collaboration, but Conversation diagrams do maintain all the features of a Collaboration. 
/// In particular, Processes can appear within the Participants (Pools) of Conversation diagrams, to show how Conversation and Activities are related.
/// 
/// A Conversation is a logical grouping of Message exchanges (Message Flows) that can share a Correlation. 
/// A Conversation is the logical relation of Message exchanges. The logical relation, in practice, often concerns a business
/// object(s) of interest, e.g, “Order,” “Shipment and Delivery,” and “Invoice.” Hence, a Conversation is associated with a
/// set of name-value pairs, or a Correlation Key (e.g., “Order Identifier,” “Delivery Identifier”), which is recorded in
/// the Messages that are exchanged. In this way, a Message can be routed to the specific Process instance responsible
/// for receiving and processing the Message.
/// 
/// Message exchanges are related to each other and reflect distinct business scenarios. The relation is sometimes simple,
/// e.g., a request followed by a response (and can be described as part of a structural interface of a service, e.g., as a WSDL
/// operation definition). However for commercial business transactions managed through Business Processes, the
/// relation can be complex, involving long-running, reciprocal Message exchanges, and that could extend beyond bilateral
/// to complex, multilateral Collaborations. For example, in logistics, stock replenishments involve the following types
/// scenarios: creation of sales orders; assignment of carriers for shipments combining different sales orders; crossing
/// customs/quarantine; processing payment and investigating exceptions
/// 
/// In addition to an orchestration Process, Conversations are relevant to a Choreography, but the Conversations
/// are not visualized in a Choreography. The difference is that a Choreography provides a multi-party perspective of a
/// Conversation. This is because the Message exchanges modeled using Choreography Activities concern multiple
/// Participants, unlike an orchestration Process where the Message sending and receiving elements relate to one
/// Participant only. Other than the difference in perspective, the notion of Conversation remains the same across
/// Choreography and orchestration - and the Message exchanges of a Conversation will ultimately to be executed
/// through an orchestration Process.
/// 
/// Since Collaboration provides a top-down, design-time modeling perspective for Message exchanges and their
/// Conversations, an abstracted view of the all Conversations pertaining to a domain being modeled is available
/// through a Conversation diagram. A Conversation diagram, as depicted in Figure 9.18, shows Conversations (as
/// hexagons) between Participants. 
/// This provides a “bird’s eye” perspective of the different Conversations which relate to the domain.
/// 
/// a hierarchical structure of Conversations can be seen with one set of Message Flows occurring within another in a parent-child relationship
/// 
/// A common dependency between Conversations is overlap. Overlap occurs when two or more Conversations have
/// some Message exchanges in common but not others.
/// 
/// Splits and joins are special types of overlap scenarios. A Conversation split arises when, as part of a Conversation, a
/// message is exchanged between two or more Participants that at the same time spawns a new, distinct Conversation
/// (either between the same set of Participants or another set). Additionally, no further Message exchanges are shared by
/// the split Conversations as well as no subsequent merges of them occur. An example is Delivery Planning which leads
/// to Carrier Planning and Special Cover. A Conversation join occurs when several Conversations are merged into one
/// Conversation and no further Message exchanges occur in the original Conversations, i.e., these Conversations
/// are finalized. The generalization of a split and join is a Conversation refactor where Conversations are split into
/// parallel Conversations and then are merged at a later point in time.
/// </remarks>
/// <summary>
/// ConversationNode is the abstract super class for all elements that can comprise the Conversation elements of a
/// Collaboration diagram, which are Conversation, Sub-Conversation, and Call Conversation
/// </summary>
public class ConversationNode(Collaboration parent, string id, string name) : BaseElement(parent, id, name), ICorrelationKeyContainer
{
    //		public string name;

    /// <summary>
    /// This provides the list of Participants that are used in the ConversationNode 
    /// from the list provided by the ConversationNode’s parent Conversation. 
    /// This reference is visualized through a Conversation Link
    /// </summary>
    public List<Participant> participantRefs;

    /// <summary>
    /// reference to all Message Flows (and consequently Messages) grouped by a Conversation element.
    /// </summary>
    public List<MessageFlow> messageFlowRefs;

    /// <summary>
    /// list of the ConversationNode’s CorrelationKeys, which are used to group Message Flows for the ConversationNode
    /// </summary>
    public List<CorrelationKey> correlationKeys { get; set; }
}