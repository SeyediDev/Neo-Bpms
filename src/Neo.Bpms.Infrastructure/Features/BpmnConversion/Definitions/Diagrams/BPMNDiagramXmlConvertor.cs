using Neo.Bpms.Domain.Models.Bpmn.Collaborations;
using Neo.Bpms.Domain.Models.Bpmn.Collaborations.PoolAndParticipant;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Diagrams;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

/// <summary>
/// 14.8
/// </summary>
internal partial class BpmnDiagramXmlConvertor(BpmnDefinitions bpmnDefinitions)
{
    private const int ElementDistance = 135;
    private const int EventY = 76;
    private const int ActivityY = 54;
    private const int GatewayY = 70;
    private const int EventHeight = 36;//todo delete
    private const int EventWidth = 36;
    private const int ActivityHeight = 80;
    private const int ActivityWidth = 100;
    private const int GatewayHeight = 50;
    private const int GatewayWidth = 50;

    internal readonly List<LaneSymbol> _laneDiagrams = [];
    internal readonly BpmnDefinitions _bpmnDefinitions = bpmnDefinitions;
    private readonly Dictionary<string, FlowNodeSymbol> _flowNodeSymbols = [];

    internal static void Export(dynamic node, BpmnDefinitions bpmnDefinitions,
         ElasticObject diagramBpmnDefinition)
    {
        try
        {
            var drawDefaultDiagram = FindXmlAndMemoryDifference(bpmnDefinitions.Process, diagramBpmnDefinition);
            if (drawDefaultDiagram)
            {
                var bpmnDiagram = new BpmnDiagramXmlConvertor(bpmnDefinitions);
                bpmnDiagram.DrawDefaultDiagram(node);
            }
            else
            {
                node.AddElement(diagramBpmnDefinition.GetElement("BPMNDiagram"));
            }
        }
        catch (Exception e)
        {
            bpmnDefinitions?.ErrorInfos.AddFatal(e.ToString(), "BpmnDefinitions" + bpmnDefinitions.Id, "14.8.0", "Exception");
        }
    }

    private void DrawDefaultDiagram(dynamic node)
    {
        var bpmnDiagram = node.BPMNDiagram();
        AddBpmnDiagramNameSpace(bpmnDiagram);
        bpmnDiagram.id = "BPMNDiagram" + (_bpmnDefinitions.Id ?? _bpmnDefinitions.Process.Id);//todo can a Process have two digrams ?		
        foreach (var collaboration in _bpmnDefinitions.GetRootElements()?.OfType<Collaboration>() ?? [])
        {
            CollaborationDiagram(bpmnDiagram, collaboration);
        }
        //_laneDiagrams.Clear();
    }

    private void CollaborationDiagram(dynamic bpmnDiagram, Collaboration collaboration)
    {
        var bpmnPlane = bpmnDiagram.BPMNPlane();
        bpmnPlane.id = "BpmnPlane" + collaboration.Id;
        bpmnPlane.bpmnElement = collaboration.Id;
        AddBpmnDiagramNameSpace(bpmnPlane);
        foreach (var participant in collaboration.participants ?? Enumerable.Empty<Participant>())
        {
            FillFlowNodeSymbols(participant.processRef);
            FillLanesData(participant.processRef);
            LanesDiagram();
            LocatingFlowElement(bpmnPlane);
            LocatingLanes();
            ParticipantDiagram(bpmnPlane, participant);
            DrawLanesDiagram(bpmnPlane);
            DrawFlowElement(bpmnPlane);
            SetDataStoreRefToXml(bpmnPlane, participant.processRef);
            SetSequenceFlowsToXml(_flowNodeSymbols, bpmnPlane, participant.processRef, _bpmnDefinitions);
        }
    }

