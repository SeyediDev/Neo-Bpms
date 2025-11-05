using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.CallActivity;

namespace Neo.Bpms.Domain.Models.Bpmn.Choreographies;

public class CallChoreography(Choreography choreography, string id, string name, CallableElement calledChoreographyRef) : ChoreographyActivity(choreography, id, name), IParticipantAssociationContainer
{
    /// <summary>
    /// The element to be called, which will be either a Choreography or a GlobalChoreographyTask.
    /// </summary>
    public CallableElement calledChoreographyRef = calledChoreographyRef;

    /// <summary>
    /// Specifies how Participants in a nested Choreography or GlobalChoreographyTask match up with the Participants in the Choreography referenced by the Call Choreography.
    /// </summary>
    public List<ParticipantAssociation> participantAssociations { get; set; }
}