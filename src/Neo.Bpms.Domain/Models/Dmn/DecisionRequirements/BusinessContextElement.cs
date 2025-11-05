using Neo.Bpms.Domain.Models.Dmn.Core;

namespace Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

/// <summary>
/// The abstract class BusinessContextElement, and its concrete specializations PerformanceIndicator and OrganizationUnit are placeholders, 
/// anticipating a definition to be adopted from other OMG meta-models, such as OMG OSM when it is further developed.
/// </summary>
public class BusinessContextElement : DMNElement
{
    /// <summary>
    /// The URI of this BusinessContextElement
    /// an instance of PerformanceIndicator references any number of impactingDecision, which are the Decision elements that impact it;
    /// an instance of OrganisationalUnit references any number of decisionMade and of decisionOwned, which are the Decision elements 
    /// that model the decisions that the organization unit makes or owns
    /// </summary>
    public string URI;

    public BusinessContextElement(string id, string URI) : base(id) { this.URI = URI; }
    public BusinessContextElement(string id, string name, string URI) : base(id, name) { this.URI = URI; }
    public BusinessContextElement(string id, string name, string description, string URI) : base(id, name, description) { this.URI = URI; }
}
