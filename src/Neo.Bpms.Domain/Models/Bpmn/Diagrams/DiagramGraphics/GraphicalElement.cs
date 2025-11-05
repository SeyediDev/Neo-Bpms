namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

public class GraphicalElement
{
    public List<Style> sharedStyles = null;
    public List<Style> localStyles = null;
    public List<ClipPath> clipPathes = null;
    public void clip(ClipPath clipPath)
    {
        clipPathes ??= [];
        clipPathes.Add(clipPath);
    }
    public void addStyle(Style style)
    {
        localStyles ??= [];
        localStyles.Add(style);
    }
    public void addSharedStyle(Style style)
    {
        sharedStyles ??= [];
        sharedStyles.Add(style);
    }
}
