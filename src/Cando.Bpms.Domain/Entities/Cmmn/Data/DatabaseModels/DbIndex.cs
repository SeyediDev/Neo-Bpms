namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

public class DbIndex
{
    public bool AddField(string fieldName, bool isDescending, bool isIncluded)
    {
        Fields.Add(new DbIndexField
        {
            Name = fieldName,
            IsDescending = isDescending,
            IsIncluded = isIncluded
        });
        return true;
    }

    public string Name;

    public bool IsUnique;

    //public short m_DefragmentationPercent;
    public List<DbIndexField> Fields { get; set; } = [];

    public DbFileGroup DataSpace;
    public IndexType Type;
    public bool IsPrimaryKey;

    public enum IndexType
    {
        Heap,
        Clustered,
        NonClustered
    }

    public static bool IsPk(string name)
    {
        return name.Length > 2 && name[..2] == "PK" ||
                 name.Length > 4 && name[..4] == "SYS_";
    }

    public class DbIndexField
    {
        public string Name;
        public bool IsDescending;
        public bool IsIncluded;
    }

    public bool Clustered => Type == IndexType.Clustered;
}
