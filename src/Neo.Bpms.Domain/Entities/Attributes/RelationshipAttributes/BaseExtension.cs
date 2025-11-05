namespace Neo.Bpms.Domain.Entities.Attributes.RelationshipAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class BaseExtensionAttribute(string booleanFieldIdInParentThatPresentMe = null) : Attribute
{
    public string BooleanFieldIdInParentThatPresentMe { get; set; } = booleanFieldIdInParentThatPresentMe;
}