using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;

public class InclusiveGatewayRuntime(ProcessVersionRuntime processVersion, InclusiveGateway inclusiveGateway) : GatewayRuntime(processVersion, inclusiveGateway)
{
    public InclusiveGateway InclusiveGateway => Gateway as InclusiveGateway;

    internal override void ReceiveToken(ProcessInstance pi,
        LocalParameters inputData,
        SequenceFlow seq)
    {
        if (CheckIncoming(pi, seq))
            SendTokenToOutgoing(pi, null, TokenPattern.Inclusive,
                InclusiveGateway.defaultSequenceFlowId);
    }

    private bool CheckIncoming(ProcessInstance pi, SequenceFlow seq)
    {
        if (InclusiveGateway.incoming.Count <= 1)
            return true;
        var instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(pi, this);
        var gwi = instances.Values.OfType<GatewaySyncWaitingInstance>().FirstOrDefault();
        if (gwi == null)
        {
            gwi = new GatewaySyncWaitingInstance(0, this, pi, ProcessInstanceStateId.Activated)
            {
                activationCount = 0,
                CreationTime = DateTime.UtcNow
            };
            gwi.Save();
            CheckAndAddNotStartedInputs(pi, seq, gwi);
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

        foreach (var input in InclusiveGateway.incoming)
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

    private void CheckAndAddNotStartedInputs(ProcessInstance pi, SequenceFlow seq, GatewaySyncWaitingInstance gwi)
    {
        foreach (var input in InclusiveGateway.incoming)
        {
            if (input.sourceRef.Id == seq.sourceRef.Id) continue;
            if (!HasActiveAi(pi, input.sourceRef))
                AddPseudoReceivedToken(gwi, input);
        }
    }

    private static bool HasActiveAi(ProcessInstance pi, FlowNode flowNode)
    {
        var checkedFlowNodes = new Dictionary<string, FlowNode>();
        return HasActiveAiByCheckFlowNode(pi, flowNode, checkedFlowNodes);
    }

    private static bool HasActiveAiByCheckFlowNode(ProcessInstance pi, FlowNode flowNode,
        IDictionary<string, FlowNode> checkedFlowNodes)
    {
        var flowNodeRunTime = pi.ProcessVersion.nodes.GetItem(flowNode.Id);
        if (checkedFlowNodes.ContainsKey(flowNode.Id))
            return false;
        checkedFlowNodes.Add(flowNode.Id, flowNode);
        if (flowNode.nodeType == FlowNode.eFlowNodeType.Gateway)
        {
            var gwt = (flowNode as Gateway)?.gatewayType;
            if (gwt == Gateway.eGatewayType.Complex ||
                gwt == Gateway.eGatewayType.Inclusive ||
                gwt == Gateway.eGatewayType.Parallel)
            {
                return false;
            }
        }
        var instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(pi, flowNodeRunTime);
        var ai = instances.Values.OfType<ActivityInstance>().FirstOrDefault();
        if (ai != null) return true;
        var ei = instances.Values.OfType<EventWaitingInstance>().FirstOrDefault();
        return ei != null ||
               flowNode.incoming.Any(input => HasActiveAiByCheckFlowNode(pi, input.sourceRef, checkedFlowNodes));
    }

    private static void AddPseudoReceivedToken(GatewaySyncWaitingInstance gwi, SequenceFlow input)
    {
        var rti = new ReceiveTokenInfo() { receiveTokenCount = 0 };
        gwi.ReceivedTokens.Add(input.sourceRef.Id, rti);
    }
}
