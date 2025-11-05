using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway.RuntimeEventBasedGateway;

public class EventBasedGatewayRuntime(ProcessVersionRuntime processVersion, EventBasedGateway gateway) 
    : GatewayRuntime(processVersion, gateway)
{
    public EventBasedGateway EventBasedGateway => Gateway as EventBasedGateway;

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        if (EventBasedGateway.instantiate)
        {
            GatewaySyncWaitingInstance oldGInstance = DataStorage
                .FetchFlowNodeInstancesOfFlowNode(IncomingEventBasedGatewayRuntime, pi.Execution, pi)
                ?.Values.OfType<GatewaySyncWaitingInstance>()
                .FirstOrDefault();
            if (oldGInstance != null)
            {
                GatewaySyncWaitingInstance gwi = new(0, this, pi,
                    ProcessInstanceStateId.Activated)
                {
                    CreationTime = DateTime.Now
                };
                gwi.Save();
            }
        }

        SendTokenToOutgoing(pi, inputData, TokenPattern.Parallel);
    }
}
