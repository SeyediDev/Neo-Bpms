namespace Neo.Bpms.Domain.Entities.Bpmn.Collaborations.Conversations;

/// <remarks>
/// Correlations are the mechanism that is used to assign the Messages to the proper Process instance, and can be defined
/// for the Message Flows that belong to the Conversation. Correlations can be used to specify Conversations between
/// Processes that follow a fairly simple Conversation pattern in the sense that:
/// • The conceptual data of the Conversation is well known and defined by the participating Processes. However this
/// doesn’t mandate that underlying type systems are identical. It is sufficient that the data is known “conceptually” on a
/// (potentially very high) business level.
/// • A Conversation takes place by means of simple Message exchange between Processes, no additional
/// agreements MUST be considered.
/// • There exists send and receive Tasks accepting the conceptual data of the Conversation. (An Order send by a Task
/// of a Process should be received by at least one Task of the participating Process).
/// • The correlation itself is defined in terms of correlation fields, which denote a subset of the conceptual data that should
/// be used for the correlation. (For example, if the conceptual data comprises an order, then the correlation field might be
/// denoted by the order ID).
/// 
/// In some applications it is useful to allow more Messages to be sent between Participants when a Collaboration is
/// carried out than are contained in the Collaboration model. This enables Participants to exchange other Messages as
/// needed without changing the Collaboration. If the isClosed attribute of a Collaboration has a value of false or no
/// value, then Participants MAY send Messages to each other without additional Message Flows in the Collaboration.
/// If the isClosed attribute of a Collaboration has a value of true, then Participants MAY NOT send Messages to each
/// other without additional Message Flows in the Collaboration. If a Collaboration contains a Choreography, then
/// the value of the isClosed attribute MUST be the same in both. Restrictions on unmodeled messaging specified with
/// isClosed apply only under the Collaboration containing the restriction. PartnerEntities and PartnerRoles
/// of the Participants MAY send Messages to each other under other Choreographies, Collaborations, and Conversations.
/// </remarks>
//class Corrolations
//{
//}
