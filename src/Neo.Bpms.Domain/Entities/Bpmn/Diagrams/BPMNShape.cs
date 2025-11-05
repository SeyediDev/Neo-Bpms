using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams;

public class BPMNShape(Bounds bounds, BaseElement modelElement, string name, 
    DiagramElement owningElement, BPMNLabel label) 
    : Shape(modelElement.Id, name, bounds, modelElement, owningElement)
{
    public BPMNLabel label = label;
    public bool isHorizontal;
    public bool isExpanded;
    public bool isMarkerVisible;
    public bool isMessageVisible;
    public eParticipantBandKind participantBandKind;
    public BPMNShape choreographyActivityShape;
    //public

    public enum eParticipantBandKind
    {
        top_initiating,
        middle_initiating,
        bottom_initiating,
        top_non_initiating,
        middle_non_initiating,
        bottom_non_initiating,
    }
}
