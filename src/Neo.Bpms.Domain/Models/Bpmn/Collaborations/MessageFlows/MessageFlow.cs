using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.MessageFlows;

/// <summary>
/// A Message Flow is used to show the flow of Messages between two Participants that are prepared to send and receive them.
/// 
/// A Message Flow MUST connect two separate Pools. They connect either to the Pool boundary or to Flow Objects within the Pool boundary. They MUST NOT connect two objects within the same Pool
/// 
/// In Collaboration Diagrams, a Message Flow can be extended to show the Message that is passed from one Participant to another
/// 
/// If a Choreography is included in the Collaboration, then the Message Flow will “pass-through” a Choreography Task as it connects from one Participant to another
/// </summary>
public class MessageFlow(Collaboration collaboration, string id, string name, InteractionNode sourceRef, InteractionNode targetRef, Message messageRef) : BaseElement(collaboration, id, name)
{
    //		public string name;
    /// <summary>
    /// only Pools/Participants, Activities, and Events can be the source of a Message Flow.
    /// </summary>
    public InteractionNode sourceRef = sourceRef;
    /// <summary>
    /// only Pools/Participants, Activities, and Events can be the target of a Message Flow
    /// </summary>
    public InteractionNode targetRef = targetRef;
    /// <summary>
    /// the Message that is passed via the Message Flow
    /// </summary>
    public Message messageRef = messageRef;

    public MessageFlow(Collaboration collaboration, string id, string name, InteractionNode sourceRef, InteractionNode targetRef) :
        this(collaboration, id, name, sourceRef, targetRef, null)
    { }
}
