namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

public class DataInputRef(DataInput dataInput, bool isOptional, bool availableWhileExecuting)
{
    public DataInput dataInput = dataInput;
    public bool isOptional = isOptional;
    public bool availableWhileExecuting = availableWhileExecuting;
}
