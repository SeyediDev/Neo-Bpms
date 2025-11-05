using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.UMLDiagram;

	public class NoteShape(string id, string name, Bounds bounds, DiagramElement owningElement) : Shape(id, name, bounds, null, owningElement)
	{
}
