namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class EllipticalArcTo : PathCommand
{
    public Point point;
    public Dimension radii;
    public double rotation;
    public bool isLargeArc;
    public bool isSweep;
}
