using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams;

public class BPMNPlane(BaseElement modelElement, string name, BPMNDiagram owningDiagram) : DiagramElement(modelElement.Id, name, modelElement, owningDiagram)
{
    public BaseElement bpmnElement => ModelElement as BaseElement;
}
