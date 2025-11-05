using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams;

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