    private bool FindMissedFlowNodes(dynamic bpmnPlane, Dictionary<string, FlowNodeSymbol> diagramFlowNodes)
    {
        var mustDrawFlowNode = new List<FlowNodeSymbol>();
        var drawedIncomingNum = 0;

        FlowNodeSymbol nodeSymbol = null;
        foreach (var notdrawnElement in diagramFlowNodes.Values
             .Where(nde => nde.SetInDiagram == false).OrderBy(n => n.Incoming.Count != 0))
        {
            if (notdrawnElement.Incoming.Count != 0)
            {
                var drawedIncomingCount = notdrawnElement.Incoming
                     .Count(of => of.SetInDiagram);
                if (drawedIncomingNum < drawedIncomingCount)
                {
                    nodeSymbol = notdrawnElement;
                    drawedIncomingNum = drawedIncomingCount;
                }
            }
            else if (notdrawnElement.FlowNode is BoundaryEvent)
                continue;
            else
            {
                if (drawedIncomingNum == 0)
                {
                    nodeSymbol = notdrawnElement;
                    break;
                }
            }
        }
        if (nodeSymbol != null)
        {
            mustDrawFlowNode.Add(nodeSymbol);
            SettingLocation(mustDrawFlowNode, diagramFlowNodes);
        }
        return nodeSymbol != null;
    }
    private void SetDataStoreRefToXml(dynamic bpmnPlane, Process processDef)
    {
        int dataStoreRefY = 12;
        int dataStoreRefX = 200;
        foreach (var dataStoreRef in processDef?.flowElements?.Values.OfType<DataStoreReference>()
                                              ?? [])
        {
            var bpmnShape = bpmnPlane.BPMNShape();
            bpmnShape.id = "dataStoreRef_" + dataStoreRef.Id;
            bpmnShape.bpmnElement = dataStoreRef.Id;
            AddBpmnDiagramNameSpace(bpmnShape);
            var bounds = bpmnShape.Bounds();
            AddBpmnDiagramBoundsNameSpace(bounds);
            bounds.x = dataStoreRefX += 100;
            bounds.y = dataStoreRefY;
            bounds.height = 50;
            bounds.width = 50;

        }
    }

    private void LanesDiagram()
    {
        var finder = new LanePostionFinder(_laneDiagrams);
        LaneSymbol laneSymbol;
        while ((laneSymbol = finder.FindLane()) != null)
        {
            laneSymbol.Paste = true;
            laneSymbol.XCoefficient = 1;
            laneSymbol.YCoefficient = 1;
            laneSymbol.Height = 1;
        }

        var minindex = _laneDiagrams.Min(l => l.Index);
        foreach (var lane in _laneDiagrams)
        {
            lane.Index -= minindex;
        }
    }

    private static void AddBpmnDiagramBoundsNameSpace(dynamic node)
    {
        node.Namespace = "http://www.omg.org/spec/DD/20100524/DC";
    }
    private static void AddBpmnDiagramWayPointNameSpace(dynamic node)
    {
        node.Namespace = "http://www.omg.org/spec/DD/20100524/DI";
    }

    private static void AddBpmnDiagramNameSpace(dynamic node)
    {
        node.Namespace = "http://www.omg.org/spec/BPMN/20100524/DI";
    }
}
internal class FlowNodeSymbol
{
    private const int EventHeight = 36;
    private const int EventWidth = 36;
    private const int ActivityHeight = 80;
    private const int ActivityWidth = 100;
    private const int GatewayHeight = 50;
    private const int GatewayWidth = 50;

    public FlowNode FlowNode { get; set; }
    public bool SetInDiagram { get; set; }
    public bool Draw { get; set; }
    public long XCoefficient { get; set; }
    public long YCoefficient { get; set; }
    public List<FlowNodeSymbol> Incoming { get; set; }
    public List<FlowNodeSymbol> Outgoing { get; set; }
    public LaneSymbol Lane { get; set; }
    public long Height { get; set; } = 1;
    public long Width { get; set; } = 1;
    public long XDiagramPosition { get; set; }
    public long YDiagramPosition { get; set; }

    internal long Left => XDiagramPosition;
    internal long MiddleLeftX => Left + Height / 2;
    internal long MiddleLeftY => YDiagramPosition + Height / 2;
    internal long Right => XDiagramPosition + Width;
    internal long MiddleRightX => MiddleLeftX + Width;
    internal long MiddleRightY => YDiagramPosition + Height / 2;
    internal long Up => YDiagramPosition;
    internal long Down => YDiagramPosition + Height;
    internal long MiddleX => Left + Width / 2;
}
internal class LaneSymbol
{
    public string Id { get; set; }
    public long XCoefficient { get; set; }
    public long YCoefficient { get; set; }
    public long Height { get; set; }
    public long Width { get; set; }
    public List<FlowNodeSymbol> FlowNodeSymbols { get; set; }
    public int Index { get; set; }
    public bool Drawn { get; set; }
    public bool Paste { get; set; }
}
