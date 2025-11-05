namespace Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;

/// <summary>
/// The ExtensionDefinition class defines and groups additional attributes. 
/// This type is not applicable when the XML schema interchange is used, since XSD Complex Types already satisfy this requirement
/// </summary>
public class ExtensionDefinition
{
    public string name;
    public List<ExtensionAttributeDefinition> extensionAttributeDefinitions;
    public ExtensionDefinition(string name)
    {
        this.name = name;
        extensionAttributeDefinitions = [];
    }
    public ExtensionDefinition(string name, List<ExtensionAttributeDefinition> extensionAttributeDefinitions)
    {
        this.name = name;
        this.extensionAttributeDefinitions = extensionAttributeDefinitions;
    }
}
