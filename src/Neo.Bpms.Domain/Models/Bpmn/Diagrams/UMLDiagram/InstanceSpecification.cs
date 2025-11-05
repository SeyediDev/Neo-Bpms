using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;


//using Neo.Bpms.Domain.Model.Base.UMLClassesKernel;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class InstanceSpecificationShape(string id, string name, Bounds bounds, InstanceSpecification instance, DiagramElement owningElement) : Shape(id, name, bounds, instance, owningElement)
	{
}
public class InstanceSpecification(BaseModelClass entity, string id, string name) : BaseModelClass(entity, id, name)
	{
    public BaseClass Entity => Parent;
		public List<SlotValue> slotValues = [];
	}
	public class SlotValue(BaseClass field, object value)
{
		public BaseClass field = field;
		public object value = value;
}
