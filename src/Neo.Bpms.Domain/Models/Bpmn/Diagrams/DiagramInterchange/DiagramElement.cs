using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

/// <inheritdoc />
/// <summary>
/// DiagramElement is the abstract super type of all elements in diagrams, including diagrams themselves. When contained
/// in a diagram, diagram elements are laid out relative to the diagram’s origin.
/// A diagram element can be useful on its own (i.e., purely notational) or more commonly used as a depiction of another
/// MOF-based element from an abstract syntax model (like a UML model). In the latter case, the diagram element references
/// the depicted model element and defines notational properties for that element. An example of a depicting diagram element
/// is a Class shape on a UML diagram that specifies the bounds of the class, its colors, its compartments...etc. An example
/// of a purely notational diagram element is a Note shape on a UML diagram that provides a textual description of part of
/// the diagram. The diagram element’s reference to model element is defined abstractly as derived union to allow language
/// specific extensions of DI to refine it further to suit their purposes (like specializing its type).
/// A diagram element can own other diagram elements in a graph-like hierarchy. The collection of owned elements is
/// defined abstractly as a derived union to allow language-specific extensions of DI to define the allowed topologies for their
/// diagram elements (e.g., a UML class shape can own UML compartments). This collection is also specialized in subclasses
/// of diagram element in the DI package.
/// More specialized diagram element types define properties that characterize their nature. However, a subset of those
/// properties is stylistic in nature and tends to have similar values across many diagram elements. Examples of such
/// properties are fill properties, stroke properties, and font properties. To minimize the footprint of diagram interchange
/// models, those stylistic properties are not defined on diagram elements directly but are rather defined on Style elements
/// that can be owned and/or shared by diagram elements. Shared style elements are owned by other elements, which might
/// be packaging elements in the language incorporating diagram interchange. Style property values are calculated based on a
/// well-defined algorithm given in “Style [Abstract Class]”
/// </summary>
public abstract class DiagramElement : BaseModelClass
{
    protected DiagramElement(string id, string name, BaseModelClass modelElement = null, DiagramElement owningElement = null)
        : base(modelElement, id, name)
    {
        owningElement?.AddSubElement(this);
    }

    public void AddSubElement(DiagramElement element)
    {
        element.owningElement = this;
        ownedElements ??= [];
        ownedElements.Add(element);
    }
    public object ModelElement => Parent;
    public List<DiagramElement> ownedElements;
    public DiagramElement owningElement;
    public List<Style> sharedStyles;
    public List<Style> localStyles;
    public void AddStyle(Style style)
    {
        localStyles ??= [];
        localStyles.Add(style);
    }
    public void AddSharedStyle(Style style)
    {
        sharedStyles ??= [];
        sharedStyles.Add(style);
    }
}
