namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;

public class TextAnnotation(IArtifactContainer parent, string id, string text, string textFormat = "text/plain") : Artifact(parent, id)
{
    public string text = text;

    /// <summary>
    /// mime-type format
    /// </summary>
    public string textFormat = textFormat;
}
