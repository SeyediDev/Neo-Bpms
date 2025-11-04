using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams;

public class BPMNLabel(string text, Bounds bounds, AlignmentKind alignment) 
    : Text(text, bounds, alignment)
{
}
