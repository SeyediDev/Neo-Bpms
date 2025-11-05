//using System.Collections.Generic;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class ClassShape(ClassShape.Type type, string id, string name, Bounds bounds, BaseModelClass entity, DiagramElement owningElement) : Shape(id, name, bounds, entity, owningElement)
	{
		public Type type = type;
		public enum Type
		{
			Class,
			GenericClass,
			Interface,
			Enumeration,
			ORMPersistableClass,
			ORMAbstractPersistableClass,
			ORMUserTypeClass,
			ORMParametrizedTypeClass,
			EntityBean,
			Model,
			Entity,
			View,
			StoredProcedureResultSet,
			Object,
			Triggers,
			StoredProcedures,
			Sequence,
			CRCCard,
		}
}
public class ClassShapeItem(ClassShapeItem.Type type, string id, string name, BaseModelClass modelElement, ClassShape classShape) : Shape(id, name, classShape.bounds, modelElement, classShape)
	{
		public enum  Type
		{
			Field,//Attribute
			Property,//Attribute with getter and setter
			TypeDefinition,
			Constructor,
			Method,//Operation
			TemplateParameter,
			StoredProcedure,
			Trigger,
			SuperClass,
			SubClass,
			Responsibility
		}
		public Type type = type;
		public class ShapeItemParameter
		{
			public string type;
			public string name;
		}
		public List<ShapeItemParameter> parameters;
		public ShapeItemParameter AddParameter(ShapeItemParameter parameter)
		{
			if (parameters == null)
				parameters = [];
			parameters.Add(parameter);
			return parameter;
		}
}
