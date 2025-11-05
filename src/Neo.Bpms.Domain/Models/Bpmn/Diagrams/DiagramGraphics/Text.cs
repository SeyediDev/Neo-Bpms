namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

	public class Text(string text, Bounds bounds, AlignmentKind alignment) : GraphicalElement
	{
    public string text = text;
		public Bounds bounds = bounds;
		public AlignmentKind alignment = alignment;
	}
