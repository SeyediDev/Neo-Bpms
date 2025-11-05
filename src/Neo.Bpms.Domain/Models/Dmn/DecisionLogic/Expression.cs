using Neo.Bpms.Domain.Models.Dmn.Core;
using Neo.Bpms.Domain.Models.Dmn.DecisionTable;

namespace Neo.Bpms.Domain.Models.Dmn.DecisionLogic;

/// <summary>
/// An important characteristic of decisions and business knowledge models, in DMN, is that they may contain an expression that describes the logic 
/// by which a modeled decision shall be made, or pieces of that logic.
/// In DMN 1.0, the class Expression is the abstract super class for all expressions that are used to describe complete or parts of decision logic in DMN models 
/// and that return a single value when interpreted.
/// 
/// An instance of Expression is a component of a Decision element, of a BusinessKnowledgeModel element, or of an ItemDefinition element, or it is a component of 
/// another instance of Expression, directly or indirectly. The id of an Expression element MUST be unique within the containing instance of Decision, 
/// BusinessKnowledgeModel or ItemDefinition.
/// 
/// An instance of Expression can be interpreted to derive a single value from the values assigned to its inputVariables. 
/// How the value of an Expression element is derived from the values assigned to its inputVariables depends on the concrete kind of the Expression.
/// </summary>
public class DMNExpression(string id, IEnumerable<InformationItem> inputVariable, DMNItemDefinition itemDefinition) : DMNElement(id)
{
    /// <summary>
    /// the instances of InformationItem that are free in this Expression.
    /// The inputVariables are lexically scoped, in instances of Expression, and the scope is defined by the instance of Decision that contains them as part of an 
    /// informationRequirement element, or by the instance of BusinessKnowledgeModel that contains them as parameters. 
    /// An Expression element that is contained in an instance of ItemDefinition MUST NOT reference any inputVariable
    /// </summary>
    public List<InformationItem> inputVariable = [.. inputVariable];
    /// <summary>
    /// The instance of ItemDefinition to which the value of this Expression must conform.
    /// </summary>
    public DMNItemDefinition itemDefinition = itemDefinition;
    /// <summary>
    /// The containing instance of Clause, if this Expression is an inputEntry element in an instance of DecisionTable.
    /// </summary>
    public Clause inputClause = null;
    /// <summary>
    /// The containing instance of Clause, if this Expression is an outputEntry element in an instance of DecisionTable
    /// </summary>
    public Clause outputClause = null;
}
