namespace Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions.Extensions;

public static class EntityExtensions
{
    public static bool HasProcess(this Entity entity)
    {
        return ProjectDefinition.Project?.BusinessProcesses?.Values.Any(bp =>
            bp?.Versions.Any(bpv => bpv.Value?.BpmnDefinitions?.Process?.EntityId == entity?.Id &&
                                   bpv.Value?.BpmnDefinitions?.Process?.EntityNamespaceId == entity?.NamespaceId) ?? false) ?? false;
    }
}