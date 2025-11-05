namespace Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

public class DbFileGroup
{
    public long Id;
    public string Name;
    public string Type;
    //public bool IsDefault;
    //public bool IsSystem;
    public DbPartitionFunction PartitionFunction;
    public Dictionary<string, DataFile> Files = [];

    public class DataFile
    {
        public string Name;
        public string PhysicalName;
    }
}

public class DbPartitionScheme
{
    public string Name;

    public string PartitionFunctionId { get; set; }
}
