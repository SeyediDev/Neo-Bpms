namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Line(Point start, Point end) : MarkedElement
{
    public Point start = start;
    public Point end = end;
}
