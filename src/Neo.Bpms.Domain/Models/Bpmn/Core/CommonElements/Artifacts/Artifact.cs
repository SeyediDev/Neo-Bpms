namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Artifacts;

/// <summary>
/// BPMN provides modelers with the capability of showing additional information about a Process that is not directly related to the Sequence Flows or Message Flows of the Process.
/// At this point, BPMN provides three standard Artifacts: Associations, Groups, and Text Annotations.
/// </summary>
public abstract class Artifact(IArtifactContainer parent, string id) : BaseElement(parent as BaseElement, id)
{
}

public interface IArtifactContainer
{
    List<Artifact> artifacts { get; set; }
}
