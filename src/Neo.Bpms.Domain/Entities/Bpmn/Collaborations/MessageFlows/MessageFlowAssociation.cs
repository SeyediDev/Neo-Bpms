using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.MessageFlows;

/// <summary>
/// These elements are used to do mapping between two elements that both contain Message Flows. 
/// The MessageFlowAssociation provides the mechanism to match up the Message Flows
/// 
/// A MessageFlowAssociation is used when an (outer) diagram with Message Flows contains an (inner) diagram
/// that also has Message Flows. It is used when:
/// • A Collaboration references a Choreography for inclusion between the Collaboration’s Pools (Participants).
/// The Message Flows of the Choreography (the inner diagram) need to be mapped to the Message Flows of the
/// Collaboration (the outer diagram).
/// • A Collaboration references a Conversation that contains Message Flows. The Message Flows of the
/// Conversation can serve as a partial requirement for the Collaboration. Thus, the Message Flows of the
/// Conversation (the inner diagram) need to be mapped to the Message Flows of the Collaboration (the outer diagram).
/// • A Choreography references a Conversation that contains Message Flows. The Message Flows of the
/// Conversation can serve as a partial requirement for the Choreography. Thus, the Message Flows of the
/// Conversation (the inner diagram) need to be mapped to the Message Flows of the Choreography (the outer diagram).
/// </summary>
public class MessageFlowAssociation(Collaboration collaboration, string id, MessageFlow innerMessageFlowRef, MessageFlow outerMessageFlowRef) : BaseElement(collaboration, id)
{
    public MessageFlow innerMessageFlowRef = innerMessageFlowRef;
    public MessageFlow outerMessageFlowRef = outerMessageFlowRef;
}
