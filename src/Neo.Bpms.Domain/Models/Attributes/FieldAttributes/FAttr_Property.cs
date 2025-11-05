namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

/// <summary>
/// اضافه کردن ویژگی به فیلد
/// </summary>
/// <param name="propertyId">ویژگی</param>
/// <param name="value">مقدار</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class FieldProperty(EntityFieldPropertyId propertyId, object value) : Attribute
{
    public EntityFieldPropertyId PropertyId { get; set; } = propertyId;
    public object Value { get; set; } = value;
}
