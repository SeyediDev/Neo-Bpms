using Neo.Bpms.Domain.Entities.Dmn.Core;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

/// <summary>
/// DRGElement is the abstract super class for all DMN elements that are contained within Definitions and that have a graphical representation in a DRD. 
/// All the elements of a DMN decision model that are not contained directly in a Definitions element (specifically: all three kinds of requirement, bindings, 
/// clause and decision rules, import, and objective)MUST be contained in an instance of DRGElement, or in a model element that is contained in an instance of DRGElement, 
/// recursively.
/// The concrete specializations of DRGElement are Decision, InputData, BusinessKnowledgeModel and KnowledgeSource.
/// A Decision Requirements Diagram (DRD) is the diagrammatic representation of one or more instances of DRGElement and their information, 
/// knowledge and authority requirement relations. The instances of DRGElement are represented as the vertices in the diagram; the edges represent instances of 
/// InformationRequirement, KnowledgeRequirement or AuthorityRequirement
/// </summary>
public class DRGElement : DMNElement
{
    public DRGElement(string id, string name, string description) : base(id, name, description) { }
    public DRGElement(string id, string name) : base(id, name, null) { }
    public DRGElement(string id) : base(id, null, null) { }
}
