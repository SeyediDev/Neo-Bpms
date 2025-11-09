using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Activities;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Events;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes;

internal static class FlowElementsContainerXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, dynamic bpmnElement, dynamic element, IFlowElementsContainer flowElementsContainer)
    {
        ExportFlowElements(bpmnDefinitions, bpmnElement, flowElementsContainer, element);
        LaneSetXmlConvertor.Export(bpmnDefinitions, element, flowElementsContainer);
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IFlowElementsContainer flowElementsContainer, dynamic element)
    {
        ImportFlowElements(bpmnDefinitions, bpmnElement, flowElementsContainer, element);
        LaneSetXmlConvertor.Import(bpmnDefinitions, element, flowElementsContainer, null);
    }

    internal static void ExportFlowElements(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, IFlowElementsContainer flowElementsContainer,
        dynamic node)
    {
        foreach (var flowElement in flowElementsContainer.flowElements?.Values ?? Enumerable.Empty<FlowElement>())
        {
            ExportFlowElement(bpmnDefinitions, bpmnElement, node, flowElement);
        }
    }

    private static void ExportFlowElement(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic node, FlowElement flowElement)
    {
        switch (flowElement.flowElementType)
        {
            case FlowElement.eFlowElementType.Gateway:
                GatewayXmlConvertor.Export(node, flowElement);
                break;
            case FlowElement.eFlowElementType.Activity:
                ActivityXmlConvertor.Export(bpmnDefinitions, bpmnElement, node, flowElement);
                break;
            case FlowElement.eFlowElementType.Event:
                EventXmlConvertor.Export(bpmnDefinitions, bpmnElement, node, flowElement);
                break;
            case FlowElement.eFlowElementType.ChoreographyActivity:
                ChoreographyActivityXmlConvertor.Export(node, flowElement);
                break;
            case FlowElement.eFlowElementType.DataObject:
            case FlowElement.eFlowElementType.DataObjectRef:
            case FlowElement.eFlowElementType.DataStoreRef:
                DataFlowElementXmlConvertor.Export(bpmnElement, node, flowElement as DataFlowElement);
                break;
            case FlowElement.eFlowElementType.SequenceFlow:
                SequenceFlowXmlConvertor.Export(node, flowElement);
                break;
        }
    }

    private static void ImportFlowElements(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IFlowElementsContainer flowElementsContainer, ElasticObject node)
    {
        var seenElementsIds = new Dictionary<string, BaseElement>();

        foreach (var elements in node.ElementCollections ?? Enumerable.Empty<KeyValuePair<string, List<ElasticObject>>>())
        {
            if (elements.Key == "sequenceFlow") continue;

            foreach (var elementNode in elements.Value)
            {
                var id = elementNode.GetString("id");
                if (string.IsNullOrEmpty(id)) continue; //todo throw exception
                flowElementsContainer.flowElements.TryGetValue(id, out var flowElement);

                // ReSharper disable once SuggestVarOrType_SimpleTypes
                FlowElement importedFlowElement = ActivityXmlConvertor.Import(bpmnDefinitions, bpmnElement,
                        flowElementsContainer, elements.Key, elementNode, ref flowElement, id)
                    ?? GatewayXmlConvertor.Import(flowElementsContainer,
                        elements.Key, elementNode, ref flowElement, id)
                    ?? EventXmlConvertor.Import(bpmnDefinitions, bpmnElement,
                        flowElementsContainer, elements.Key, elementNode, ref flowElement, id)
                    ?? DataFlowElementXmlConvertor.Import(bpmnDefinitions, bpmnElement,
                        flowElementsContainer, elements.Key, elementNode, ref flowElement, id)
                    ?? (FlowElement)ChoreographyActivityXmlConvertor.Import(flowElementsContainer,
                        elements.Key, elementNode, ref flowElement, id);
                if (importedFlowElement == null) continue;
                if (flowElement == null)
                    flowElementsContainer.flowElements.Add(id, importedFlowElement);
                seenElementsIds.Add(id, importedFlowElement);
            }
        }
        foreach (var elements in node.ElementCollections ?? Enumerable.Empty<KeyValuePair<string, List<ElasticObject>>>())
        {
            if (elements.Key != "sequenceFlow") continue;
            foreach (var elementNode in elements.Value)
            {
                var id = elementNode.GetString("id");
                if (string.IsNullOrEmpty(id)) continue; //todo throw exception
                flowElementsContainer.flowElements.TryGetValue(id, out var flowElement);

                var importedFlowElement = SequenceFlowXmlConvertor.Import(bpmnDefinitions,
                    flowElementsContainer, elements.Key, elementNode, ref flowElement, id);
                if (importedFlowElement == null) continue;
                if (flowElement == null)
                    flowElementsContainer.flowElements.Add(id, importedFlowElement);
                seenElementsIds.Add(id, importedFlowElement);
            }
        }

        var removeItems = new List<FlowElement>();
        removeItems.AddRange(flowElementsContainer.flowElements
                                                                .Where(flowElement => !seenElementsIds.ContainsKey(flowElement.Key))
                                                                .Select(f => f.Value));
        foreach (var flowElement in removeItems)
        {
            flowElementsContainer.flowElements.Remove(flowElement.Id);
            if (flowElement.flowElementType != FlowElement.eFlowElementType.SequenceFlow) continue;
            foreach (var flowElement2 in flowElementsContainer.flowElements.Values)
            {
                var fe = flowElement2 as FlowNode;
                foreach (var incoming in fe?.incoming.Where(f => f.Id == flowElement.Id).ToList() ?? Enumerable.Empty<SequenceFlow>())
                    fe?.incoming.Remove(incoming);
                foreach (var outgoing in fe?.outgoing.Where(f => f.Id == flowElement.Id).ToList() ?? Enumerable.Empty<SequenceFlow>())
                    fe?.outgoing.Remove(outgoing);
            }
        }
        foreach (var flowElement in flowElementsContainer.flowElements.Values.Where(fe => fe.flowElementType != FlowElement.eFlowElementType.SequenceFlow))
        {
            if (flowElement is not FlowNode flowNode) continue;
            flowNode.incoming = FetchIncomingSequenceToFlowNode(flowElementsContainer.flowElements, flowElement);
            flowNode.outgoing = FetchOutgoingSequenceFromFlowNode(flowElementsContainer.flowElements, flowElement);
        }
    }

    private static List<SequenceFlow> FetchOutgoingSequenceFromFlowNode(Dictionary<string, FlowElement> flowElements, FlowElement flowElement)
    {
        var outgoingList = new List<SequenceFlow>();
        foreach (var element in flowElements.Values.Where(fe => fe.flowElementType == FlowElement.eFlowElementType.SequenceFlow))
        {
            var sequenceFlow = element as SequenceFlow;
            if (sequenceFlow != null && sequenceFlow.sourceRef.Id != flowElement.Id) continue;
            outgoingList.Add(sequenceFlow);
        }
        return outgoingList;
    }

    private static List<SequenceFlow> FetchIncomingSequenceToFlowNode(Dictionary<string, FlowElement> flowElements, FlowElement flowElement)
    {
        var incomingList = new List<SequenceFlow>();
        foreach (var element in flowElements.Values.Where(fe => fe.flowElementType == FlowElement.eFlowElementType.SequenceFlow))
        {
            var sequenceFlow = element as SequenceFlow;
            if (sequenceFlow != null && sequenceFlow.targetRef.Id != flowElement.Id) continue;
            incomingList.Add(sequenceFlow);
        }
        return incomingList;
    }
}
