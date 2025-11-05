using Neo.Bpms.Domain.Models.Dmn.Core;

namespace Neo.Bpms.Domain.Models.Dmn.DecisionLogic;

/// <summary>
/// In DMN, the inputs and output of decisions are data items whose value, at the decision logic level, is assigned to variables or represented by value expressions.
/// An important characteristic of data items in decision models is their structure. DMN does not require a particular format for this data structure, 
/// but it does designate a subset of FEEL as its default.
/// 
/// In DMN 1.0, the class ItemDefinition is used to model the structure and the range of values of the input and the outcome of decisions.
/// 
/// The default type language for all elements can be specified in the Definitions element using the typeLanguage attribute. For example, a typeLanguage value of 
/// http://www.w3.org/2001/XMLSchema” indicates that the data structures using by elements within that Definitions are in the form of XML Schema types. 
/// If unspecified, the default is FEEL.
/// 
/// Notice that the data types that are built in the typeLanguage that is associated with an instance of Definitions need not be redefined by ItemDefinition 
/// elements contained in that Definitions element: they are considered imported and can be referenced in DMN elements within the Definitions element.
/// 
/// The type language can be overridden locally using the typeLanguage attribute in the ItemDefinition element.
/// Notice, also, that the data types and structures that are defined at the top level in a data model that is imported using an Import element that is 
/// associated with an instance of Definitions need not be redefined by ItemDefinition elements contained in that Definitions element: they are considered 
/// imported and can be referenced in DMN elements within the Definitions element.
/// 
/// An ItemDefinition element may have a typeDefinition, which is a String that defines the data structure using the typeLanguage, or a typeRef, 
/// which is a String that references a builtin data type in the associated typeLanguage or a type or data structure defined at the top level in an 
/// external document using the typeLanguage: in the latter case, the external document MUST be imported in the Definitions element that contains the 
/// instance of ItemDefinition, using an Import element. For example, in the case of data structures contributed by an XML schema, an Import would be used to 
/// specify the file location of that schema, and the typeRef attribute would reference the type or element definition in the imported schema.
/// 
/// By default, the name of an ItemDefinition is the name of the type that is defined in its typeDefinition or referenced in its typeRef. 
/// An ItemDefinition element MUST NOT have both a typeDefinition and a typeRef. If the type language is FEEL the builtin types are the FEEL built-in data types: 
/// number, string, boolean, duration, time and date and time.
/// 
/// An ItemDefinition element may restrict the values that are allowed from the typeDefinition or typeRef, using the allowedValue attribute: each allowedValue is an
/// instance of Expression that specifies a single allowed value or a range of allowed values from the typeDefinition or typeRef. The itemDefinition of the allowedValues 
/// MUST be the containing ItemDefinition element itself and MAY be omitted. 
/// If an ItemDefinition element contains one or more allowedValues, the list of the allowedValues specifies the complete range of values that this ItemDefinition represents. 
/// If an ItemDefinition element does not contain an allowedValue, its range of allowed values is the full range of the referenced typeRef or defined typeDefinition.
/// 
/// In cases where the values that an ItemDefinition element represents are collections of values in the allowed range, the multiplicity can be projected into the 
/// attribute isCollection. The default value for this attribute is false.
/// 
/// An alternative way to define an instance of ItemDefinition is as a composition of other ItemDefinition elements. An instance of ItemDefinition may reference zero or 
/// more itemComponentRef, which are ItemDefinition elements: each value in the range of an ItemDefinition element that references at least one itemComponentRef is made 
/// of one value in the range each of the referenced itemComponentRef elements.
/// 
/// An ItemDefinition element must be defined using only one of the alternative ways:
/// . inline definition of a data type or structure using a typeDefinition, possibly restricted with allowedValues;
/// . reference to a built-in or imported typeRef, possibly restricted with allowedValues;
/// . composition of other ItemDefinition elements, referencing itemComponentRef.
/// 
/// That is, an ItemDefinition element that references an itemComponentRef element MUST NOT have a typeDefinition, a typeRef or allowedValues. Reciprocally, 
/// an ItemDefinition element that has a typeDefinition or a typeRef attribute MUST NOT reference any itemComponentRef. As already mentioned above, an ItemDefinition 
/// element MUST NOT have both a typeDefinition and a typeRef.
/// </summary>
public class DMNItemDefinition : DMNElement
{
    /// <summary>
    /// This attribute is used to define in line the base data structure for this ItemDefinition
    /// </summary>
    public string typeDefinition;
    /// <summary>
    /// This attribute is used to identifies the base type of this ItemDefinition
    /// </summary>
    public string typeRef;
    /// <summary>
    /// This attribute identifies the type language used to specify the base type of this ItemDefinition. This value overrides the type language specified 
    /// in the Definitions element. The language MUST be specified in a URI format.
    /// </summary>
    public string typeLanguage;
    /// <summary>
    /// the Expression elements that define the values or range of values in the base type that are allowed in this ItemDefinition
    /// </summary>
    public List<DMNExpression> allowedValue = null;
    public void addAllowedValue(DMNExpression allowedValue)
    {
        this.allowedValue ??= [];
        this.allowedValue.Add(allowedValue);
    }
    /// <summary>
    /// the ItemDefinition elements that compose this ItemDefinition
    /// </summary>
    public List<DMNItemDefinition> itemComponentRef = null;
    public void addItemComponent(DMNItemDefinition itemComponentRef)
    {
        this.itemComponentRef ??= [];
        this.itemComponentRef.Add(itemComponentRef);
    }
    /// <summary>
    /// Setting this flag to true indicates that the actual values defined by this ItemDefinition are collections of allowed values. The default is false.
    /// </summary>
    public bool isCollection;

    public DMNItemDefinition(string id, string name, string typeDefinition)
        : base(id, name)
    {
        isCollection = false;
        this.typeDefinition = typeDefinition;
        typeRef = null;

    }
    public DMNItemDefinition(string id, string name, string typeRef, bool isCollection) : base(id, name)
    {
        this.isCollection = isCollection;
        this.typeRef = typeRef;
        typeDefinition = null;

    }
    public DMNItemDefinition(string id, string name, IEnumerable<DMNItemDefinition> itemComponentRef)
        : base(id, name)
    {
        isCollection = false;
        typeRef = null;
        typeDefinition = null;
        this.itemComponentRef = [.. itemComponentRef];
    }
}
