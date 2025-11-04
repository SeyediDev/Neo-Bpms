using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class FlowElementXmlConvertor
{
    internal static void Export(dynamic element, FlowElement flowElement)
    {
        BaseElementXmlConvertor.Export(element, flowElement);
        element.name = flowElement.Name;
        AuditingXmlConvertor.Export(element, flowElement);
        MonitoringXmlConvertor.Export(element, flowElement);
        // TODO
        // choreography
    }

    internal static void Import(ElasticObject element, FlowElement flowElement)
    {
        BaseElementXmlConvertor.Import(element, flowElement);
        flowElement.Name = element.GetString("name");
        AuditingXmlConvertor.Import(element, flowElement);
        MonitoringXmlConvertor.Import(element, flowElement);
        // TODO
        // choreography
    }
}
