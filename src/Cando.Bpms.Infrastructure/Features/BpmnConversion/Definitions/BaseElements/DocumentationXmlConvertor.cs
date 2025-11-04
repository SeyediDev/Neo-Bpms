using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class DocumentationXmlConvertor
{
    internal static void Export(dynamic node, BaseElement baseElement, string @namespace)
    {
        foreach (var documentation in baseElement?.BaseElementDocumentation ?? Enumerable.Empty<Documentation>())
        {
            var element = node.documentation();
            element.id = documentation.id;
            element.text = documentation.text;
            element.textFormat = documentation.textFormat;
            BaseElementXmlConvertor.NamespaceExport(element, @namespace);
        }
    }

    internal static void Import(ElasticObject node, BaseElement baseElement)
    {
        foreach (var element in node?.GetElements("documentation") ?? Enumerable.Empty<ElasticObject>())
        {
            baseElement.BaseElementDocumentation ??= [];
            baseElement.BaseElementDocumentation.Add(new Documentation(
                element.GetString("id"),
                element.GetString("text"),
                element.GetString("textFormat")));
        }
    }
}
