using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;
public class ExclusiveGateway(IFlowElementsContainer flowElementsContainer, string id, string name) : Gateway(flowElementsContainer, id, name, eGatewayType.Exclusive), IHasDefaultSequenceFlow
{
    public string defaultSequenceFlowId { get; set; }
}

public interface IHasDefaultSequenceFlow
{
    /// <summary>
    /// The name of defaultSequenceFlowId property in BPMN.2 is default.
    /// </summary>
    string defaultSequenceFlowId { get; set; }
}