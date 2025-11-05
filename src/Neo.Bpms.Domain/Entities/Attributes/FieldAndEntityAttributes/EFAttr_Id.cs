namespace Neo.Bpms.Domain.Entities.Attributes.FieldAndEntityAttributes;

/// <summary>
/// Identification Attribute
/// 
/// This attribute delivers additional identification setting for fields and entities
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = true)]
public class EFAttr_Id : Attribute
{
    public string Name { get; set; }
    public string EnName { get; set; }
    public string DBName { get; set; }
    public string OldDbName { get; set; }
    public override string ToString() => Name;
}