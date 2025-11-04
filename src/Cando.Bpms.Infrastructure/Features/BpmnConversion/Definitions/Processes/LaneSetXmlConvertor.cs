using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class LaneSetXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, dynamic process, IFlowElementsContainer flowElementsContainer)
    {
        foreach (var laneSet in flowElementsContainer.laneSets ?? Enumerable.Empty<LaneSet>())
        {
            var node = process.laneSet();
            ExportLaneSet(bpmnDefinitions, laneSet, node);
        }
    }

    internal static void ExportLaneSet(BpmnDefinitions bpmnDefinitions, LaneSet laneSet, dynamic node)
    {
        BaseElementXmlConvertor.Export(node, laneSet);
        node.name = laneSet.Name;
        if (laneSet.parentLane != null)
            node.parentLane = laneSet.parentLane.Id;
        LaneXmlConvertor.Export(bpmnDefinitions, node, laneSet);
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions,
        ElasticObject processElement, IFlowElementsContainer flowElementsContainer, Lane parentLane)
    {
        var laneSets = flowElementsContainer.laneSets;
        foreach (var element in processElement.GetElements("laneSet") ?? Enumerable.Empty<ElasticObject>())
            ImportLaneSet(bpmnDefinitions, flowElementsContainer, parentLane, ref laneSets, element);
        flowElementsContainer.laneSets = laneSets;
    }

    internal static void ImportLaneSet(BpmnDefinitions bpmnDefinitions, IFlowElementsContainer flowElementsContainer, Lane parentLane, ref List<LaneSet> laneSets, ElasticObject element)
    {
        laneSets ??= [];
        var id = element.GetString("id");
        if (string.IsNullOrEmpty(id)) return;
        var name = element.GetString("name");
        var laneSet = laneSets.FirstOrDefault(ls => ls.Id == id);
        if (laneSet == null)
        {
            laneSet = new LaneSet(flowElementsContainer, id, name, parentLane);
        }
        else
        {
            laneSet.Name = name;
            laneSet.parentLane = parentLane;
        }
        BaseElementXmlConvertor.Import(element, laneSet);
        LaneXmlConvertor.Import(bpmnDefinitions, flowElementsContainer, laneSet, element);
    }
}