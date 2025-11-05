using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionTable;

/// <summary>
/// In a decision table, a clause specifies a subject, which is defined by an input expression or an output domain, 
/// and the finite set of the sub-domains of the subject’s domain that are relevant for the piece of decision logic that is described by the decision table.
/// 
/// In DMN 1.0, the class Clause is used to model a decision table clause.
/// </summary>
public class Clause
{
    /// <summary>
    /// The subject of this input Clause.
    /// A Clause element MUST have a set of inputEntry if it has an inputExpression.
    /// MUST have a set of outputEntry if it does not have an inputExpression.
    /// </summary>
    public DMNExpression inputExpression;
    /// <summary>
    /// The range of this output Clause.
    /// </summary>
    public ItemDefinition outputDefinition;
    public string name;
    /// <summary>
    /// The instances of Expression that compose this Clause.
    /// </summary>
    public List<DMNExpression> inputEntry = [];
    /// <summary>
    /// The instances of Expression that compose this Clause.
    /// </summary>
    public List<DMNExpression> outputEntry = [];
    public Clause(DMNExpression inputExpression, IEnumerable<DMNExpression> inputEntry, ItemDefinition outputDefinition)
    {
        this.inputExpression = inputExpression;
        this.inputEntry = [.. inputEntry];
        this.outputDefinition = outputDefinition;
        name = null;
        outputEntry = null;
    }
    public Clause(string name, IEnumerable<DMNExpression> outputEntry, ItemDefinition outputDefinition)
    {
        inputExpression = null;
        inputEntry = null;
        this.outputDefinition = outputDefinition;
        this.name = name;
        this.outputEntry = [.. outputEntry];
    }
}
