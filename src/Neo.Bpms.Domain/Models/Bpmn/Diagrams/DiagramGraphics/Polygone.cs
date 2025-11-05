namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

public class Polygone : MarkedElement
{
    public Polygone(Point point1, Point point2, Point point3, params Point[] otherPoints)
    {
        points.Add(point1);
        points.Add(point2);
        points.Add(point3);
        foreach (Point point in otherPoints)
        {
            points.Add(point);
        }
    }
    public List<Point> points = [];
}
