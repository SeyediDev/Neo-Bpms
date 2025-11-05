using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;

/// <summary>
/// A PartnerEntity is one of the possible types of Participant
/// </summary>
public class PartnerEntity(BpmnDefinitions parent, string id, string name) : RootElement(parent, id, name)
{
    //		public string name;
    public List<Participant> participantRef;

    public override void Copy(RootElement newRootElement)
    {
        PartnerEntity newItem = newRootElement as PartnerEntity;
        if (newItem == null) return;
        // todo
    }
}