namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

/// <summary>
/// Circle is a graphical element that renders as a circle shape with a given center point and a radius in the x-y coordinate system.
/// </summary>
//[OCL] radius >= 0
public class Circle(Point center, double radous) : GraphicalElement
{
    /// <summary>
    /// the center point of the circle in the x-y coordinate system
    /// </summary>
    public Point center = center;
    /// <summary>
    /// a real number (>=0) that represents the radius of the circle.
    /// </summary>
    public double radous = radous;
}
