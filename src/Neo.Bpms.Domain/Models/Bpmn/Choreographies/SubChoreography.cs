using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Artifacts;

namespace Neo.Bpms.Domain.Models.Bpmn.Choreographies;

public class SubChoreography(Choreography choreography, string id, string name) : ChoreographyActivity(choreography, id, name), IArtifactContainer //, IFlowElementsContainer
{
    public List<Artifact> artifacts { get; set; }
}
