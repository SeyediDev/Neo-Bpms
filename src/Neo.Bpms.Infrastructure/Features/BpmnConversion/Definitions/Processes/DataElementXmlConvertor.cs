using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class DataElementXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic element, DataElement dataElement)
    {
        BaseElementXmlConvertor.Export(element, dataElement);
        ItemAwareElementXmlConvertor.Export(bpmnElement, element, dataElement);
        element.name = dataElement.Name;
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        ElasticObject element, DataElement dataElement)
    {
        BaseElementXmlConvertor.Import(element, dataElement);
        ItemAwareElementXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, dataElement);
        dataElement.Name = element.GetString("name");
    }
}
