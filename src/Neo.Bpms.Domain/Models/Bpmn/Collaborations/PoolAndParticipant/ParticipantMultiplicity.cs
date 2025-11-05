namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.PoolAndParticipant;

/// <summary>
/// ParticipantMultiplicity is used to define the multiplicity of a Participant
/// 
/// The multi-instance marker will be displayed in bottom center of the Pool, or the
/// Participant Band of a Choreography Activity (see page 321), when the ParticipantMultiplicity is
/// associated with the Participant, and the maximum attribute is either not set, or has a value of two or more.
/// </summary>
public class ParticipantMultiplicity(int minimum, int maximum)
{
    /// <summary>
    /// defines minimum number of Participants that MUST be involved in the Collaboration
    /// </summary>
    public int minimum = minimum;
    /// <summary>
    /// maximum number of Participants that MAY be involved in the Collaboration. 
    /// The value of maximum MUST be one or greater, AND MUST be equal or greater than the minimum value
    /// If the value of maximum be zero it means that it is not set(has not the maximum)
    /// </summary>
    public int maximum = maximum;
    public bool hasMaximum() { return maximum <= 0; }

    public ParticipantMultiplicity(int minimum) :
        this(minimum, 0)
    { }
}
