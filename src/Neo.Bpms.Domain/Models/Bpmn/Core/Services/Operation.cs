using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Models.Bpmn.Core.Services;

/// <summary>
/// An Operation defines Messages that are consumed and, optionally, produced when the Operation is called. 
/// It can also define zero or more errors that are returned when operation fails
/// </summary>
public class Operation(Interface parent, string id, string name, Message inMessageRef, Message outMessageRef,
    string implementationRef) : BaseElement(parent, id, name)
{
    //		public string name;

    /// <summary>
    /// An operation has exactly one input Message
    /// </summary>
    public Message inMessageRef = inMessageRef;

    /// <summary>
    /// An operation has at most one output Message
    /// </summary>
    public Message outMessageRef = outMessageRef;

    /// <summary>
    /// This attribute specifies errors that the Operation may return. 
    /// An Operation MAY refer to zero or more Error elements.
    /// </summary>
    public List<Error> errorRef;

    /// <summary>
    /// This attribute allows to reference a concrete artifact in the underlying implementation
    /// technology representing that operation, such as a WSDL operation.
    /// </summary>
    public string implementationRef = implementationRef;

    public Interface Interface => Parent as Interface;

    public void Copy(Operation newItem)
    {
        Name = newItem.Name;
        inMessageRef = newItem.inMessageRef;
        outMessageRef = newItem.outMessageRef;
        implementationRef = newItem.implementationRef;
        errorRef = newItem.errorRef;
        CloneBase(newItem);
    }

    public static string GenerateId(string interfaceName, string operationName)
    {
        return $"Operation.{interfaceName}.{operationName}";
    }
}
