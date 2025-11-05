using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;

public class InclusiveGateway(IFlowElementsContainer flowElementsContainer, string id, string name) : Gateway(flowElementsContainer, id, name, eGatewayType.Inclusive), IHasDefaultSequenceFlow
{
    public string defaultSequenceFlowId { get; set; }
}