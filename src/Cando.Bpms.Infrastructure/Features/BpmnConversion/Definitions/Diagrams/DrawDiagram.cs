using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal partial class BpmnDiagramXmlConvertor
{
    private const int PaticipantXValue = 75;
    private const int PaticipantYValue = 5;
    private const int LaneXValue = 105;
    private const int Height = 160;
    private const int DefaultLaneWidth = 600;
    private const int WidthCoefficient = 50;

    private void LocatingLanes()
    {
        foreach (var lane in _laneDiagrams.OrderBy(l => l.Index))
        {
            lane.XCoefficient = 1;
            lane.YCoefficient = lane.Index;
            lane.Height = Math.Max(lane.FlowNodeSymbols.Max(f => f.YCoefficient), 1);
            lane.Width = Math.Max(lane.FlowNodeSymbols.Max(f => f.XCoefficient), 1);
        }
    }

    private void DrawLanesDiagram(dynamic bpmnPlane)
    {
        var currentLaneY = 0L;
        foreach (var lane in _laneDiagrams.OrderBy(l => l.Index))
        {
            var bpmnShape = bpmnPlane.BPMNShape();
            AddBpmnDiagramNameSpace(bpmnShape);
            bpmnShape.id = "Lane_" + lane.Id;
            bpmnShape.bpmnElement = lane.Id;
            var bounds = bpmnShape.Bounds();
            AddBpmnDiagramBoundsNameSpace(bounds);
            bounds.x = lane.XCoefficient * LaneXValue;
            bounds.y = (currentLaneY * Height) + PaticipantYValue;
            currentLaneY += lane.Height;
            bounds.height = lane.Height * Height;
            bounds.width = LaneMaxWidth;
        }
    }
    private void ParticipantDiagram(dynamic bpmnPlane, Participant participant)
    {
        var bpmnShape = bpmnPlane.BPMNShape();//todo Process that have more than one Participant make problem
        bpmnShape.id = "ParticipantShape" + participant.Id;
        bpmnShape.bpmnElement = participant.Id;
        AddBpmnDiagramNameSpace(bpmnShape);
        var bounds = bpmnShape.Bounds();
        bounds.x = PaticipantXValue;
        bounds.y = PaticipantYValue;
        bounds.height = _laneDiagrams.Sum(l => l.Height) * Height;
        bounds.width = LaneMaxWidth + 30;
        AddBpmnDiagramBoundsNameSpace(bounds);
    }

    private long LaneMaxWidth
    {
        get { return Math.Max(_laneDiagrams.Max(l => l.Width) * 130, 600); }
    }

    private long GetFlowNodeYFromLane(FlowNodeSymbol diagramFlowNode)
    {
        return (new SequenceDraw(_laneDiagrams, null, null, null)
                    .CalcParticipantYPosition(diagramFlowNode) - 1) * Height;
    }
}
