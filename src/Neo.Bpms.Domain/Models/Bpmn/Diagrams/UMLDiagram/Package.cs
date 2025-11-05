using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

public class PackageShape(PackageShape.eType type, string id, string name, Bounds bounds, DiagramElement owningElement) : Shape(id, name, bounds, null, owningElement)
{
    public eType type = type;
    public enum eType
    {
        Pakage,
        OverviewDiagram,
        Subsystem,
        ORMContainer,
        Process,
        Document,
    }
}
public class PackageShapeItem(PackageShapeItem.eType type, string id, string name, 
    Bounds bounds, Diagram linkedDiagram, PackageShape package) 
    : Shape(id, name, bounds, null, package)
{
    public enum eType
    {
        SpecificationElement,//for subsystem
        RealizationElement,//for subsystem
        SubDiagram,//for package
        SubSystem,
        SubPackage,
    }
    public eType type = type;
    public Diagram linkedDiagram = linkedDiagram;
}
