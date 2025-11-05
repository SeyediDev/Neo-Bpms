namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public abstract class FieldValidation : Attribute
{
}