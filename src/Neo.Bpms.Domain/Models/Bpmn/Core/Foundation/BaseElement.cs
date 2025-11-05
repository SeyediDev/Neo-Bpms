namespace Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;

/// <summary>
/// BaseElement is the abstract super class for most BPMN elements. It provides the attributes id and documentation, which other elements will inherit.
/// </summary>
public abstract class BaseElement : BaseModelClass
{
    //		public string Id { get; set; }

    /// <summary>
    /// The documentation
    /// Any Model object can have multiple Documents.
    /// </summary>
    public List<Documentation> BaseElementDocumentation;

    /// <summary>
    /// This attribute is used to attach additional attributes and associations to any
    /// BaseElement. This association is not applicable when the XML schema
    /// interchange is used, since the XSD mechanisms for supporting
    /// anyAttribute and any element already satisfy this requirement.
    /// </summary>
    public List<ExtensionDefinition> extensionDefinitions;

    /// <summary>
    /// This attribute is used to provide values for extended attributes and model
    /// associations. This association is not applicable when the XML schema
    /// interchange is used, since the XSD mechanisms for supporting
    /// anyAttribute and any element already satisfy this requirement.
    /// </summary>
    // public List<ExtensionAttributeValue> extensionValues;

    //protected BaseElement()
    //{
    //}

    protected BaseElement(string id) : base(null, id, id)
    {
    }

    //[XmlIgnore]
    //public object parent { get; set; }

    [XmlIgnore]
    public BaseElement ParentElement => (BaseElement)Parent;
    protected BaseElement(BaseElement parent, string id, string name = null) :
        base(parent, id, name ?? id)
    {
    }
}
