using Neo.Bpms.Domain.Entities.Dmn.Core;
using Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

/// <summary>
/// In DMN 1.0, the class InformationItem is used to model variables at the decision logic level in decision models.
/// The name of an InformationItem element MUST be unique within its scope.
/// 
/// In DMN, variables represent the values that are input to a decision, in the description of the decision’s logic, or the values that are passed to a module of 
/// decision logic that is defined as a function (and that is represented by a business knowledge model element). 
/// In the first case, a variable is the realization, at the decision logic level, of one of the information requirements (at the decision requirements level) of a decision; 
/// in the second case, a variable is one of  the parameters of the function that is the realization, at the decision logic level, of a business knowledge model element.
/// As a consequence, an InformationItem element MUST be either a variable in an instance of InformationRequirement or a parameter in an instance of BusinessKnowledgeModel; 
/// it MUST NOT be both. The scope of an InformationItem element is the Decision that contains the containing InformationRequirement element, or the containing 
/// BusinessKnowledgeModel element.
/// 
/// A variable in an instance of InformationRequirement MUST be an inputVariable in the decisionLogic in the Decision element that contains the InformationRequirement 
/// element. A parameter in an instance of BusinessKnowledgeModel MUST be an inputVariable in the valueExpression in that BusinessKnowledgeModel element.
/// 
/// As a concrete specialization of Expression, an InformationItem element can be interpreted and assigned a value. Specifically:
/// . An InformationItem element is assigned the value of the requiredDecision that is referenced by its containing instance of InformationRequirement, if it references one.
/// . An InformationItem element that is a parameter in a BusinessKnowledgeModel element can only be assigned a value using a Binding element as part of an instance of Invocation.
/// . Otherwise, an InformationItem element is assigned a value by the external data source that is attached at runtime to the requiredInput element that its 
/// containing instance of InformationRequirement references. How a data source is attached to an instance of InputData at run time, and how it assigns a value to an 
/// InformationItem element is out of the scope of DMN 1.0.
/// 
/// In any case, the valueDefinition element that is associated with an instance of InformationItem must be compatible with the valueDefinition that is associated 
/// with the DMN model element from which it takes its value
/// </summary>
public class InformationItem(string id, string name, ItemDefinition itemDefinition) : DMNElement(id, name)
{
    /// <summary>
    /// The Expression whose value is assigned to this InformationItem. This is a derived attribute
    /// </summary>
    public DMNExpression valueExpression = null;
    /// <summary>
    /// The instance of InformationRequirement in which this InformationItem is a part, if any.
    /// </summary>
    public InformationRequirement informationRequirement = null;
    /// <summary>
    /// The instance of ItemDefinition to which the value of this InformationItem must conform.
    /// </summary>
    public ItemDefinition itemDefinition = itemDefinition;
}
