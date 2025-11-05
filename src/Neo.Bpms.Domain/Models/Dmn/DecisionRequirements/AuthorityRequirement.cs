namespace Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

/// <summary>
/// An Authority Requirement denotes the dependence of a DRG element on a Knowledge Source, or the dependence of a Knowledge Source on a DRG element.
/// 
/// The class AuthorityRequirement is used to model an authority requirement, as represented by an arrow drawn with a dashed line and a filled circular head in a DRD.
/// An AuthorityRequirement element is a component of a Decision, BusinessKnowledgeModel or KnowledgeSource element, and it associates that requiring 
/// Decision, BusinessKnowledgeModel or KnowledgeSource element with a requiredAuthority element, which is an instance of 
/// KnowledgeSource, a requiredDecision element, which is an instance of Decision, or a requiredInput element, which is an instance of InputData.
/// </summary>
public class AuthorityRequirement
{
    /// <summary>
    /// The instance of KnowledgeSource that this AuthorityRequirement associates with its its containing KnowledgeSource, Decision or BusinessKnowledgeModel element
    /// </summary>
    public KnowledgeSource requiredAuthority;
    /// <summary>
    /// The instance of Decision that this AuthorityRequirement associates with its containing Decision element
    /// </summary>
    public Decision requiredDecision;
    /// <summary>
    /// The instance of InputData that this AuthorityRequirement associates with its containing Decision element
    /// </summary>
    public InputData requiredInput;
    /// <summary>
    /// The instance of InformationItem that represents this InformationRequirement in the logic of the requiring Decision.
    /// </summary>

    public AuthorityRequirement(Decision requiredDecision, KnowledgeSource requiredAuthority)
    {
        this.requiredAuthority = requiredAuthority;
        this.requiredDecision = requiredDecision;
        requiredInput = null;
    }
    public AuthorityRequirement(InputData requiredInput, KnowledgeSource requiredAuthority)
    {
        this.requiredAuthority = requiredAuthority;
        requiredDecision = null;
        this.requiredInput = requiredInput;
    }
}
