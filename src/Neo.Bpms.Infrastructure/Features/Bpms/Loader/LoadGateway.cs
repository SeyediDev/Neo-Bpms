using Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway.RuntimeEventBasedGateway;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    private FlowNodeRunTime AddGateway(ProcessVersionRuntime processVersion, Gateway gateway)
    {
        FlowNodeRunTime result = null;
        switch (gateway)
        {
            case ExclusiveGateway exclusiveGateway:
                result = new ExclusiveGatewayRuntime(processVersion, exclusiveGateway);
                break;
            case InclusiveGateway inclusiveGateway:
                result = new InclusiveGatewayRuntime(processVersion, inclusiveGateway);
                break;
            case ParallelGateway parallelGateway:
                result = new ParallelGatewayRuntime(processVersion, parallelGateway);
                break;
            case EventBasedGateway eventBasedGateway:
                result = new EventBasedGatewayRuntime(processVersion, eventBasedGateway);
                break;
            case ComplexGateway complexGateway:
                result = new ComplexGatewayRuntime(processVersion, complexGateway);
                break;
        }

        return result;
    }

    private static void SetEventBasedGatewayLinks(ProcessVersionRuntime processVersion)
    {
        foreach (EventBasedGatewayRuntime eventBasedGatewayRuntime in processVersion.nodes.Values.OfType<EventBasedGatewayRuntime>())
        {
            foreach (var sequenceFlow in eventBasedGatewayRuntime.Gateway.outgoing)
            {
                if (processVersion.TryGetFlowNodeRuntime(sequenceFlow.targetRef.Name, out FlowNodeRunTime targetRef))
                {
                    targetRef.IncomingEventBasedGatewayRuntime = eventBasedGatewayRuntime;
                }
            }
        }
    }
}
