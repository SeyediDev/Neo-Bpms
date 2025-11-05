using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;

/// <summary>
/// The Complex Gateway can be used to model complex synchronization behavior. An Expression activationCondition is used to describe the precise behavior. 
/// For example, this Expression could specify that tokens on three out of five incoming Sequence Flows are needed to activate the Gateway. 
/// What tokens are produced by the Gateway is determined by conditions on the outgoing Sequence Flows as in the split behavior of the Inclusive Gateway. 
/// If tokens arrive later on the two remaining Sequence Flows, those tokens cause a reset of the Gateway and new token can be produced on the outgoing Sequence Flows. 
/// To determine whether it needs to wait for additional tokens before it can reset, the Gateway uses the synchronization semantics of the Inclusive Gateway.
/// 
/// The Complex Gateway has, in contrast to other Gateways, an internal state, which is represented by the boolean instance attribute waitingForStart, 
/// which is initially true and becomes false after activation. This attribute can be used in the conditions of the outgoing Sequence Flows to specify where 
/// tokens are produced upon activation and where tokens are produced upon reset. It is RECOMMENDED that each outgoing Sequence Flow either get a token upon
/// activation or upon reset but not both. At least one outgoing Sequence Flow should receive a token upon activation but a token MUST NOT be produced upon reset.
/// </summary>
public class ComplexGateway(IFlowElementsContainer flowElementsContainer, string id, string name, FormalExpression activationCondition) : Gateway(flowElementsContainer, id, name, eGatewayType.Complex), IHasDefaultSequenceFlow
{
    public string defaultSequenceFlowId { get; set; }
    /// <summary>
    /// Determines which combination of incoming tokens will be synchronized for activation of the Gateway.
    /// </summary>
    public FormalExpression activationCondition = activationCondition;
}