using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams;

public class BPMNPlane(BaseElement modelElement, string name, BPMNDiagram owningDiagram) : DiagramElement(modelElement.Id, name, modelElement, owningDiagram)
{
    public BaseElement bpmnElement => ModelElement as BaseElement;
}
