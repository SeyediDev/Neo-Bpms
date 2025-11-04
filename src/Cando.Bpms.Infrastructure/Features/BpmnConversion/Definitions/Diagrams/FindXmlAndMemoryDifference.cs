using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal partial class BpmnDiagramXmlConvertor
{
    private static bool FindXmlAndMemoryDifference(Process processDef, ElasticObject diagramBpmnDefinition)
    {
        if (diagramBpmnDefinition?.GetElement("BPMNDiagram") == null) return true;
        var process = diagramBpmnDefinition.GetElement("process");
        var processLanes = new Dictionary<string, ProcessFlowElements>();
        var processFlowElements = new Dictionary<string, ProcessFlowElements>();
        foreach (var elementCollection in process.ElementCollections
            .Where(e => e.Key != "sequenceFlow"))
        {
            switch (elementCollection.Key)
            {
                case "extensionDefinitions":
                    break;
                case "laneSet":
                    SetLaneElementsIntoDictionary(elementCollection.Value, processLanes);
                    break;
                default:
                    SetElementsIntoDictionary(elementCollection.Value, processFlowElements);
                    break;
            }
        }
        var drawDiagram = FindLanesXmlAndMemoryDifference(processDef, processLanes);
        if (!drawDiagram)
            drawDiagram = FindElementsXmlAndMemoryDifference(processDef,
                processFlowElements);
        return drawDiagram;
    }

    private static void SetLaneElementsIntoDictionary(List<ElasticObject> elementCollections,
        IDictionary<string, ProcessFlowElements> processLanes)
    {
        foreach (var elementCollection in elementCollections ?? Enumerable.Empty<ElasticObject>())
        {
            foreach (var element in elementCollection.ElementCollections
                                        ?.Values.FirstOrDefault()
                                    ?? Enumerable.Empty<ElasticObject>())
            {
                var id = element.GetString("id");
                if (string.IsNullOrEmpty(id)) continue;
                processLanes.Add(id, new ProcessFlowElements { FlowElement = element });
            }
        }
    }

    private static void SetElementsIntoDictionary(List<ElasticObject> elementCollections,
        Dictionary<string, ProcessFlowElements> processFlowElements)
    {
        foreach (var element in elementCollections ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            if (string.IsNullOrEmpty(id)) continue;
            processFlowElements.Add(id, new ProcessFlowElements { FlowElement = element });
        }
    }

    private static bool FindElementsXmlAndMemoryDifference(Process processDef,
        Dictionary<string, ProcessFlowElements> processFlowElements)
    {
        bool drawDiagram = false;
        foreach (var element in processDef.flowElements.Values
            .Where(f => f.flowElementType != FlowElement.eFlowElementType.SequenceFlow))
        {
            ProcessFlowElements xmlElement;
            processFlowElements.TryGetValue(element.Id, out xmlElement);
            if (xmlElement != null)
            {
                xmlElement.Find = true;
            }
            if (xmlElement == null)
            {
                drawDiagram = true;
                break;
            }
        }
        if (!drawDiagram)
        {
            var extraElement = processFlowElements?.FirstOrDefault(fe => fe.Value?.Find == false);
            if (extraElement == null) drawDiagram = true;
        }

        return drawDiagram;
    }

    private static bool FindLanesXmlAndMemoryDifference(Process processDef,
        Dictionary<string, ProcessFlowElements> processLanes)
    {
        bool drawDiagram = false;
        foreach (var lanes in processDef.laneSets ?? Enumerable.Empty<LaneSet>())
        {
            foreach (var lane in lanes.lanes ?? Enumerable.Empty<Lane>())
            {
                ProcessFlowElements xmlLane = null;
                processLanes?.TryGetValue(lane.Id, out xmlLane);
                if (xmlLane != null)
                {
                    xmlLane.Find = true;
                }
                if (xmlLane == null)
                {
                    drawDiagram = true;
                    break;
                }
            }
            if (!drawDiagram)
            {
                var extraElement = processLanes?.FirstOrDefault(fe => fe.Value?.Find == false);
                if (extraElement == null) drawDiagram = true;
            }
        }

        return drawDiagram;
    }

    private class ProcessFlowElements
    {
        public ElasticObject FlowElement { get; set; }
        public bool Find { get; set; }
    }
}
