namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

public class Line(Point start, Point end) : MarkedElement
{
    public Point start = start;
    public Point end = end;
}
