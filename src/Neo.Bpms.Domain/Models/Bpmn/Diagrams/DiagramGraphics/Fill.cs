namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

public class Fill
{
    public List<Transform> transforms = null;
    public void transform(Transform t)
    {
        transforms ??= [];
        transforms.Add(t);
    }
}
