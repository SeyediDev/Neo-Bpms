using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;

public class ParallelGatewayRuntime(ProcessVersionRuntime processVersion, ParallelGateway gateway) : GatewayRuntime(
    processVersion, gateway)
{
    public ParallelGateway gatewayDefinition = gateway;

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        if (CheckIncoming(pi, seq))
            SendTokenToOutgoing(pi, null, TokenPattern.Parallel);
    }

    private bool CheckIncoming(ProcessInstance pi, SequenceFlow seq)
    {
        if (gatewayDefinition.incoming.Count <= 1)
            return true;
        var instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(pi, this);
        var gwi = instances.Values.OfType<GatewaySyncWaitingInstance>().FirstOrDefault();
        if (gwi == null)
        {
            gwi = new GatewaySyncWaitingInstance(0, this, pi, ProcessInstanceStateId.Activated)
            {
                activationCount = 0,
                CreationTime = DateTime.Now
            };
            gwi.Save();
        }

        if (seq == null) return true;
        ReceiveTokenInfo rti;
        if (!gwi.ReceivedTokens.TryGetValue(seq.sourceRef.Id, out ReceiveTokenInfo value))
        {
            rti = new ReceiveTokenInfo() { receiveTokenCount = 0 };
            gwi.ReceivedTokens.Add(seq.sourceRef.Id, rti);
        }
        else
            rti = value;

        rti.receiveTokenCount++;
        foreach (var input in gatewayDefinition.incoming)
        {
            if (!gwi.ReceivedTokens.ContainsKey(input.sourceRef.Id))
            {
                gwi.Save();
                return false;
            }
        }

        gwi.ReceivedTokens.Clear();
        gwi.CompleteAndSave();
        return true;
    }
}
