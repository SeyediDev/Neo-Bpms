using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Correlations;

internal static class CorrelationPropertyBindingXmlConvertor
{
    internal static void Export(dynamic correlationSubscriptionElement, CorrelationSubscription correlationSubscription)
    {
        foreach (var correlationPropertyBinding in correlationSubscription.correlationPropertyBinding ?? Enumerable.Empty<CorrelationPropertyBinding>())
        {
            var element = correlationSubscriptionElement.correlationPropertyBinding();
            BaseElementXmlConvertor.Export(element, correlationPropertyBinding);
            element.correlationPropertyRef = correlationPropertyBinding.correlationPropertyRef?.Id;
            FormalExpressionXmlConvertor.Export(element.dataPath(), correlationPropertyBinding.dataPath);
        }
    }

    internal static void Import(ElasticObject correlationSubscriptionElement, CorrelationSubscription correlationSubscription)
    {
        correlationSubscription.correlationPropertyBinding = null;
        foreach (var element in correlationSubscriptionElement.GetElements("correlationPropertyBinding") ??
                                Enumerable.Empty<ElasticObject>())
        {
            var correlationPropertyRef = element.GetString("correlationPropertyRef");
            var correlationPropertyBinding = new CorrelationPropertyBinding(correlationSubscription, "",
                FormalExpressionXmlConvertor.Import(element.GetElement("dataPath")),
                correlationSubscription.correlationKeyRef?.correlationPropertyRef?.FirstOrDefault(p => p.Id == correlationPropertyRef));
            BaseElementXmlConvertor.Import(element, correlationPropertyBinding);

            correlationSubscription.correlationPropertyBinding ??= [];
            correlationSubscription.correlationPropertyBinding.Add(correlationPropertyBinding);
        }
    }
}
