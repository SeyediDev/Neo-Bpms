namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Polyline : MarkedElement
{
    public Polyline(Point point1, Point point2, params Point[] otherPoints)
    {
        points.Add(point1);
        points.Add(point2);
        foreach (Point point in otherPoints)
        {
            points.Add(point);
        }
    }
    public List<Point> points = [];
}
