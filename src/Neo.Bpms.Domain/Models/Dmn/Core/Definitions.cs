using Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

namespace Neo.Bpms.Domain.Models.Dmn.Core;

/// <summary>
/// The Definitions class is the outermost containing object for all elements of a DMN decision model. It defines the scope of visibility and the namespace for 
/// all contained elements. Elements that are contained in an instance of Definitions have their own defined life-cycle and are not deleted with the deletion of 
/// other elements. The interchange of DMN files will always be through one or more Definitions.
/// </summary>
public class Definitions(string id, string _namespace) : DMNElement(id)
{
    /// <summary>
    /// This attribute identifies the namespace associated with this Definitions and follows the convention established by XML Schema.
    /// </summary>
    public string _namespace = _namespace;
    /// <summary>
    /// This attribute identifies the expression language used in LiteralExpressions within the scope of this Definitions. 
    /// The Default is FEEL. This value MAY be overridden on each individual LiteralExpression. The language MUST be specified in a URI format.
    /// </summary>
    public string expressionLanguage = null;//"FEEL";
    /// <summary>
    /// This attribute identifies the type language used in LiteralExpressions within the scope of this Definitions. 
    /// The Default is FEEL. This value MAY be overridden on each individual ItemDefinition. The language MUST be specified in a URI format.
    /// </summary>
    public string typeLanguage = null;//"FEEL";
    /// <summary>
    /// This attribute lists the instances of ItemDefinition that are contained in this Definitions.
    /// </summary>
    Dictionary<string, DecisionLogic.DMNItemDefinition> itemDefinition = [];
    public void addItemDefinition(DecisionLogic.DMNItemDefinition itemDef)
    {
        itemDefinition.Add(itemDef.id, itemDef);
    }
    /// <summary>
    /// the instances of DRGElement that are contained in this Definitions.
    /// </summary>
    public Dictionary<string, DRGElement> drgElement = [];
    public void addDRGElement(DRGElement drgElmnt)
    {
        drgElement.Add(drgElmnt.id, drgElmnt);
    }
    /// <summary>
    /// the instances of BusinessContextElement that are contained in this Definitions
    /// </summary>
    public Dictionary<string, BusinessContextElement> businessContextElement = [];
    public void addBusinessContextElement(BusinessContextElement bce)
    {
        businessContextElement.Add(bce.id, bce);
    }
    /// <summary>
    /// the instances of ElementCollection that are contained in this Definitions.
    /// </summary>
    public Dictionary<string, ElementCollection> elementCollection = [];
    public void addElementCollection(ElementCollection ec)
    {
        elementCollection.Add(ec.id, ec);
    }
    /// <summary>
    /// This attribute is used to import externally defined elements and make them available for use by elements in this Definitions.
    /// </summary>
    public List<DmnImport> import = [];
}
