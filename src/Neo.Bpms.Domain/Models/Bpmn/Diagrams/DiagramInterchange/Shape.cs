using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

/// <summary>
/// Shape is a diagram element with given bounds that is laid out relative to the origin of the diagram.
/// 
/// Shape is an abstract class that is expected to be further sub classed in a language-specific DI meta model.
/// A shape can be purely notational, i.e., does not reference any model element. Examples include a note shape on a UML
/// class diagram with some text describing the diagram and an overlay shape with some semi-transparent fill enclosing a
/// bunch of shapes on the diagram to make them stand out. On the other hand, a shape can be a depiction of a component
/// (non-relational) element from an abstract syntax model. Examples include a UML class shape and a BPMN activity
/// shape.
/// </summary>
public abstract class Shape(string id, string name, 
    Bounds bounds, BaseModelClass modelElement = null, DiagramElement owningElement = null) 
    : DiagramElement(id, name, modelElement, owningElement)
{
    /// <summary>
    /// the optional bounds of the shape relative to the origin of its nesting plane.
    /// </summary>
    public Bounds bounds = bounds;
}
