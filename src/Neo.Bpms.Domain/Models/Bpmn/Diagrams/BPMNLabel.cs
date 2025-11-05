using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams;

public class BPMNLabel(string text, Bounds bounds, AlignmentKind alignment) 
    : Text(text, bounds, alignment)
{
}
