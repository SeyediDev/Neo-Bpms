using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

/// <summary>
/// Edge is a diagram element that renders as a poly line, connecting a source diagram element to a target diagram element,
/// and is positioned relative to the origin of the diagram.
/// 
/// 
/// Edge represents a diagram element defined with a sequence of connected way points forming a poly line that connects two
/// diagram elements: a source element and a target element (could be the same as the source as in self connection). The
/// way points are positioned relative to the origin of the nesting diagram, specifying a route for the poly line on the diagram.
/// An edge can be purely notational, i.e., does not reference any model element. An example is the line attaching a comment
/// to a UML element. On the other hand, an edge can be a depiction of a relational element from an abstract syntax model.
/// Examples include UML generalization edge or a BPMN message flow edge. In that case, the edge’s source and target
/// reference diagram elements depicting the relationship’s source and target elements (or its two related elements if the
/// relationship is not directed) respectively. The edge’s source and target references are defined abstractly as derived unions.
/// In an extending language-specific DI meta model, these references need to be refined. In case the source and target
/// references can be derived unambiguously from the model element, the properties can be redefined with that derivation
/// logic. Otherwise, the properties can be specialized with concrete settable properties.
/// </summary>
public abstract class Edge(string id, string name,
DiagramElement source, DiagramElement target,
BaseModelClass modelElement, DiagramElement owningElement) : DiagramElement(id, name, modelElement, owningElement)
{
    /// <summary>
    /// the edge’s source diagram element, i.e., where the edge starts from.
    /// </summary>
    public readonly DiagramElement source = source;
    /// <summary>
    /// the edge’s target diagram element, i.e., where the edge ends at.
    /// </summary>
    public readonly DiagramElement target = target;
    /// <summary>
    /// {ordered, non unique} - an optional list of points relative to the origin of the nesting diagram that
    /// specifies the connected line segments of the edge.
    /// </summary>
    public List<Point> wayPoints;
    public void AddWayPoint(Point point)
    {
        wayPoints ??= [];
        wayPoints.Add(point);
    }
}
