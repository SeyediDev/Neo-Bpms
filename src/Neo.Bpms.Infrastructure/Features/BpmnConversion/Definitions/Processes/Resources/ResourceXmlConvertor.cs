namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Resources;

internal static class ResourceXmlConvertor
{
    internal static void Export(dynamic bpmn)
    {
        foreach (var resource in ProjectDefinition.Project.Resources ?? [])
        {
            var node = bpmn.resource();
            ExportResource(node, resource);
        }
    }

    internal static void ExportResource(dynamic resourceElement, Resource resource)
    {
        RootElementXmlConvertor.Export(resourceElement, resource);
        resourceElement.name = resource.Name;
        ResourceParameterXmlConvertor.Export(resourceElement, resource);

        resourceElement.namespaceId = resource.NamespaceId;
        resourceElement.entityId = resource.EntityId;
    }

    internal static void Import(ElasticObject bpmn)
    {
        foreach (var resourceNode in bpmn.GetElements("resource") ?? Enumerable.Empty<ElasticObject>())
        {
            ImportResource(resourceNode);
        }
    }

    private static void ImportResource(ElasticObject element)
    {
        var id = element.GetString("id");
        var resource = ProjectDefinition.Project.GetResource(id);

        //if (resource != null) return;
        var namespaceId = element.GetString("namespaceId");
        var entityId = element.GetString("entityId");
        var name = element.GetString("name");
        if (resource != null)
        {
            resource.Name = name;
            resource.NamespaceId = namespaceId;
            resource.EntityId = entityId;
        }
        else
        {
            resource = ProjectDefinition.Project.AddResource(id, name, namespaceId, entityId);
        }
        RootElementXmlConvertor.Import(element, resource);
        ResourceParameterXmlConvertor.Import(resource, element);
    }
}
