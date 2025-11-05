using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes;

internal static class FlowNodeXmlConvertor
{
    internal static void Export(dynamic node, FlowNode flowNode)
    {
        FlowElementXmlConvertor.Export(node, flowNode);
        IncommingExport(node, flowNode);
        OutgoingExport(node, flowNode);

        // BaseElementXmlConvertor.AddExtensionExport(node, "inputStateId", flowNode.inputStateId);
        if (flowNode.inputStateId > 0)
            node.inputStateId = flowNode.inputStateId;

        // BaseElementXmlConvertor.AddExtensionExport(node, "outputStateId", flowNode.outputStateId);
        if (flowNode.outputStateId > 0)
            node.outputStateId = flowNode.outputStateId;
    }

    internal static void Import(ElasticObject element, FlowNode flowNode)
    {
        FlowElementXmlConvertor.Import(element, flowNode);
        flowNode.inputStateId = element.GetInteger("inputStateId", 0);
        //flowNode.inputStateId = ToInt32(BaseElementXmlConvertor.ImportExtensionItem(element, "inputStateId"));
        flowNode.outputStateId = element.GetInteger("outputStateId", 0);
        //flowNode.outputStateId = ToInt32(BaseElementXmlConvertor.ImportExtensionItem(element, "outputStateId"));
        //incomming not needed to load
        //outgoing not needed to load
    }

    private static void OutgoingExport(dynamic obj, FlowNode flowNode)
    {
        foreach (var sequenceFlow in flowNode.outgoing ?? Enumerable.Empty<SequenceFlow>())
        {
            var sequenceFlowNode = obj.outgoing();
            BaseElementXmlConvertor.ExportRef(sequenceFlowNode, sequenceFlow);
        }
    }

    private static void IncommingExport(dynamic obj, FlowNode flowNode)
    {
        foreach (var sequenceFlow in flowNode.incoming ?? Enumerable.Empty<SequenceFlow>())
        {
            var sequenceFlowNode = obj.incoming();
            BaseElementXmlConvertor.ExportRef(sequenceFlowNode, sequenceFlow);
        }
    }
}