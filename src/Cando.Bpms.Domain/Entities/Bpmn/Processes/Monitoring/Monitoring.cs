using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Monitoring;

/// <summary>
/// The Monitoring element and its model associations allow defining attributes related to auditing. It leverages the BPMN
/// extensibility mechanism. This element is used by FlowElements and Process. The actual definition of monitoring
/// attributes is out of scope of this International Standard.
/// </summary>
public class Monitoring(IMonitoringContainer parent, string id) : BaseElement(parent as BaseElement, id)
{
}

public interface IMonitoringContainer
{
    Monitoring monitoring { get; set; }
}