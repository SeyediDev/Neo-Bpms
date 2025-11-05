using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal partial class BpmnDiagramXmlConvertor
{
    private void LocatingFlowElement(dynamic bpmnPlane)
    {
        //do while if incoming   for each ta tah rasm 
        //bad az do while any elemement draw = false ye ja part mikonim tah lanesh 
        do
        {
            var tries = 0;
            bool elementDrawed;
            do
            {
                tries++;
                SetFlowElementsLocation([.. _flowNodeSymbols.Values.Where(CheckNotLocatedElements)], _flowNodeSymbols);
                elementDrawed = _flowNodeSymbols.Values.Any(fn => !fn.Draw);
            } while (elementDrawed && tries < 10);
        } while (FindMissedFlowNodes(bpmnPlane, _flowNodeSymbols));
    }
    private static bool CheckNotLocatedElements(FlowNodeSymbol drawflowNodeSymbol)
    {
        if (drawflowNodeSymbol == null) return false;
        return !drawflowNodeSymbol.SetInDiagram && (drawflowNodeSymbol.Incoming.Count < 1 ||
                                                        drawflowNodeSymbol.Incoming.Any(f => f.SetInDiagram));
    }

    private void SetFlowElementsLocation(List<FlowNodeSymbol> notDrawnFlowNodes,
        Dictionary<string, FlowNodeSymbol> diagramFlowNodes)
    {
        if (notDrawnFlowNodes == null || notDrawnFlowNodes.Count == 0) return;
        SettingLocation(notDrawnFlowNodes.Where(ndf => ndf.FlowNode is StartEvent), diagramFlowNodes);
        SettingLocation(notDrawnFlowNodes.Where(ndf => !ndf.FlowNode.HasIncoming), diagramFlowNodes);
    }
    private void SettingLocation(IEnumerable<FlowNodeSymbol> notDrawnNodes,
        Dictionary<string, FlowNodeSymbol> diagramFlowNodes)
    {
        foreach (var notdrawnNode in notDrawnNodes)
        {
            SetLocation(notdrawnNode, diagramFlowNodes);
        }
    }
    private void SetLocation(FlowNodeSymbol notDrawnNode, Dictionary<string, FlowNodeSymbol> diagramFlowNodes)
    {
        if (SetBoundaryEventLocation(notDrawnNode, diagramFlowNodes)) return;
        notDrawnNode.SetInDiagram = true;
        notDrawnNode.XCoefficient = 1;
        foreach (var incomingFlow in notDrawnNode.Incoming)
        {
            notDrawnNode.XCoefficient = Math.Max(notDrawnNode.XCoefficient, incomingFlow.XCoefficient + 1);
        }
        switch (notDrawnNode.FlowNode.flowElementType)
        {
            case FlowElement.eFlowElementType.Event:
                notDrawnNode.Height = EventHeight;
                notDrawnNode.Width = EventWidth;
                break;
            case FlowElement.eFlowElementType.Activity:
                notDrawnNode.Height = ActivityHeight;
                notDrawnNode.Width = ActivityWidth;
                break;
            case FlowElement.eFlowElementType.Gateway:
                notDrawnNode.Height = GatewayHeight;
                notDrawnNode.Width = GatewayWidth;
                break;
            case FlowElement.eFlowElementType.ChoreographyActivity:
            case FlowElement.eFlowElementType.DataObject:
            case FlowElement.eFlowElementType.DataObjectRef:
            case FlowElement.eFlowElementType.DataStoreRef:
            case FlowElement.eFlowElementType.SequenceFlow:
                break;
        }
        var heightChanged = false;
        if (notDrawnNode.FlowNode is not BoundaryEvent)
        {
            notDrawnNode.YCoefficient = 1;
            foreach (var flowNode in notDrawnNode.Lane?.FlowNodeSymbols ?? Enumerable.Empty<FlowNodeSymbol>())
            {
                if (notDrawnNode.XCoefficient == flowNode.XCoefficient && notDrawnNode.YCoefficient == flowNode.YCoefficient &&
                     flowNode.FlowNode.Id != notDrawnNode.FlowNode.Id)
                {
                    notDrawnNode.YCoefficient++;
                    heightChanged = true;
                }
            }

            foreach (var incoming in notDrawnNode.Incoming ?? Enumerable.Empty<FlowNodeSymbol>())
            {
                if (notDrawnNode.Incoming.Any(i =>
                     i.YCoefficient == incoming.YCoefficient && i.FlowNode.Id != incoming.FlowNode.Id &&
                     i.Lane?.Id == incoming.Lane?.Id))
                {
                    incoming.YCoefficient = (long)notDrawnNode.Incoming?.Max(i => i.YCoefficient) + 1;
                    heightChanged = true;
                }
            }

            //foreach (var outgoing in notDrawnNode.Outgoing ?? Enumerable.Empty<FlowNodeSymbol>())
            //{
            //	if (notDrawnNode.Outgoing.Any(o =>
            //				o.Y == outgoing.Y && o.Lane?.Id == outgoing.Lane?.Id &&
            //				o.FlowNode.Id != outgoing.FlowNode.Id && o.X < notDrawnNode.X))
            //	{
            //		outgoing.Y = (long)notDrawnNode.Outgoing?.Max(i => i.Y) + 1;
            //		heightChanged = true;
            //	}
            //}//todo why and solve it

            var sameLaneIncommings =
                 notDrawnNode.Incoming?.Where(i => notDrawnNode.Lane?.Id == i.Lane?.Id).ToList();
            if (sameLaneIncommings.Count > 1)
            {
                notDrawnNode.YCoefficient = Math.Max(notDrawnNode.YCoefficient, sameLaneIncommings.Max(s => s.YCoefficient));
            }
            if (heightChanged)
            {
                foreach (var flowNode in notDrawnNode.Lane?.FlowNodeSymbols ?? Enumerable.Empty<FlowNodeSymbol>())
                {
                    if (notDrawnNode.Lane != null)
                        notDrawnNode.Lane.Height = Math.Max(notDrawnNode.Lane.Height, flowNode.YCoefficient);
                }
                var lanesYChanges = (notDrawnNode.Lane?.Height ?? 1) - 1;
                foreach (var lane in _laneDiagrams.Where(l => l.YCoefficient > (notDrawnNode.Lane?.YCoefficient ?? 1)))
                {
                    lane.YCoefficient += lanesYChanges;
                }
            }
        }
        var outNotDrawFlowNodes = notDrawnNode.Outgoing?.Where(CheckNotLocatedElements).ToList();
        SettingLocation(outNotDrawFlowNodes, diagramFlowNodes);
    }

    private static bool SetBoundaryEventLocation(FlowNodeSymbol notDrawnNode, Dictionary<string, FlowNodeSymbol> diagramFlowNodes)
    {
        if (notDrawnNode.FlowNode is not BoundaryEvent boundaryEvent) return false;
        var attachedToRef = boundaryEvent.attachedToRef;
        if (string.IsNullOrEmpty(attachedToRef)) return false;
        var flowNode = diagramFlowNodes.Values.FirstOrDefault(f => f.FlowNode.Id == attachedToRef);
        if (flowNode?.SetInDiagram == true)
        {
            notDrawnNode.SetInDiagram = true;
            notDrawnNode.XCoefficient = flowNode.XCoefficient;
            notDrawnNode.YCoefficient = flowNode.YCoefficient;
            notDrawnNode.Height = EventHeight;
            notDrawnNode.Width = EventWidth;
            return true;
        }
        return false;
    }
}
