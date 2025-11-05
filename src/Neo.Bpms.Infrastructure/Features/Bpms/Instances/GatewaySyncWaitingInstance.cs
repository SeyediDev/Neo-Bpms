using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public class GatewaySyncWaitingInstance : FlowNodeInstance
{
    public GatewaySyncWaitingInstance(long instanceId, GatewayRuntime gatewayRuntime, ProcessInstance pi,
        ProcessInstanceStateId instanceState)
        : base(instanceId, gatewayRuntime, pi, instanceState)
    {
    }

    public GatewaySyncWaitingInstance(ActivityInstanceRecordDb air, GatewayRuntime gatewayRuntime, ProcessInstance pi)
        : base(air, gatewayRuntime, pi)
    {
        ReceivedTokens = Deserialize(air.ReceivedTokens);
        waitingForStart = air.waitingForStart;
        activationCount = air.activationCount;
    }

    public Gateway Gateway => (Gateway)FlowNode;

    /// <summary>
    /// for parallel, complex, inclusive gateways, it is needed to control the paths which has sent token.
    /// 
    /// 
    /// for parallel event based gateways, it is needed to control the events received from back tokens.
    /// </summary>
    public Dictionary<string, ReceiveTokenInfo> ReceivedTokens = [];

    //Instance attributes related to the Complex Gateway:
    /// <summary>
    /// Refers at runtime to the number of tokens that are present on an incoming Sequence Flow of the Complex Gateway.
    /// </summary>
    public long activationCount;

    /// <summary>
    /// Represents the internal state of the Complex Gateway. It is either waiting for start (=true) or waiting for reset (=false).
    /// </summary>
    public bool waitingForStart = true;

    public bool IsExecuting()
    {
        return state switch
        {
            ProcessInstanceStateId.Activated => true,
            _ => false,
        };
    }

    private static Dictionary<string, ReceiveTokenInfo> Deserialize(string receivedTokens)
    {
        Dictionary<string, ReceiveTokenInfo> result = [];
        string[] receivedTokenList = receivedTokens?.Split(',');
        foreach (string receivedToken in receivedTokenList ?? Enumerable.Empty<string>())
        {
            string[] receivedTokenParts = receivedToken.Split(':');
            if (receivedTokenParts.Length == 2)
            {
                ReceiveTokenInfo rti = new();
                if (long.TryParse(receivedTokenParts[1], out rti.receiveTokenCount))
                {
                    result.Add(receivedTokenParts[0], rti);
                }
            }
        }

        return result;
    }
}

public class ReceiveTokenInfo
{
    public long receiveTokenCount;
}
