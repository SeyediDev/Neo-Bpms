using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

/// <summary>
/// Diagram is an abstract container of a graph of diagram elements. Diagrams are diagram elements with an origin point in
/// the x-y coordinate system. Their elements are laid out relative to their origin point.
/// 
/// A diagram does not need to be nested. It can be persisted in the same resource as the abstract syntax model or in a
/// different resource. It can also be owned by elements of the abstract syntax model, or by no element at all (like being the
/// root of the resource).
/// A diagram represents a two dimensional x-y coordinate system that is used to layout nested and inter-connected diagram
/// elements. A diagram has an origin point (0, 0) on the x and y. The coordinate system of a diagram increases along the x axis
/// from left to right and along the y-axis from top to bottom. All the nested diagram elements are laid out relative to
/// their nesting diagram’s origin.
/// As a kind of diagram element, a diagram may reference a model element from an abstract syntax model, in which case the
/// whole diagram is considered a depiction of that element (e.g., an activity diagram is a depiction of a UML activity).
/// Alternatively, a diagram without such a reference is simply a layout container for its diagram elements (e.g., a class
/// diagram is a container for UML class shapes and edges).
/// A diagram can have a name and a documentation. This information is not shown as part of the rendering of the diagram
/// itself but can be used in an application to label a diagram (e.g., “DI Package Diagram”) in a browser and show its intent
/// (e.g., “A diagram that shows the classes of the DI package”).
/// 
/// A diagram also specifies a resolution expressed in units per inch. The resolution specifies the conversion ratio between
/// the logical units used by the diagram and a unit of physical measurement (an inch in this case). For example, a resolution
/// value of 300 specifies that every 300 logical unit of length map to an inch. The resolution value is mainly used when
/// printing diagrams or when rendering diagrams on display in their physical size.
/// Styles contain combinations of style property values used by different elements across the diagram. This allows a large
/// number of elements in a diagram to reference a small number of styles, which would dramatically reduce a diagram’s
/// footprint.
/// </summary>
public class Diagram(string id, string name, Bounds bounds, 
    BaseModelClass modelElement = null, DiagramElement owningElement = null) 
    : Shape(id, name, bounds, modelElement, owningElement)
{
    /// <summary>
    /// default: 300 - the resolution of the diagram expressed in user units per inch.
    /// </summary>
    public double resolution = 300;
    public List<Style> sharingStyles = [];
    public List<GraphicalElement> commonElements = [];
}
