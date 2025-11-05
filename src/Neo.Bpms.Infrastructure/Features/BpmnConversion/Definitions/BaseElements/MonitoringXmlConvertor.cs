using Neo.Bpms.Domain.Models.Bpmn.Processes.Monitoring;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

internal static class MonitoringXmlConvertor
{
    internal static void Export(dynamic containerElement, IMonitoringContainer monitoringContainer)
    {
        if (monitoringContainer.monitoring == null) return;
        var element = containerElement.monitoring();
        BaseElementXmlConvertor.Export(element, monitoringContainer.monitoring);
    }

    internal static void Import(ElasticObject containerElement, IMonitoringContainer container)
    {
        var element = containerElement.GetElement("monitoring");
        if (element == null) return;
        container.monitoring = new Monitoring(container, element.GetString("id"));
        BaseElementXmlConvertor.Import(element, container.monitoring);
    }
}