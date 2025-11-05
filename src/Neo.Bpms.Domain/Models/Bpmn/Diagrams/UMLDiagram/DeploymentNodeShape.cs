using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class DeploymentNodeShape(DeploymentNodeShape.eType type, string id, string name, Bounds bounds, BaseModelClass node, DiagramElement owningElement) : Shape(id, name, bounds, node, owningElement)
	{
		public eType type = type;
		public enum eType
		{
			Node,
			DeviceNode,
			ExecutionEnvironmentNode,
		}
}
