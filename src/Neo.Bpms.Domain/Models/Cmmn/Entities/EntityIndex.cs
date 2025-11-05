namespace Neo.Bpms.Domain.Models.Cmmn.Entities;

/// <summary>
/// Entity Index
/// 
/// each entity can have multiple unique or non unique indexes.
/// </summary>
public class EntityIndex
{
    public string Id;
    public string EnName;
    public string Name;
    public bool IsUnique;
    public List<IndexField> Fields = [];
    public bool Clustered;

    public EntityIndex Clone()
    {
        return new EntityIndex
        {
            Name = Name,
            Id = Id,
            EnName = EnName,
            Clustered = Clustered,
            IsUnique = IsUnique,
            Fields = [.. Fields]
        };
    }
}

public class IndexField
{
    public string FieldName;
    public bool IsIncluded;
    public bool IsDescending;
}
