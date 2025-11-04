using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;

public class InputOutputBinding
{
    /// <summary>
    /// A reference to one specific DataInput defined as part of the InputOutputSpecification of the Activity.
    /// </summary>
    public DataInput inputDataRef;
    /// <summary>
    /// A reference to one specific DataOutput defined as part of the InputOutputSpecification of the Activity.
    /// </summary>
    public DataOutput outputDataRef;
    /// <summary>
    /// A reference to one specific Operation defined as part of the Interface of the Activity.
    /// </summary>
    public Operation operationRef;
}