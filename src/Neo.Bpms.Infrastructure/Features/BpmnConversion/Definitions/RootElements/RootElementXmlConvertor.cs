using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements;

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
