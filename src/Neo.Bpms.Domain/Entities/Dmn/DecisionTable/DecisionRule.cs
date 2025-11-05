using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionTable;

/// <summary>
/// In DMN 1.0, the class DecisionRule is used to model the rules in a decision table.
/// An instance of DecisionRule has a set of conditions and a non-empty set of conclusions, which are all instances of Expression.
/// 
/// An instance of Expression that is referenced by an instance of DecisionRule as a condition MUST be associated with a containing clause in which it is an inputEntry. 
/// In the same way, an instance of Expression that is referenced by an instance of DecisionRule as a conclusion MUST be associated with a containing clause in which 
/// it is an outputEntry.
/// A DecisonRule element MUST not have more than one conclusion contained in the same clause.
/// 
/// By definition, a DecisionRule element that has no condition is always applicable. Otherwise, given a set of values for the inputExpressions of the clauses 
/// of its condition, an instance of DecisionRule is said to be applicable if and only if, for each Clause element that contains at least one of the rule’s condition, 
/// at least one the rule’s conditions that is contained in the Clause element is true. Equivalently, in logical terms, a DecisionRule element is said to be applicable 
/// if the conjunction is true where there is a conjunct per Clause element that has at least one inputEntry referenced as a condition by the DecisionRule element, 
/// and each conjunct is a disjunction of all the rule’s conditions that are contained in the same Clause element.
/// </summary>
public class DecisionRule
{
    /// <summary>
    /// This attribute lists the instances of Expression that compose the condition of this DecisionRule.
    /// </summary>
    public List<DMNExpression> condition = [];
    /// <summary>
    /// the instances of Expression that compose the conclusion of this DecisionRule.
    /// </summary>
    public List<DMNExpression> conclusion = [];
    public DecisionRule() { }
    public DecisionRule(IEnumerable<DMNExpression> condition, IEnumerable<DMNExpression> conclusion)
    {
        this.condition = [.. condition];
        this.conclusion = [.. conclusion];
    }
}
