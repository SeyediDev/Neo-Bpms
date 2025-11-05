namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

public class DataOutputRef(DataOutput dataOutput, bool isOptional, bool canBeProducedWhileExecuting)
{
    public DataOutput dataOutput = dataOutput;
    public bool isOptional = isOptional;
    public bool canBeProducedWhileExecuting = canBeProducedWhileExecuting;
}
