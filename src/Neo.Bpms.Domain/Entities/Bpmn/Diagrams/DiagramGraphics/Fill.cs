namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Fill
{
    public List<Transform> transforms = null;
    public void transform(Transform t)
    {
        transforms ??= [];
        transforms.Add(t);
    }
}
