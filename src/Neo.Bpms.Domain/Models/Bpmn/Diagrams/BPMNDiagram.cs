using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams;

public class BPMNDiagram(Bounds bounds, RootElement modelElement, string name) :
    DiagramInterchange.Diagram(modelElement.Id, name, bounds, modelElement, null)
{
    public void AddPlane(BPMNPlane element)
    {
        element.owningElement = this;
        ownedElements ??= [];
        ownedElements.Add(element);
    }
}
