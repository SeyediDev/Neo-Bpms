namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
/// <summary>
/// The Event-Based Gateway represents a branching point in the Process where the alternative paths that follow the
/// Gateway are based on Events that occur, rather than the evaluation of Expressions using Process data (as with anExclusive or Inclusive Gateway). 
/// A specific Event, usually the receipt of a Message, determines the path that will be taken. 
/// Basically, the decision is made by another Participant, based on data that is not visible to Process, thus, requiring the use of the Event-Based Gateway.
/// 
/// For example, if a company is waiting for a response from a customer they will perform one set of Activities if the customer responds “Yes” 
/// and another set of Activities if the customer responds “No.” The customer’s response determines which path is taken. The identity of the Message 
/// determines which path is taken. That is, the “Yes” Message and the “No” Message are different Messages—i.e., 
/// they are not the same Message with different values within a property of the Message. The receipt of the Message can be modeled with an Intermediate 
/// Event with a Message trigger or a Receive Task. In addition to Messages, other triggers for Intermediate Events can be used, such as Timers.
/// </summary>
public class EventBasedGateway : Gateway
{
    public bool instantiate;
    public eEventBasedGatewayType eventGatewayType;

    public EventBasedGateway(IFlowElementsContainer flowElementsContainer,
        string id, string name, bool instantiate, eEventBasedGatewayType eventGatewayType) :
        base(flowElementsContainer, id, name, eGatewayType.EventBased)
    {
        if (eventGatewayType == eEventBasedGatewayType.Parallel)
            instantiate = true;
        this.instantiate = instantiate;
        this.eventGatewayType = eventGatewayType;
    }

    public enum eEventBasedGatewayType
    {
        Parallel,
        Exclusive
    }
}