namespace Neo.Bpms.Domain.Models.Dmn.Core;

/// <summary>
/// DMNElement is the abstract super class for the decision requirement model elements . 
/// It provides the mandatory attribute id and the optional attributes name and description, which all are Strings, and which other elements will inherit. 
/// The id of a DMNElement element MUST be unique within the containing element.
/// 
/// DMNElement has three abstract specializations: Expression, BusinessContextElement and DRGElement and four concrete specializations: 
/// Definitions, ItemDefinition, InformationItem and ElementCollection.
/// </summary>
public class DMNElement
{
    /// <summary>
    /// The string that identifies this DMNElement uniquely within its containing Definitions element.
    /// </summary>
    public string id;
    public string name;
    public string description;
    public DMNElement(string id)
    {
        this.id = id;
        name = null;
        description = null;
    }
    public DMNElement(string id, string name)
    {
        this.id = id;
        this.name = name;
        description = null;
    }
    public DMNElement(string id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
    }
}
