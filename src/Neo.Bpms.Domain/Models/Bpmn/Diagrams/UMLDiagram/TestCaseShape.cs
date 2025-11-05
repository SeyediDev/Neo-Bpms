using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

	public class TestCaseShape(string id, string name, Bounds bounds, BaseModelClass testCase, DiagramElement owningElement) : Shape(id, name, bounds, testCase, owningElement)
	{
    //public eType type;
    //public enum eType
    //{
    //	UnitTest,
    //	UIUnitTest,
    //	SystemTest,
    //	SystemIntegrationTest,
    //	PerformanceTest,
    //	StressTest,


}
