using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams;

public class BPMNEdge(DiagramElement source, DiagramElement target, BaseElement modelElement,
    string name, DiagramElement owningElement, BPMNLabel label) : Edge(modelElement.Id, name, source, target, modelElement, owningElement)
{
    public BPMNLabel label = label;
    public eMessageVisibleKind messageVisibleKind;

    public enum eMessageVisibleKind
    {
        initiating,
        non_initiating,
    }
}