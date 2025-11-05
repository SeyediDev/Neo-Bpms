//using System;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

	public enum eDiagramType
	{
		UML_UseCaseDiagram = 11,
		UML_ComponentDiagram,
		UML_DeploymentDiagram,
		UML_PackageDiagram,
		ProcessMapDiagram,
		SysModel,
		SoaML,

		BPMN_PrivateNonExecutableProcess = 31,//PrivateNonExecutableBusinessProcess
		BPMN_PrivateExecutableProcess,//PrivateExecutableBusinessProcess
		BPMN_PublicProcesses,
		BPMN_Choreography,
		BPMN_Collaboration,

		UML_ActivityDiagram = 41,
		UML_StateMachineDiagram,
		UML_CommunicationDiagram,
		UML_SequenceDiagram,
		UML_TimingDiagram,
		UML_IntractionOverviewDiagram,
		EPCDiagram,
		ConversationDiagram,
		DataFlowDiagram,

		UML_ClassDiagram = 51,
		ObjectDiagram,
		CompositStructureDiagram,
		ClassHierarchy,

		EntityRelationDiagram = 61,
		ORMDiagram,

		TextualAnalysis = 71,
		RequirementDiagram,
		FactDiagram,
		ArchiMateDiagram,
		BasicDiagram,
		CRCCardDiagram,
		ConceptMap,

		WSDLDiagram = 81,

		ControlTree = 91,
		DOMTree,
		VisualTree,
		PageFlow,//InteractionFlow

		OrganisationChart = 101,
		BMM,

		//general diagrams, special purpose diagrams:
		OverviewDiagram = 201,
		UserInterface,
		MindMappingDiagram,
		Matrix,
		Grid,
		AnalysisDiagram,
		Profile,
		GraphicalTree,
		//Tree,
		//DecisionTree,
		//DoubleTree,
		//IncrementalTree,
		//Fishbone,
		PERT,
		Gantt,
		GrafcetDiagram,

	}
