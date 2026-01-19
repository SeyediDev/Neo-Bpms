using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;
public class ComplexGatewayRuntime(ProcessVersionRuntime processVersion, ComplexGateway gateway) : GatewayRuntime(processVersion,
    gateway)
{
    public ComplexGateway gatewayDefinition = gateway;

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        LocalParameters localParameters;
        if (CheckIncoming(pi, seq, out localParameters))
            SendTokenToOutgoing(pi, localParameters, TokenPattern.Inclusive, gatewayDefinition.defaultSequenceFlowId);
    }

    private bool CheckIncoming(ProcessInstance pi, SequenceFlow seq, out LocalParameters localParameters)
    {
        localParameters = null;
        if (gatewayDefinition.incoming.Count <= 1)
            return true;
        var instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(pi, this);
        var gwi = instances?.Values.OfType<GatewaySyncWaitingInstance>().FirstOrDefault();
        if (gwi == null)
        {
            gwi = new GatewaySyncWaitingInstance(0, this, pi, ProcessInstanceStateId.Activated)
            {
                activationCount = 0,
                CreationTime = DateTime.UtcNow
            };
            gwi.Save();
        }

        if (seq == null) return true;
        ReceiveTokenInfo rti;
        if (!gwi.ReceivedTokens.TryGetValue(seq.sourceRef.Id, out ReceiveTokenInfo value))
        {
            rti = new ReceiveTokenInfo { receiveTokenCount = 0 };
            gwi.ReceivedTokens.Add(seq.sourceRef.Id, rti);
        }
        else
            rti = value;

        rti.receiveTokenCount++;
        gwi.activationCount = gwi.ReceivedTokens.Count;
        bool activated;
        if (gwi.waitingForStart)
        {
            var lp = new LocalParameters(pi.AuditTrail?.User) { { "activationCount", gwi.activationCount } };
            var exp = gatewayDefinition.activationCondition.Expression;
            activated = ExpressionNode.CheckIfTrue(exp?.Root?.Eval(pi.Data, lp));
            if (activated)
                gwi.waitingForStart = false;
        }
        else
        {
            foreach (var input in gatewayDefinition.incoming)
            {
                if (!gwi.ReceivedTokens.ContainsKey(input.sourceRef.Id))
                {
                    gwi.Save();
                    return false;
                }
            }

            gwi.ReceivedTokens.Clear();
            gwi.Complete();
            activated = true;
        }

        gwi.Save();
        localParameters = new LocalParameters { { "waitingForStart", gwi.waitingForStart } };
        return activated;
    }
}
