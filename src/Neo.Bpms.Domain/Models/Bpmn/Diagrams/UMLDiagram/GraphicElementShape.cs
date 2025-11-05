using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class GraphicElementShape(string id, string name, Bounds bounds, GraphicalElement ge, DiagramElement owningElement) : Shape(id, name, bounds, null, owningElement)
	{
    public GraphicalElement element = ge;
	}
