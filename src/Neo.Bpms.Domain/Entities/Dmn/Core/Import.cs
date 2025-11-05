namespace Neo.Bpms.Domain.Entities.Dmn.Core;

/// <summary>
/// The Import class is used when referencing external elements, either DMN DRGElement instances contained in other Definitions elements, 
/// or non-DMN elements, such as an XML Schema or a PMML file. Imports must be explicitly defined.
/// </summary>
public class Import(string importType, string locationURI, string _namespace)
{
    /// <summary>
    /// Specifies the style of import associated with this Import.
    /// For example, a value of “http://www.w3.org/2001/XMLSchema” indicates that the imported element is an XML schema. 
    /// A value of <DMN namespace> indicates that the imported element is a DMN Definitions element.
    /// </summary>
    public string importType = importType;
    public string locationURI = locationURI;
    public string _namespace = _namespace;
}
