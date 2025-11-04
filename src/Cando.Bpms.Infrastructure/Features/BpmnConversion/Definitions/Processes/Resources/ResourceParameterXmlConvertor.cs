using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Resources;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class ResourceParameterXmlConvertor
{
    internal static void Export(dynamic resourcenode, Resource resource)
    {
        foreach (var resourceParameter in resource.resourceParameters ?? Enumerable.Empty<ResourceParameter>())
        {
            var element = resourcenode.resourceParameter();
            BaseElementXmlConvertor.Export(element, resourceParameter);
            element.name = resourceParameter.Name;
            element.type = resourceParameter.type;
            element.isRequired = resourceParameter.isRequired ? "true" : "false";
        }
    }

    internal static void Import(Resource resource, ElasticObject node)
    {
        resource.resourceParameters = [];
        foreach (var element in node.GetElements("resourceParameter") ?? Enumerable.Empty<ElasticObject>())
        {
            ResourceParameter.eType type;
            Enum.TryParse(element.GetString("type"), out type);
            var resourceParameter = new ResourceParameter(resource,
                element.GetString("id"), element.GetString("name"), type,
                element.GetBool("isRequired"));
            BaseElementXmlConvertor.Import(element, resourceParameter);
            resource.resourceParameters.Add(resourceParameter);
        }
    }
}