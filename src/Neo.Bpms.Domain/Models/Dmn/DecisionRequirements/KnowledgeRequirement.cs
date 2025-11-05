namespace Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

/// <summary>
/// A Knowledge Requirement denotes the invocation of a Business Knowledge Model by the decision logic of a Decision.
/// 
/// The class KnowledgeRequirement is used to model a knowledge requirement, as represented by a dashed arrow in a DRD.
/// A KnowledgeRequirement element is a component of a Decision element or of a BusinessKnowledgeModel element, and it associates that requiring 
/// Decision or BusinessKnowledgeModel element with a requiredKnowledge element, which is an instance of BusinessKnowledgeModel.
/// 
/// An instance of KnowledgeRequirement is said to be well-formed if and only if all of the following are true:
/// . it references a requiredKnowledge element,
/// . the referenced requiredKnowledge element is well-formed,
/// . and, if the InformationRequirement element is contained in an instance of BusinessKnowledgeModel, that BusinessKnowledgeModel element is not in the requirement subgraph of the referenced requiredKnowledge element.
/// </summary>
public class KnowledgeRequirement(BusinessKnowledgeModel requiredKnowledge)
{
    /// <summary>
    /// The instance of BusinessKnowledgeModel that this KnowledgeRequirement associates with its its containing Decision or BusinessKnowledgeModel element.
    /// </summary>
    public BusinessKnowledgeModel requiredKnowledge = requiredKnowledge;
}
