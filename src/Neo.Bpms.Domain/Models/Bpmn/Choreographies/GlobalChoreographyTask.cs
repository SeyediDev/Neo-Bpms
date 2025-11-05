namespace Neo.Bpms.Domain.Models.Bpmn.Choreographies;

public class GlobalChoreographyTask(BpmnDefinitions parent, string id, string name) : Choreography(parent, id, name)
{
    /// <summary>
    /// One of the Participants will be the one that initiates the Global Choreography Task
    /// </summary>
    public Participant initiatingParticipantRef;
}
