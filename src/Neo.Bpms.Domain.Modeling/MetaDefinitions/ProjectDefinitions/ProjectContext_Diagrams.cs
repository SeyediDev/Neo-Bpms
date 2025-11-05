using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Model.Project;

public partial class ProjectContext
{
    public Dictionary<string, Diagram> Diagrams { get; } = [];

    public bool AddDiagram(Diagram diagram)
    {
        if (Diagrams.ContainsKey(diagram.Id)) return false;
        Diagrams.Add(diagram.Id, diagram);
        return true;
    }
}