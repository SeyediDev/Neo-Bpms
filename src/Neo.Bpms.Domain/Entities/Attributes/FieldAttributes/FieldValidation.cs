namespace Neo.Bpms.Domain.Entities.Attributes.FieldAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public abstract class FieldValidation : Attribute
{
}