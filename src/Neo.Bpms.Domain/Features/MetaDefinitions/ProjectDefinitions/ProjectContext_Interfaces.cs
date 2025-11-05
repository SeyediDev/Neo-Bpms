namespace Neo.Bpms.Domain.Model.Project;

public partial class ProjectContext
{
    public ServiceInterfaceDefinition ServiceInterfaces { get; } =
        new ServiceInterfaceDefinition();
}