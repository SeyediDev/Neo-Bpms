namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// States
/// </summary>
/// <remarks>
/// وضعیت رکورد
/// </remarks>
/// <param name="enumType">فیلد از نوع Enum</param>
[AttributeUsage(AttributeTargets.Class)]
public class States(Type enumType) : Attribute
{
    public Type EnumType { get; set; } = enumType;
}