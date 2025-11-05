using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Auditing;
/// <summary>
/// The Auditing element and its model associations allow defining attributes related to auditing. It leverages the BPMN
/// extensibility mechanism. This element is used by FlowElements and Process. The actual definition of auditing
/// attributes is out of scope of this International Standard.
/// </summary>
public class Auditing(IAuditingContainer parent, string id) : BaseElement(parent as BaseElement, id)
{
    public bool saveInstances { get; set; }
    public bool generateTraceLog { get; set; }
    public string logCondition { get; set; }
    public string breakPointCondition { get; set; }
}

public interface IAuditingContainer
{
    Auditing auditing { get; set; }
}
