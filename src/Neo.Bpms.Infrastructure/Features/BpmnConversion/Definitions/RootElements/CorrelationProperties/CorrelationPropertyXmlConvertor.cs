using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements.CorrelationProperties;

internal static class CorrelationPropertyXmlConvertor
{
    internal static void Export(dynamic bpmnElement)
    {
        foreach (var correlationProperty in ProjectDefinition.Project.BpmnDefinitions.GetRootElements()
                                                ?.OfType<CorrelationProperty>() ?? [])
        {
            var element = bpmnElement.correlationPropertyRef();
            BaseElementXmlConvertor.Export(element, correlationProperty);
            element.name = correlationProperty.Name;
            element.type = correlationProperty.type;
            CorrelationPropertyRetrievalExpressionXmlConvertor.Export(element, correlationProperty);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("correlationProperty") ?? Enumerable.Empty<ElasticObject>())
        {
            var correlationProperty =
                new CorrelationProperty(ProjectDefinition.Project.BpmnDefinitions, "", element.GetString("name"),
                    element.GetString("type"));
            BaseElementXmlConvertor.Import(element, correlationProperty);
            CorrelationPropertyRetrievalExpressionXmlConvertor.Import(element, correlationProperty);

            ProjectDefinition.Project.BpmnDefinitions.AddRootElement(correlationProperty);
        }
    }

    internal static void Export(dynamic correlationKeyElement, CorrelationKey correlationKey)
    {
        foreach (var correlationProperty in correlationKey.correlationPropertyRef ?? Enumerable.Empty<CorrelationProperty>())
        {
            var element = correlationKeyElement.correlationPropertyRef();
            BaseElementXmlConvertor.ExportRef(element, correlationProperty);
        }
    }

    internal static void Import(ElasticObject correlationKeyElement, CorrelationKey correlationKey)
    {
        correlationKey.correlationPropertyRef = null;
        foreach (var element in correlationKeyElement.GetElements("correlationPropertyRef") ??
                                Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id") ?? element.InternalValue?.ToString();
            var correlationProperty = ProjectDefinition.Project.BpmnDefinitions.GetRootElement(id) as CorrelationProperty;
            correlationKey.correlationPropertyRef ??= [];
            correlationKey.correlationPropertyRef.Add(correlationProperty);
        }
    }
}
