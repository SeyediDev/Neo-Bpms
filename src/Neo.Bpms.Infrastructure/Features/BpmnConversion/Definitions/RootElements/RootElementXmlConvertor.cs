using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class RootElementXmlConvertor
{
    internal static void Export(dynamic node, RootElement rootElement)
    {
        BaseElementXmlConvertor.Export(node, rootElement);
    }

    internal static void Import(ElasticObject element, RootElement rootElement)
    {
        BaseElementXmlConvertor.Import(element, rootElement);
    }
}
