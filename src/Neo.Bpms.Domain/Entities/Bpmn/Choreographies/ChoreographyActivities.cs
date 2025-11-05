using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Choreographies;

public class ChoreographyActivity(Choreography choreography, string id, string name) : FlowNode(choreography, id, name, eFlowNodeType.ChoreographyActivity), ICorrelationKeyContainer
{
    /// <summary>
    /// A Choreography Activity has two or more Participants
    /// </summary>
    public List<Participant> participantRefs;

    /// <summary>
    /// One of the Participants will be the one that initiates the Choreography Activity.
    /// </summary>
    public Participant initiatingParticipantRef;

    /// <summary>
    /// A Choreography Activity MAY be performed once or MAY be repeated. The loopType attribute will determine the appropriate marker for the Choreography Activity
    /// </summary>
    public ChoreographyLoopType loopType;

    /// <summary>
    /// This association specifies correlationKeys used by the Message Flow in the Choreography Activity, including Sub-Choreographies and called Choreographies
    /// </summary>
    public List<CorrelationKey> correlationKeys { get; set; }

    public enum ChoreographyLoopType
    {
        None,
        Standard,
        MultiInstanceSequential,
        MultiInstanceParallel
    }
}
