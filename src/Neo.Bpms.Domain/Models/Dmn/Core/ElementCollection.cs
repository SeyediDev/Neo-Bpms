using Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

namespace Neo.Bpms.Domain.Models.Dmn.Core;

/// <summary>
/// The ElementCollection class is used to define named groups of DRGElement instances. ElementCollections may be used for any purpose relevant to an implementation.
/// To identify the requirements subgraph of a set one or more decisions (i.e. all the elements in the closure of the requirements of the set)
/// To identify the elements to be depicted on a DRD
/// </summary>
public class ElementCollection : DMNElement
{
    public ElementCollection(string id) : base(id) { }
    public ElementCollection(string id, string name) : base(id, name) { }

    public Dictionary<string, DRGElement> drgElement = [];
    public void addDRGElement(DRGElement drge)
    {
        drgElement.Add(drge.id, drge);
    }
}
