namespace Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;

/// <summary>
/// The Import class is used when referencing external element, either BPMN elements contained in other BPMN Definitions or non-BPMN elements.
/// </summary>
public class Import(string importType, string location, string _namespace)
{
    /// <summary>
    /// The value of the importType attribute MUST be set to http://www.w3.org/2001/XMLSchema when importing XML Schema 1.0 documents, 
    /// to http://www.w3.org/TR/wsdl20/ when importing WSDL 2.0 documents, 
    /// and http://www.omg.org/spec/BPMN/20100524/MODEL when importing BPMN 2.0 documents.
    /// Xml Schema 1.0, WSDL 2.0 and BPMN 2.0 types MUST be supported
    /// </summary>
    public string importType = importType;
    public string location = location;
    public string _namespace = _namespace;
}
