using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Model.BPMN.Processes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;

/// <summary>
/// A Participant represents a specific PartnerEntity (e.g., a company) and/or a more general PartnerRole (e.g., a buyer, seller, or manufacturer) 
/// that are Participants in a Collaboration. 
/// A Participant is often responsible for the execution of the Process enclosed in a Pool; however, a Pool MAY be defined without a Process.
/// </summary>
public class Participant(BaseElement parent, string id, string name) : BaseElement(parent, id, name)
{
    ///// <summary>
    ///// The name of the Participant can be displayed directly or it can be substituted by the associated PartnerRole or PartnerEntity. 
    ///// Potentially, both the PartnerEntity name and PartnerRole name can be displayed for the Participant.
    ///// </summary>
    //		public string name;
    /// <summary>
    /// identifies the Process that the Participant uses in the Collaboration
    /// </summary>
    public Process processRef;

    /// <summary>
    /// identifies a PartnerRole that the Participant plays in the Collaboration. 
    /// Both a PartnerRole and a PartnerEntity MAY be defined for the Participant
    /// </summary>
    public List<PartnerRole> partnerRoleRef;

    /// <summary>
    /// identifies a PartnerEntity that the Participant plays in the Collaboration. 
    /// Both a PartnerRole and a PartnerEntity MAY be defined for the Participant
    /// </summary>
    public List<PartnerEntity> partnerEntityRef;

    public List<Interface> interfaceRef;

    /// <summary>
    /// is used to define Participants that represent more than one (1) instance of the Participant for a given interaction
    /// </summary>
    public ParticipantMultiplicity participantMultiplicity;

    /// <summary>
    /// This attribute is used to specify the address (or endpoint reference) of concrete services realizing the Participant.
    /// </summary>
    public List<EndPoint> endPointRefs;
}
