using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.DataFlows;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.DataStores;

/// <summary>
/// 14.0.
/// </summary>
internal static class ItemAwareElementXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic node, IItemAwareElement itemAwareElemet)
    {
        if (itemAwareElemet.itemSubjectRef != null)
        {
            ItemDefinitionXmlConvertor.Export(bpmnElement, itemAwareElemet.itemSubjectRef);
            node.itemSubjectRef = itemAwareElemet.itemSubjectRef.Id;
        }
        DataStateXmlConvertor.Export(node, itemAwareElemet.dataState);
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        ElasticObject node, IItemAwareElement itemAwareElemet)
    {
        var itemSubjectRef = node.GetString("itemSubjectRef");
        if (!string.IsNullOrEmpty(itemSubjectRef))
        {
            itemAwareElemet.itemSubjectRef = ItemDefinitionXmlConvertor.Import(
                bpmnDefinitions, bpmnElement, itemSubjectRef);
        }
        itemAwareElemet.dataState = DataStateXmlConvertor.Import(itemAwareElemet, node,
            itemAwareElemet.dataState);
    }

    internal static void ExportElementRef(dynamic element, IItemAwareElement itemAwareElemet)
    {
        BaseElementXmlConvertor.ExportRef(element, itemAwareElemet as BaseElement);
    }

    internal static IItemAwareElement ImportElementRef(BpmnDefinitions bpmnDefinitions,
        IItemAwareContainer container, ElasticObject element, bool fromInputItems)
    {
        if (element == null) return null;
        var id = element.InternalValue?.ToString() ?? "";
        var name = string.Empty;
        var i = id.IndexOf("teta.itemAware.", StringComparison.Ordinal);
        if (i >= 0)
            name = id[(i + "teta.itemAware.".Length)..];
        var item = container.GetItemAwareElement(id, name, fromInputItems);
        if (item == null)
            bpmnDefinitions.ErrorInfos.AddError($"Could not find item-aware-element {name}.",
                (container as BaseElement)?.Id, "14.0.0", "Could not find item aware element by name.");
        return item;
    }
}
