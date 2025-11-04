namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Ellipse(Point center, Dimension radii) : GraphicalElement
{
    public Point center = center;
    public Dimension radii = radii;
}
