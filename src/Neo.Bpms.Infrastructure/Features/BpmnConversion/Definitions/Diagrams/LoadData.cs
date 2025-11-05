using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal partial class BpmnDiagramXmlConvertor
{
    private void FillFlowNodeSymbols(Process processDef)
    {
        var processFlowElements = processDef.flowElements;
        foreach (var flowElement in processFlowElements.Values
            .Where(f =>
                f.flowElementType != FlowElement.eFlowElementType.SequenceFlow &&
                f.flowElementType != FlowElement.eFlowElementType.DataObject &&
                f.flowElementType != FlowElement.eFlowElementType.DataObjectRef &&
                f.flowElementType != FlowElement.eFlowElementType.DataStoreRef))
        {
            if (!_flowNodeSymbols.ContainsKey(flowElement.Id))
            {
                _flowNodeSymbols.Add(flowElement.Id, new FlowNodeSymbol { FlowNode = flowElement as FlowNode });
            }
        }

        foreach (var flowNodeSymbol in _flowNodeSymbols.Values)
        {
            flowNodeSymbol.Incoming = FetchIncomingSequenceToFlowNode(_flowNodeSymbols,
                flowNodeSymbol.FlowNode.incoming ?? Enumerable.Empty<SequenceFlow>());
            flowNodeSymbol.Outgoing = FetchOutgoingSequenceFromFlowNode(_flowNodeSymbols,
                flowNodeSymbol.FlowNode.outgoing ?? Enumerable.Empty<SequenceFlow>());
        }
    }
    private void FillLanesData(Process processDef)
    {
        foreach (var laneset in processDef.laneSets ?? Enumerable.Empty<LaneSet>())
        {
            foreach (var lane in laneset?.lanes ?? Enumerable.Empty<Lane>())
            {
                var l = new LaneSymbol
                {
                    Id = lane.Id,
                    Paste = false
                };
                var flowNodeRefs = lane.flowNodeRefs.Select(fn =>
                {
                    var flowNode = processDef.GetFlowNode(fn);
                    if (flowNode == null)
                    {
                        _bpmnDefinitions.ErrorInfos.AddWarning($"Could not find lane flowNodeRef {fn}", lane.Name, "14.8.1", "", eWarningLevel.WarningLevel0);
                        return null;
                    }

                    if (!_flowNodeSymbols.TryGetValue(fn, out var f))
                    {
                        _bpmnDefinitions.ErrorInfos.AddWarning($"Could not find lane flowNodeRef {fn}", lane.Name, "14.8.1", "", eWarningLevel.WarningLevel0);
                        return null;
                    }
                    f.Lane = l;
                    return f;
                }).ToList();
                l.FlowNodeSymbols = [.. flowNodeRefs.Where(fn => fn != null)];
                _laneDiagrams.Add(l);
            }
        }
    }
    private List<FlowNodeSymbol> FetchIncomingSequenceToFlowNode(Dictionary<string, FlowNodeSymbol> diagramFlowNodes, IEnumerable<SequenceFlow> sequences)
    {
        return [.. sequences.Select(sequence =>
        {
            {
                FlowNodeSymbol inFlowNodeSymbol;
                diagramFlowNodes.TryGetValue(
                    sequence.sourceRef.Id,
                    out inFlowNodeSymbol);
                return inFlowNodeSymbol;
            }
        })];
    }
    private List<FlowNodeSymbol> FetchOutgoingSequenceFromFlowNode(Dictionary<string, FlowNodeSymbol> diagramFlowNodes, IEnumerable<SequenceFlow> sequences)
    {
        return [.. sequences.Select(sequence =>
        {
            {
                FlowNodeSymbol outFlowNodeSymbol;
                diagramFlowNodes.TryGetValue(
                    sequence.targetRef?.Id ?? "",
                    out outFlowNodeSymbol);
                return outFlowNodeSymbol;
            }
        })];
    }
}
