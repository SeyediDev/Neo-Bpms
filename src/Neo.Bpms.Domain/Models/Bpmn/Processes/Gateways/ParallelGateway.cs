namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;

public class ParallelGateway(IFlowElementsContainer flowElementsContainer, string id, string name) : Gateway(flowElementsContainer, id, name, eGatewayType.Parallel)
{
}