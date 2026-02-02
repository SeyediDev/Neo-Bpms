namespace Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RelationshipConstraintAttribute(string constraint) : Attribute
{
    public string Constraint { get; set; } = constraint;
}

