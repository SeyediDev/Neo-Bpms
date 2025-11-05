using Neo.Bpms.Domain.Entities.Bpmn.Choreographies;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.Conversations;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.MessageFlows;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations;

/// <remarks>
/// To handle Message Flows, the innerMessageFlow of a MessageFlowAssociation refers to a Message Flow
/// in the Choreography, while the outerMessageFlow refers to a Message Flow in the Collaboration containing
/// the Choreography. This mapping matches the Message Flows of the Choreography (which are not visible) to the
/// Message Flows in the Collaboration (which are visible). This allows the Message Flows of the Collaboration to
/// be “wired up” through the appropriate Choreography Activity in the Choreography
/// 
/// The ParticipantAssociations might be derived from the partnerEntities or partnerRoles of the
/// Participants. For example, if a Choreography Activity has a Participant with the same partnerEntity as a
/// Participant in the Collaboration containing the Choreography, then these two Participants could be assumed to be
/// the inner and outerParticipants of a ParticipantAssociation. Similarly, Message Flows that reference
/// the same Message in a Call Choreography Activity and the Collaboration, could be automatically synchronized by
/// a MessageFlowAssociation, if only one Message Flow has that Message.
/// </remarks>
/// <summary>
/// The Collaboration package contains classes that are used for modeling Collaborations, which is a collection of
/// Participants shown as Pools, their interactions as shown by Message Flows, and MAY include Processes within the
/// Pools and/or Choreographies between the Pools (see Figure 9.1). A Choreography is an extended type of Collaboration. 
/// When a Collaboration is defined it is contained in Definitions
/// </summary> 
public class Collaboration(BpmnDefinitions parent, string id, string name) : RootElement(parent, id, name), IArtifactContainer, ICorrelationKeyContainer, IParticipantAssociationContainer
{
    //		public string name;

    /// <summary>
    /// The choreographyRef model association defines the Choreographies that can be shown between the Pools of the Collaboration. 
    /// A Choreography specifies a business contract (or the order in which messages will be exchanged) between interacting Participants.
    /// The participantAssociations (see below) are used to map the Participants of the Choreography to the Participants of the Collaboration.
    /// The MessageFlowAssociations (see below) are used to map the Message Flows of the Choreography to the Message Flows of the Collaboration.
    /// The conversationAssociations (see below) are used to map the Conversations of the Choreography to the Conversations of the Collaboration.
    /// Note that this attribute is not applicable for Choreography or GlobalConversation which are a subtypes of Collaboration. 
    /// Thus, a Choreography cannot reference another Choreography.
    /// </summary>
    public List<Choreography> choreographyRef;

    /// <summary>
    /// This association specifies CorrelationKeys used to associate Messages to a particular Collaboration.
    /// </summary>
    public List<CorrelationKey> correlationKeys { get; set; }

    /// <summary>
    /// This attribute provides a list of mappings from the Conversations of a 
    /// referenced Collaboration to the Conversations of another Collaboration.
    /// It is used when:
    /// • When a Choreography is referenced by a Collaboration.
    /// </summary>
    public List<ConversationAssociation> conversationAssociations;

    /// <summary>
    /// The conversations model aggregation relationship allows a Collaboration to contain Conversation elements, 
    /// in order to group Message Flows of the Collaboration and associate correlation information,
    /// as is REQUIRED for the definitional Collaboration of a Process model. 
    /// The Conversation elements will be visualized if the Collaboration is a Collaboration, 
    /// but not for a Choreography
    /// </summary>
    public List<ConversationNode> conversations;

    /// <summary>
    /// This provides the Conversation Links that are used in the Collaboration.
    /// </summary>
    public List<ConversationLink> conversationLinks;

    /// <summary>
    /// This attribute provides the list of Artifacts that are contained within the Collaboration.
    /// </summary>
    public List<Artifact> artifacts { get; set; }

    /// <summary>
    /// This provides the list of Participants that are used in the Collaboration.
    /// Participants are visualized as Pools in a Collaboration and as Participant
    /// Bands in Choreography Activities in a Choreography.
    /// </summary>
    public List<Participant> participants;

    /// <summary>
    /// This attribute provides a list of mappings from the Participants of a referenced Collaboration to the Participants of another Collaboration. 
    /// It is used in the following situations
    /// • When a Choreography is referenced by the Collaboration.
    /// • When a definitional Collaboration for a Process is referenced through a Call Activity (and mapped to definitional Collaboration of the calling Process).
    /// </summary>
    public List<ParticipantAssociation> participantAssociations { get; set; }

    /// <summary>
    /// This provides the list of Message Flows that are used in the Collaboration.
    /// Message Flows are visualized in Collaboration (as dashed line) and hidden in Choreography
    /// </summary>
    public List<MessageFlow> messageFlow;

    /// <summary>
    /// This attribute provides a list of mappings for the Message Flows of the Collaboration to Message Flows of a referenced model. 
    /// It is used in the following situation:
    /// • When a Choreography is referenced by a Collaboration. 
    /// This allows the "wiring up" of the Collaboration Message Flows to the appropriate Choreography Activities.
    /// </summary>
    public List<MessageFlowAssociation> messageFlowAssociations;

    /// <summary>
    /// A boolean value specifying whether Message Flows not modeled in the
    /// Collaboration can occur when the Collaboration is carried out.
    /// • If the value is true, they MAY NOT occur.
    /// • If the value is false, they MAY occur.
    /// </summary>
    public bool isClosed;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Collaboration newItem) return;
        Name = newItem.Name;
        CloneBase(newRootElement);
    }
}
