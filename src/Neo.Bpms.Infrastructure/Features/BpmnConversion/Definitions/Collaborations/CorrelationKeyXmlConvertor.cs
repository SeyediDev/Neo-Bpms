using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements.CorrelationProperties;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Collaborations;

internal static class CorrelationKeyXmlConvertor
{
    internal static void Export(dynamic correlationKeyContainerElement, ICorrelationKeyContainer correlationKeyContainer)
    {
        foreach (var correlationKey in correlationKeyContainer.correlationKeys ?? Enumerable.Empty<CorrelationKey>())
        {
            var element = correlationKeyContainerElement.correlationKey();
            BaseElementXmlConvertor.Export(element, correlationKey);
            element.name = correlationKey.Name;
            CorrelationPropertyXmlConvertor.Export(element, correlationKey);
        }
    }

    internal static void Import(ElasticObject correlationKeyContainerElement,
        ICorrelationKeyContainer correlationKeyContainer)
    {
        correlationKeyContainer.correlationKeys = null;
        foreach (var element in correlationKeyContainerElement.GetElements("correlationKey") ??
                                Enumerable.Empty<ElasticObject>())
        {
            var correlationKey = new CorrelationKey(correlationKeyContainer, "", element.GetString("name"));
            BaseElementXmlConvertor.Import(element, correlationKey);
            CorrelationPropertyXmlConvertor.Import(element, correlationKey);

            correlationKeyContainer.correlationKeys ??= [];
            correlationKeyContainer.correlationKeys.Add(correlationKey);
        }
    }
}
