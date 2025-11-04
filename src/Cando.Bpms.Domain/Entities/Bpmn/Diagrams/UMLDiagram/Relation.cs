using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.UMLDiagram;

	public class Relation(Relation.eType type, string id, string name, DiagramElement source, DiagramElement target, BaseModelClass modelElement, DiagramElement owningElement) : Edge(id, name, source, target, modelElement, owningElement)
	{
		public eType type = type;
		public enum eType
		{
			Dependency,
			BindingDependency,
			Extends,
			Include,
			Generalization,
			Realization,
			Anchor,
			Constraint,
			Aggregation,
			Containment,
			Composition,
			Association,
			SelfAssociation,
			Abstraction,
			NArrayAssociation,
			Link,
			Represents,
			Occurrence,
			InstanceOf,
			GenericConnector,
			OneToOneRelationship,
			OneToManyRelationship,
			ManyToManyRelationship,
			Derive,
			Satisfy,
			Refine,
			Trace,
			Branch,
			SequenceFlow,
			MessageFlow,
			DataFlow,
			Annotation,
			Usage,

			Permission,
			Access,
			Import,
			Merge,
			Export,
			Instatution,
			Sbstitution,

			
			BusinessRuleAssociation,


			
			
			Line,
			LableLine
		}
}
