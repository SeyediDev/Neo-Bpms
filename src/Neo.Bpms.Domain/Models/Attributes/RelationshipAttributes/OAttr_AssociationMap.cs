namespace Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;

/// <summary>
/// Association Map
/// 
/// Object fields(fields of type class) can have multiple Association Maps.
/// Association Map is for mapping fields between entity and associated entity if the keys are not equal.
/// </summary>
/// <remarks>
/// مشخص کردن فیلد های ارجاعی
/// </remarks>
/// <param name="sourceField">نام فیلد در مبدا</param>
/// <param name="destFeild">نام فیلد در مقصد</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class OAttr_AssociationMap(string sourceField, string destFeild) : Attribute
{
    public string MyField { get; set; } = sourceField.Trim();
    public string ObjectField { get; set; } = destFeild.Trim();
    public bool Bitmask { get; set; }
}