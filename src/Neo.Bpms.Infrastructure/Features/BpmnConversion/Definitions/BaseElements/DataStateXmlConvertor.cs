using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

internal static class DataStateXmlConvertor
{
    internal static void Export(dynamic node, DataState dataState)
    {
        if (dataState == null) return;
        var element = node.dataState();
        BaseElementXmlConvertor.Export(element, dataState);
        element.name = dataState.Name;
    }

    internal static DataState Import(IItemAwareElement parent, ElasticObject node, DataState dataState)
    {
        var element = node.GetElement("dataState");
        if (element == null) return null;
        dataState ??= new DataState(parent, "", "");
        BaseElementXmlConvertor.Import(element, dataState);
        dataState.Name = element.GetString("name");
        return dataState;
    }
}
