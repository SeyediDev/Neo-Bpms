namespace Neo.Bpms.Domain.Models.Bpmn.Core.Services;

/// <summary>
/// An Interface defines a set of operations that are implemented by Services
/// </summary>
public class Interface(BpmnDefinitions parent, string id, string name, object implementation) : RootElement(parent, id, name)
{
    //		public string name;
    public object implementation = implementation;
    public string implementationRef = name;

    /// <summary>
    /// operations that are defined as part of the Interface. An Interface has at least one Operation
    /// </summary>
    public Dictionary<string, Operation> operations = [];

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Interface newItem) return;
        Name = newItem.Name;
        operations = newItem.operations;
        implementationRef = newItem.implementationRef;
        //			implementation = newItem.implementation;
        CloneBase(newItem);
    }
}
