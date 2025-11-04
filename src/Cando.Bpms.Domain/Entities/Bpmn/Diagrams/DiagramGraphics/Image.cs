namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Image(string source, Bounds bounds, bool isAspectRatioPreserved) : GraphicalElement
{
    public string source = source;
    public Bounds bounds = bounds;
    public bool isAspectRatioPreserved = isAspectRatioPreserved;
    //stretch, tile
}
