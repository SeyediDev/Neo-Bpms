using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;

public abstract class GatewayRuntime(ProcessVersionRuntime processVersion, Gateway gateway) : FlowNodeRunTime(processVersion, gateway)
{
    internal override void WithdrawObsolete()
    {
        //todo
    }

    public Gateway Gateway => flowNode as Gateway;
}