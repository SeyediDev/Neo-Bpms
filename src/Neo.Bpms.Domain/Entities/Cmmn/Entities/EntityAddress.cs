namespace Neo.Bpms.Domain.Entities.Cmmn.Entities;

public class EntityAddress
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string Key => $"{NamespaceId}.{EntityId}";

    public bool Equals(EntityAddress entityAddress)
    {
        return Key.Equals(entityAddress.Key);
    }
}