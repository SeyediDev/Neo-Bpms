using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal partial class BpmnDiagramXmlConvertor
{
    private void DrawFlowElement(dynamic bpmnPlane)
    {
        foreach (var notdrawnNode in _flowNodeSymbols.Values.Where(CheckNotDrawElements).ToList())
        {
            Draw(bpmnPlane, notdrawnNode, _flowNodeSymbols);
        }
    }
    private static bool CheckNotDrawElements(FlowNodeSymbol drawflowNodeSymbol)
    {
        if (drawflowNodeSymbol == null) return false;
        return drawflowNodeSymbol.SetInDiagram && !drawflowNodeSymbol.Draw && (drawflowNodeSymbol.Incoming.Count < 1 ||
                                                        drawflowNodeSymbol.Incoming.Any(f => f.SetInDiagram && !drawflowNodeSymbol.Draw));
    }

    private void Draw(dynamic bpmnPlane, FlowNodeSymbol diagramFlowNode, Dictionary<string, FlowNodeSymbol> diagramFlowNodes)
    {
        diagramFlowNode.Draw = true;
        var bpmnShape = bpmnPlane.BPMNShape();
        AddBpmnDiagramNameSpace(bpmnShape);
        bpmnShape.id = diagramFlowNode.FlowNode.flowElementType + "_" + diagramFlowNode.FlowNode.Id;
        bpmnShape.bpmnElement = diagramFlowNode.FlowNode.Id;
        var bounds = bpmnShape.Bounds();
        AddBpmnDiagramBoundsNameSpace(bounds);
        bounds.x = diagramFlowNode.XDiagramPosition = (diagramFlowNode.Lane?.XCoefficient ?? 1) + (diagramFlowNode.XCoefficient * ElementDistance);
        switch (diagramFlowNode.FlowNode.flowElementType)
        {
            case FlowElement.eFlowElementType.Event:
                bounds.height = diagramFlowNode.Height;
                bounds.width = diagramFlowNode.Width;
                if (DrawBoundaryEvent(diagramFlowNode, bounds)) return;
                bounds.y = diagramFlowNode.YDiagramPosition = GetFlowNodeYFromLane(diagramFlowNode) + EventY;
                break;
            case FlowElement.eFlowElementType.Activity:
                bounds.height = diagramFlowNode.Height;
                bounds.width = diagramFlowNode.Width;
                bounds.y = diagramFlowNode.YDiagramPosition = GetFlowNodeYFromLane(diagramFlowNode) + ActivityY;
                break;
            case FlowElement.eFlowElementType.Gateway:
                bounds.height = diagramFlowNode.Height;
                bounds.width = diagramFlowNode.Width;
                bounds.y = diagramFlowNode.YDiagramPosition = GetFlowNodeYFromLane(diagramFlowNode) + GatewayY;
                break;
        }
    }

    private bool DrawBoundaryEvent(FlowNodeSymbol diagramFlowNode, dynamic bounds)
    {
        if (diagramFlowNode.FlowNode is BoundaryEvent)
        {
            bounds.x = diagramFlowNode.XDiagramPosition += 80;
            if (diagramFlowNode.YCoefficient > 1)
            {
                bounds.y = diagramFlowNode.YDiagramPosition =
                    GetFlowNodeYFromLane(diagramFlowNode) + EventY + (diagramFlowNode.YCoefficient - 1) * Height - 35;
            }
            else
                bounds.y = diagramFlowNode.YDiagramPosition = GetFlowNodeYFromLane(diagramFlowNode) + EventY - 35;

            return true;
        }

        return false;
    }
}
