using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

/// <summary>
/// An Information Requirement denotes Input Data or Decision output being used as input to a Decision.
/// 
/// The class InformationRequirement is used to model an information requirement, as represented by a plain arrow in a DRD.
/// An InformationRequirement element is a component of a Decision element, and it associates that requiring Decision element with a requiredDecision element, 
/// which is an instance of Decision, or a requiredInput element, which is an instance of InputData.
/// 
/// An instance of InformationRequirement is said to be well-formed if and only if all of the following are true:
/// . it references a requiredDecision or a requiredInput element, but not both,
/// . te referenced requiredDecision or requiredInput element is well-formed,
/// . and the Decision element that contains the instance of InformationRequirement is not in the requirement subgraph of the referenced requiredDecision element, if this InformationRequirement element references one.
/// </summary>
public class InformationRequirement
{
    /// <summary>
    /// The instance of Decision that this InformationRequirement associates with its containing Decision element
    /// </summary>
    public Decision requiredDecision;
    /// <summary>
    /// The instance of InputData that this InformationRequirement associates with its containing Decision element
    /// </summary>
    public InputData requiredInput;
    /// <summary>
    /// The instance of InformationItem that represents this InformationRequirement in the logic of the requiring Decision.
    /// </summary>
    public InformationItem variable;
    public InformationRequirement(Decision requiredDecision, InformationItem variable)
    {
        this.requiredDecision = requiredDecision;
        this.variable = variable;
        requiredInput = null;
    }
    public InformationRequirement(InputData requiredInput, InformationItem variable)
    {
        requiredDecision = null;
        this.variable = variable;
        this.requiredInput = requiredInput;
    }
}
