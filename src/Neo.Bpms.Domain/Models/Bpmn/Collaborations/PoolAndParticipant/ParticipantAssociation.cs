namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.PoolAndParticipant;

/// <summary>
/// These elements are used to do mapping between two elements that both contain Participants. 
/// There are situations where the Participants in different diagrams can be defined differently because they were developed independently, 
/// but represent the same thing. The ParticipantAssociation provides the mechanism to match up the Participants.
/// 
/// A ParticipantAssociation is used when an (outer) diagram with Participants contains an (inner) diagram that 
/// also has Participants. There are four usages of ParticipantAssociation. It is used when:
/// 
/// A Collaboration references a Choreography for inclusion between the Collaboration’s Pools (Participants).
/// The Participants of the Choreography (the inner diagram) need to be mapped to the Participants of the
/// Collaboration (the outer diagram)
/// 
/// A Call Conversation references a Collaboration or GlobalConversation. Thus, the Participants of the
/// Collaboration or GlobalConversation (the inner diagram) need to be mapped to the Participants referenced
/// by the Call Conversation (the outer element). Each Call Conversation contains its own set of ParticipantAssociations
/// 
/// A Call Choreography references a Choreography or GlobalChoreographyTask. Thus, the Participants of the Choreography or GlobalChoreographyTask 
/// (the inner diagram) need to be mapped to the Participants referenced by the Call Choreography (the outer element). 
/// Each Call Choreography contains its own set of ParticipantAssociations
/// 
/// A Call Activity within a Process that has a definitional Collaboration references another Process that also
/// has a definitional Collaboration. The Participants of the definitional Collaboration of the called Process (the inner diagram) 
/// need to be mapped to the Participants of the definitional Collaboration of the calling Process (the outer diagram).
/// </summary>
public class ParticipantAssociation(IParticipantAssociationContainer container,
    string id, Participant innerParticipantRef, Participant outerParticipantRef) : BaseElement(container as BaseElement, id)
{
    /// <summary>
    /// the Participant of the referenced element
    /// </summary>
    public Participant innerParticipantRef = innerParticipantRef;
    /// <summary>
    /// the Participant of the parent element
    /// </summary>
    public Participant outerParticipantRef = outerParticipantRef;
}

public interface IParticipantAssociationContainer
{
    List<ParticipantAssociation> participantAssociations { get; set; }
}