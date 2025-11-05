namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Marker(Dimension size, Point reference) : Group
{
    public Dimension size = size;
    public Point reference = reference;
}
