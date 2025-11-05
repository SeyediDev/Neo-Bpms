using Neo.Bpms.Domain.Models.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

/// <summary>
/// An Input Data element denotes information used as an input by one or more Decisions.
/// 
/// DMN uses the class InputData to model the inputs of a decision whose values are defined outside of the decision model.
/// 
/// In a DRD, an instance of InputData is represented by an input data diagram element. An InputData element does not have a requirement subgraph, and it is always well-formed.
/// </summary>
public class InputData : DRGElement
{
    public InputData(string id) : base(id) { itemDefinition = null; }
    public InputData(string id, DMNItemDefinition itemDefinition) : base(id) { this.itemDefinition = itemDefinition; }
    public InputData(string id, string name) : base(id, name) { itemDefinition = null; }
    public InputData(string id, string name, DMNItemDefinition itemDefinition) : base(id, name) { this.itemDefinition = itemDefinition; }
    /// <summary>
    /// The instance of ItemDefinition that describes the data type expected for this InputData.
    /// </summary>
    public DMNItemDefinition itemDefinition;
}
