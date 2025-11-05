namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.PoolAndParticipant;

/// <summary>
/// A PartnerRole is one of the possible types of Participant
/// </summary>
public class PartnerRole(BpmnDefinitions parent, string id, string name) : RootElement(parent, id, name)
{
    //		public string name;
    public List<Participant> participantRef;

    public override void Copy(RootElement newRootElement)
    {
        PartnerRole newItem = newRootElement as PartnerRole;
        if (newItem == null) return;
        // todo
    }
}