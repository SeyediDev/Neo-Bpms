namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;

// we can not use from this object, because c# does not support multiple inheritance, so we changed its inheritance

/// <summary>
/// Gateways are used to control how the Process flows (how Tokens flow) through Sequence Flows as they converge and diverge within a Process.
/// If the flow does not need to be controlled, then a Gateway is not needed. The term “gateway” implies that there is a gating mechanism that 
/// either allows or disallows passage through the Gateway--that is, as tokens arrive at a Gateway, they can be merged together on input and/or 
/// split apart on output as the Gateway mechanisms are invoked.
/// Gateways, like Activities, are capable of consuming or generating additional control tokens, effectively controlling the execution semantics 
/// of a given Process. The main difference is that Gateways do not represent ‘work’ being done and they are considered to have zero effect on the 
/// operational measures of the Process being executed (cost, time, etc.).
/// The Gateway controls the flow of both diverging and converging Sequence Flows. That is, a single Gateway could have multiple input and multiple 
/// output flows. Modelers and modeling tools might want to enforce a best practice of a Gateway only performing one of these functions. 
/// Thus, it would take two sequential Gateways to first converge and then to diverge the Sequence Flows.
/// </summary>
public abstract class GatewayBase(IFlowElementsContainer flowElementsContainer, string id, string name) : FlowNode(
    flowElementsContainer, id, name, eFlowNodeType.Gateway)
{
    public GatewayDirection gatewayDirection = GatewayDirection.Unspecified;

    public enum GatewayDirection
    {
        Unspecified,
        Converging,
        Diverging,
        Mixed
    }
}
