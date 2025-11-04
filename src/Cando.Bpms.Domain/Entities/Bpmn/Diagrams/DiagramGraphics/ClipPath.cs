namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

/// <summary>
/// ClipPath is a kind of group whose members collectively define a painting mask for its referencing graphical elements.
/// 
/// ClipPath represents a special kind of group element that is owned by a graphical element to define its clipping mask (or stencil). 
/// A clip path does not render as a normal graphical element but is only used to specify the regions that can be painted in its owning element.
/// 
/// The coordinate system of the clip path is the same as the one used by its owner (e.g., if the coordinates of the owner is
/// relative to the canvas, the coordinates of its clip path is also relative to the canvas). In addition, any transforms that are
/// defined on the graphical element are also applied to the clip path.
/// </summary>
public class ClipPath : Group
{
}
