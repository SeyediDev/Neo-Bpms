using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.UMLDiagram;

	public class RequirementShape(RequirementShape.eType type, string id, string name, Bounds bounds, BaseModelClass requirement, DiagramElement owningElement) : Shape(id, name, bounds, requirement, owningElement)
	{
		public eType type = type;
		public enum eType
		{
			NonFunctional,
			Functional,
		}
}
