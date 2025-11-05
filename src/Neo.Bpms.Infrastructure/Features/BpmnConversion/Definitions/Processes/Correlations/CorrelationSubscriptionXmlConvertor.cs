using Neo.Bpms.Domain.Models.Bpmn.Collaborations;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Correlations;

internal static class CorrelationSubscriptionXmlConvertor
{
    internal static void Export(dynamic processElement, Process process)
    {
        foreach (var correlationSubscription in process.correlationSubscriptions ?? Enumerable.Empty<CorrelationSubscription>())
        {
            var element = processElement.correlationSubscription();
            BaseElementXmlConvertor.Export(element, correlationSubscription);
            element.correlationKeyRef = correlationSubscription.correlationKeyRef?.Id;
            CorrelationPropertyBindingXmlConvertor.Export(element, correlationSubscription);
        }
    }

    internal static void Import(ElasticObject processElement, Process process)
    {
        process.correlationSubscriptions = null;
        foreach (var element in processElement.GetElements("correlationSubscription") ??
                                Enumerable.Empty<ElasticObject>())
        {
            var correlationSubscription = new CorrelationSubscription(process, "",
                GetCorrelationKey(process, element.GetString("correlationKeyRef")));
            BaseElementXmlConvertor.Import(element, correlationSubscription);
            CorrelationPropertyBindingXmlConvertor.Import(element, correlationSubscription);

            process.correlationSubscriptions ??= [];
            process.correlationSubscriptions.Add(correlationSubscription);
        }
    }

    private static CorrelationKey GetCorrelationKey(Process process, string correlationKey)
    {
        var collaboration = process.definitionalCollaborationRef ??
            process.BpmnDefinitions.GetRootElements()?.OfType<Collaboration>()
                .FirstOrDefault(c => c.participants.Any(p => p.processRef?.Id == process.Id));
        return collaboration?.correlationKeys?.FirstOrDefault(k => k.Id == correlationKey);
    }
}
