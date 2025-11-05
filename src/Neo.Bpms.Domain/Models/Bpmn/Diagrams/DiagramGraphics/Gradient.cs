namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

public class Gradient : Fill
{
    public Gradient()
    {

    }
    public Gradient(GradientStop stop1, GradientStop stop2, params GradientStop[] otherStops)
    {
        stops.Add(stop1);
        stops.Add(stop2);
        foreach (GradientStop stop in otherStops)
        {
            stops.Add(stop);
        }
    }
    public List<GradientStop> stops = [];
}
