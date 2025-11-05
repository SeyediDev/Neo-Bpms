using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

/// <summary>
/// A Business Knowledge Model element denotes a function encapsulating business knowledge, e.g. as business rules, a decision table, or an analytic model.
/// 
/// The business knowledge models that are associated with a decision are reusable modular expressions of all or part of their decision logic.
/// In DMN 1.0, the class BusinessKnowledgeModel is used to model a business knowledge model.
/// 
/// In a DRD, an instance of BusinessKnowledgeModel is represented by a business knowledge model diagram element.
/// 
/// An instance of BusinessKnowledgeModel is said to be well-formed if and only if, either it does not have any knowledgeRequirement, 
/// or all of its knowledgeRequirement elements are well-formed. That condition entails, in particular, that the requirement subgraph of a
/// BusinessKnowledgeModel element MUST be acyclic, that is, that a BusinessKnowledgeModel element MUST not require itself, directly or indirectly.
/// 
/// At the decision logic level, a BusinessKnowledgeModel element defines a function. It may be composed of an associated body, 
/// which is an instance of Expression and of zero or more parameter, which are instances of InformationItem. 
/// The body that is associated with a BusinessKnowledgeModel element is the reusable module of decision logic that is represented by this BusinessKnowledgeModel element. 
/// The parameters in a BusinessKnowledgeModel element are the inputVariables that are referenced by its body
/// </summary>
public class BusinessKnowledgeModel(string id, string name, string description, DMNExpression body, IEnumerable<InformationItem> parameters) : DRGElement(id, name, description)
{
    /// <summary>
    /// The instance of Expression that describes the logic represented by this BusinessKnowledgeModel, that is, the body of the function that it defines.
    /// </summary>
    public DMNExpression body = body;
    /// <summary>
    /// the instances of InformationItem that model the parameters of the function that this BusinessKnowledgeModel defines
    /// </summary>
    public List<InformationItem> parameters = [.. parameters];
    /// <summary>
    /// the instances of KnowledgeRequirement that compose this BusinessKnowledgeModel
    /// </summary>
    public List<KnowledgeRequirement> knowledgeRequirement = null;
    public void addKnowledgeRequirement(KnowledgeRequirement knowledgeRequirement)
    {
        this.knowledgeRequirement ??= [];
        this.knowledgeRequirement.Add(knowledgeRequirement);
    }
    /// <summary>
    /// the instances of AuthorityRequirement that compose this BusinessKnowledgeModel
    /// </summary>
    public List<AuthorityRequirement> authorityRequirement = null;
    public void addAuthorityRequirement(AuthorityRequirement authorityRequirement)
    {
        this.authorityRequirement ??= [];
        this.authorityRequirement.Add(authorityRequirement);
    }
}
