using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class UseCase : Shape
	{
		public UseCase(int rank, Bounds bounds, BaseModelClass process, DiagramElement owningElement) :
			base(process.Id, process.Name, bounds, process, owningElement)
		{
			this.rank = rank;
		}
		public UseCase(int rank, string id, string name, Bounds bounds, DiagramElement owningElement) :
			base(id, name, bounds, null, owningElement)
		{
			this.rank = rank;
		}
		public int rank;
	}
