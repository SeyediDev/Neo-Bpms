namespace Neo.Bpms.Domain.Models.Cmmn.Partitions;

public class PartitionFunction : BaseModelClass
{
    public PartitionFunction(string name, string enName)
        : base(null, enName, name)
    {
    }

    public PartitionFunction()
    {
    }

    [XmlIgnore] public Type ValueType { get; set; }
    public PartitionFunctionType FunctionType { get; set; }
    public PartitionFunctionBoundaryType BoundaryType { get; set; }
    public string StartOfRange { get; set; }
    public string EndOfRange { get; set; }
    public List<string> Values { get; set; }
}

public enum PartitionFunctionType
{
    Monthly = 1,
    Daily = 3,
    FixRange = 10,
    //...
}

public enum PartitionFunctionBoundaryType
{
    Right = 1,
    Left = 2
}
