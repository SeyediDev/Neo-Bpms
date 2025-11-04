using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;

namespace Neo.Bpms.Domain.Entities.Bpmn.Choreographies;

public class SubChoreography(Choreography choreography, string id, string name) : ChoreographyActivity(choreography, id, name), IArtifactContainer //, IFlowElementsContainer
{
    public List<Artifact> artifacts { get; set; }
}
