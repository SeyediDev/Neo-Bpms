namespace Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

/// <summary>
/// The ExtensionAttributeValue contains the attribute value. This type is not applicable when the XML schema
/// interchange is used; since the XSD mechanisms for supporting “AnyAttribute” and “Any” type already satisfy this requirement.
/// </summary>
public class ExtensionAttributeValue(ExtensionAttributeDefinition extensionAttributeDefinition, Element value, Element valueRef)
{
    /// <summary>
    /// The contained attribute value, used when the associated ExtensionAttributeDefinition.isReference is false.
    /// The type of this Element MUST conform to the type specified in the associated ExtensionAttributeDefinition
    /// </summary>
    public Element value = value;
    /// <summary>
    /// The referenced attribute value, used when the associated ExtensionAttributeDefinition.isReference is true.
    /// The type of this Element MUST conform to the type specified in the associated ExtensionAttributeDefinition.
    /// </summary>
    public Element valueRef = valueRef;
    /// <summary>
    /// Defines the extension attribute for which this value is being provided.
    /// </summary>
    public ExtensionAttributeDefinition extensionAttributeDefinition = extensionAttributeDefinition;
}
