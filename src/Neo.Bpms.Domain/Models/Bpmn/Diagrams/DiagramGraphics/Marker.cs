namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

public class Marker(Dimension size, Point reference) : Group
{
    public Dimension size = size;
    public Point reference = reference;
}
