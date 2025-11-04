using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.UMLDiagram;

public class Actor(string id, string name, Bounds bounds, BaseModelClass processRole, DiagramElement owningElement) : Shape(id, name, bounds, processRole, owningElement)
{
}
public class CollaborationShape(string id, string name, Bounds bounds, DiagramElement owningElement) : Shape(id, name, bounds, null, owningElement)
{
}
