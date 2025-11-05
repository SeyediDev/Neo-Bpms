using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Lanes;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Resources;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes;

internal static class LaneXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, dynamic laneSetElement, LaneSet laneSet)
    {
        foreach (var lane in laneSet.lanes ?? Enumerable.Empty<Lane>())
        {
            var element = laneSetElement.lane();
            BaseElementXmlConvertor.Export(element, lane);
            element.name = lane.Name;
            ExportPartition(lane, element);
            ExportChildLaneSet(bpmnDefinitions, lane, element);
            ExportFlowNodes(element, lane);
            var extensionNode = BaseElementXmlConvertor.ExportExtensionElements(element);
            ResourceRoleXmlConvertor.Export(bpmnDefinitions, extensionNode, lane);
        }
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions,
        IFlowElementsContainer flowElementsContainer, LaneSet laneSet, ElasticObject laneSetElement)
    {
        foreach (var element in laneSetElement.GetElements("lane") ?? Enumerable.Empty<ElasticObject>())
        {
            laneSet.lanes ??= [];
            var id = element.GetString("id");
            var name = element.GetString("name");
            var lane = laneSet.lanes.FirstOrDefault(l => l.Id == id);
            if (lane == null)
            {
                lane = new Lane(flowElementsContainer, id, name);
                laneSet.lanes.Add(lane);
            }
            else
                lane.Name = name;
            BaseElementXmlConvertor.Import(element, lane);
            ImportPartition(element, lane);
            ImportChildLaneSet(bpmnDefinitions, flowElementsContainer, element, lane);
            ImportFlowNodes(element, lane);
            var extensionNode = BaseElementXmlConvertor.ImportExtensionElementsNode(element);
            if (extensionNode != null)
                ResourceRoleXmlConvertor.Imports(bpmnDefinitions, extensionNode, lane);
        }
    }

    private static void ExportChildLaneSet(BpmnDefinitions bpmnDefinitions, Lane lane, dynamic node)
    {
        if (lane.childLaneSet == null) return;
        var childLaneSet = node.childLaneSet();
        LaneSetXmlConvertor.ExportLaneSet(bpmnDefinitions, lane.childLaneSet, childLaneSet);
    }

    private static void ImportChildLaneSet(BpmnDefinitions bpmnDefinitions,
        IFlowElementsContainer flowElementsContainer, ElasticObject element, Lane lane)
    {
        var childLaneSetElement = element.GetElement("childLaneSet");
        if (childLaneSetElement == null) return;
        var laneSets = new List<LaneSet> { lane.childLaneSet };
        LaneSetXmlConvertor.ImportLaneSet(bpmnDefinitions, flowElementsContainer, lane, ref laneSets, childLaneSetElement);
        lane.childLaneSet = laneSets?.FirstOrDefault();
    }

    private static void ExportPartition(Lane lane, dynamic node)
    {
        if (lane.partitionElement != null)
        {
            var partitionElement = node.partitionElement();
            BaseElementXmlConvertor.ExportRef(partitionElement, lane.partitionElement);
        }
        if (lane.partitionElementRef != null)
            node.partitionElementRef = lane.partitionElementRef;
    }

    private static void ImportPartition(ElasticObject element, Lane lane)
    {
        var partitionElementElement = element.GetElement("partitionElement");
        if (partitionElementElement != null)
        {
            // todo 
            // lane.partitionElement = new BaseElement(lane, partitionElementElement.GetString("id"));
            // BaseElementXmlConvertor.Import(partitionElementElement, lane.partitionElement);
        }
        lane.partitionElementRef = element.GetString("partitionElementRef");
    }

    private static void ExportFlowNodes(dynamic laneNode, Lane lane)
    {
        foreach (var flowNodeRef in lane.flowNodeRefs ?? Enumerable.Empty<string>())
        {
            var node = laneNode.flowNodeRef();
            //node.id = flowNodeRef;
            node.InternalValue = flowNodeRef;
        }
    }

    private static void ImportFlowNodes(ElasticObject laneElement, Lane lane)
    {
        lane.flowNodeRefs = [];
        foreach (var element in laneElement.GetElements("flowNodeRef") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.InternalValue?.ToString();
            if (string.IsNullOrEmpty(id))
                id = element.GetString("id");
            lane.flowNodeRefs.Add(id);
        }
    }
}