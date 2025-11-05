namespace Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;

/// <summary>
/// WeakEntityAssociation attribute
/// 
/// defines additional info for association
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="constraint"></param>
/// <param name="constraintDbName"></param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class WeakEntityAssociationAttribute(string constraint = null, string constraintDbName = null) : Attribute
{
    public string Constraint { get; set; } = constraint;
    public string ConstraintDbName { get; set; } = constraintDbName;
}
