using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class NodeShape(string id, string name, Bounds bounds, BaseModelClass term = null, DiagramElement owningElement = null) : Shape(id, name, bounds, term, owningElement)
	{
}
