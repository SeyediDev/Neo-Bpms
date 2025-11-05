namespace Neo.Bpms.Domain.Models.Cmmn.Partitions;

public class PartitionScheme : BaseModelClass
{
    public PartitionScheme(string name, string enName)
            : base(null, enName, name)
    {
    }
    public PartitionScheme() { }
    public PartitionFunction PartitionFunction { get; set; }
    public FileGroupSelectionType FileGroupSelectionType { get; set; }
    public string FileGroupPrefix { get; set; }
    public List<string> FileGroups { get; set; }
}
public enum FileGroupSelectionType
{
    Monthly = 1,
    Daily = 3,
    FromList = 10,
    //...
}