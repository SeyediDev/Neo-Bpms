using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.FEEL;

/// <summary>
/// A list is written using square brackets to delimit the list, and commas to separate the list items
/// 
/// Contexts and lists can reference other contexts and lists, giving rise to a directed acyclic graph. Naming is path based. The qualified name (QN) of a context entry is of the form N1.N2 … Nn where N1 is the name of an in-scope context.
/// Nested lists encountered in the interpretation of N1.N2 … Nn are preserved.
/// 
/// Nested lists can be flattened using the flatten() built-in function
/// </summary>
public class FEELList(string id, IEnumerable<InformationItem> inputVariable,
    ItemDefinition itemDefinition) : DMNExpression(id, inputVariable, itemDefinition)
{
    /// <summary>
    /// This attributes lists the instances of Expression that are the elements in this List.
    /// </summary>
    public List<DMNExpression> element = [];
    public void addElement(DMNExpression element)
    {
        this.element.Add(element);
    }
}
