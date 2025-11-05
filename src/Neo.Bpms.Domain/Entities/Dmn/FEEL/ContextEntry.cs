using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.FEEL;

/// <summary>
/// The class ContextEntry is used to model FEEL context entries when a context is modeled as a Context element.
/// </summary>
public class ContextEntry(InformationItem variable, DMNExpression value)
{
    /// <summary>
    /// its name is the key in the context entry
    /// </summary>
    public InformationItem variable = variable;
    /// <summary>
    /// the instance of Expression that models the expression in the context entry.
    /// </summary>
    public DMNExpression value = value;
}
