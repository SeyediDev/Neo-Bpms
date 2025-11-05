namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

/// <summary>
/// Canvas is a kind of group that represents the root of containment for all graphical elements that render one diagram.
/// 
/// Canvas is a kind of group that is used as a root container of a hierarchy of graphical elements used to render the same
/// diagram. A canvas has a two-dimensional x-y coordinate system with a (x=0, y=0) origin point and an infinite size. The
/// coordinate system increases along the x-axis from left to right and along the y-axis from top to bottom, with negative
/// coordinates allowed. The coordinates of graphical elements nested in the canvas member hierarchy are relative to the
/// origin of the canvas. Unlike a group, a canvas has a visual manifestation in the form of a background that can be filled
/// separately from its member elements.
/// </summary>
public class Convas : Group
{
    /// <summary>
    /// a color that is used to paint the background of the canvas itself. A backgroundColor
    /// value is exclusive with a backgroundFill value. If both are specified, the backgroundFill value is used. If none is
    /// specified, no fill is applied
    /// </summary>
    public ColorItem backgroundColor;
    /// <summary>
    /// a reference to a fill that is used to paint the background of the canvas itself. A
    /// backgroundFill value is exclusive with a backgroundColor value. If both are specified, the backgroundFill value is
    /// used. If none is specified, no fill is applied
    /// </summary>
    public Fill backgroundFill = null;
    /// <summary>
    /// A set of markers packaged by the canvas and referenced by marked elements in the canvas.
    /// </summary>
    public List<Marker> packagedMarkers = null;
    /// <summary>
    /// a set of fills packaged by the canvas and referenced by graphical elements in the canvas.
    /// </summary>
    public List<Fill> packagedFills = null;
    /// <summary>
    /// a set of styles packaged by the canvas and referenced by graphical elements in the canvas as shared styles.
    /// </summary>
    public List<Style> packagedStyles = null;
}
