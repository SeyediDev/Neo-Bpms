using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Interfaces;

internal static class InterfaceXmlConvertor
{
    internal static void Export(dynamic node)
    {
        foreach (var @interface in ProjectDefinition.Project.Interfaces ?? [])
        {
            var element = new ElasticObject("interface");
            node.AddElement(element);
            RootElementXmlConvertor.Export(element, @interface);
            element["name"] = @interface.Name;
            element["implementationRef"] = @interface.implementationRef;
            foreach (var operation in @interface.operations?.Values ?? Enumerable.Empty<Operation>())
                OperationXmlConvertor.Export(element, operation);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("interface") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var @interface = ProjectDefinition.Project.GetInterface(id);
            if (@interface != null) continue;

            var implementationRef = element.GetString("implementationRef");
            @interface = new Interface(null, id, element.GetString("name"), implementationRef);
            RootElementXmlConvertor.Import(element, @interface);
            OperationXmlConvertor.Import(element, @interface);
            ProjectDefinition.Project.AddInterface(@interface);
        }
    }
}
