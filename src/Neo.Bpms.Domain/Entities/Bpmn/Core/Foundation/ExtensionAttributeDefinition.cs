namespace Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

/// <summary>
/// The ExtensionAttributeDefinition defines new attributes. This type is not applicable when the XML schema interchange is used; 
/// since the XSD mechanisms for supporting “AnyAttribute” and “Any” type already satisfy this requirement.
/// </summary>
public class ExtensionAttributeDefinition(string name, string type, bool isReference)
{
    public string name = name;
    public string type = type;
    /// <summary>
    /// Indicates if the attribute value will be referenced or contained.
    /// </summary>
    public bool isReference = isReference;

    public ExtensionAttributeDefinition(string name, string type) :
        this(name, type, false)
    {
    }
}
