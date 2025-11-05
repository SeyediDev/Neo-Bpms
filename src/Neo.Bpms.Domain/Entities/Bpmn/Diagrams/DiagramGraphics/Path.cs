namespace Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;

public class Path : MarkedElement
{
    public Path(params PathCommand[] commands)
    {
        foreach (PathCommand command in commands)
        {
            this.commands.Add(command);
        }
    }
    public List<PathCommand> commands = [];
}
