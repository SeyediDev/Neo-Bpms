using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;

public class ExclusiveGatewayRuntime(ProcessVersionRuntime processVersion, ExclusiveGateway gateway) : GatewayRuntime(
    processVersion, gateway)
{
    public ExclusiveGateway gatewayDefinition = gateway;

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        SendTokenToOutgoing(pi, null, TokenPattern.Exclusive,
            gatewayDefinition.defaultSequenceFlowId);
    }
}
