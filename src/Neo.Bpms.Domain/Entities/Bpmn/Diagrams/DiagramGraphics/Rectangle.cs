namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Rectangle(Bounds bounds, double cornerRadius) : GraphicalElement
{
    public Bounds bounds = bounds;
    public double cornerRadius = cornerRadius;
}
