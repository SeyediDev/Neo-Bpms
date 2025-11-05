using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.FEEL;

/// <summary>
/// A Context element is represented diagrammatically as a boxed context. 
/// A FEEL context SHOULD be modeled as a Context element whenever possible.
/// </summary>
public class Context(string id, IEnumerable<InformationItem> inputVariable, ItemDefinition itemDefinition) 
    : DMNExpression(id, inputVariable, itemDefinition)
{
    /// <summary>
    /// This attributes lists the instances of ContextEntry that compose this Context.
    /// A context is a map of key-value pairs called context entries, and is written using curly braces to delimit the context, commas to separate the entries, 
    /// and a colon to separate key and value (grammar rule 59). The key can be a string or a name. The value is an expression.
    /// </summary>
    public List<ContextEntry> contextEntry = [];
}
