using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Choreographies;

public class GlobalChoreographyTask(BpmnDefinitions parent, string id, string name) : Choreography(parent, id, name)
{
    /// <summary>
    /// One of the Participants will be the one that initiates the Global Choreography Task
    /// </summary>
    public Participant initiatingParticipantRef;
}
