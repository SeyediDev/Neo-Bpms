using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

/// <summary>
/// 14.8
/// </summary>
internal partial class BpmnDiagramXmlConvertor
{
    private const long SequenceFlowDefaultDistance = 20;

    private void SetSequenceFlowsToXml(Dictionary<string, FlowNodeSymbol> diagramFlowNodes, dynamic bpmnPlane,
        Process processDef, BpmnDefinitions bpmnDefinitions)
    {
        List<SequenceDraw> sequenceDraws = [];
        foreach (var sequenceFlowElement in processDef?.flowElements?.Values.OfType<SequenceFlow>()
                                                        ?? [])
        {
            if (sequenceFlowElement.sourceRef?.Id == null || sequenceFlowElement.targetRef?.Id == null)
            {
                bpmnDefinitions?.ErrorInfos.AddWarning("Sequence Flow Without Source or Target has been found",
                    "BpmnDefinitions" + bpmnDefinitions.Id, "14.10.1", "Exception", eWarningLevel.WarningLevel1);
                continue;
            }

            diagramFlowNodes.TryGetValue(sequenceFlowElement.sourceRef.Id, out var source); //source ref lane
            diagramFlowNodes.TryGetValue(sequenceFlowElement.targetRef.Id, out var target); //target ref lane
            if (source == null || target == null)
            {
                bpmnDefinitions?.ErrorInfos.AddWarning("SequenceFlow Source/Target FlowNode not defined",
                    "BpmnDefinitions" + bpmnDefinitions.Id, "14.10.1", "Exception", eWarningLevel.WarningLevel1);
                continue;
            }

            var bpmnEdge = bpmnPlane.BPMNEdge();
            bpmnEdge.id = "sequenceFlow_" + sequenceFlowElement.Id;
            bpmnEdge.bpmnElement = sequenceFlowElement.Id;
            AddBpmnDiagramNameSpace(bpmnEdge);
            sequenceDraws.Add(new SequenceDraw(_laneDiagrams, bpmnEdge, source, target));
            //				SourceWayPoint(bpmnEdge, source, target, diagramFlowNodes);
            //				MiddleLines(bpmnEdge, source, target, diagramFlowNodes);
            //				TargetWayPoint(bpmnEdge, source, target);
        }

        SequencesDraw.Draw(sequenceDraws);
    }

    //private long CalcParticipantYPosition(FlowNodeSymbol symbol)
    //{
    //	var y = _laneDiagrams.Where(l => l.Index < symbol.Lane.Index)
    //				  .Sum(lane => lane.FlowNodeSymbols.Max(f => f.YCoefficient)) + symbol.YCoefficient;
    //	return y;
    //}


    private static void AddWaypoint(dynamic bpmnEdge, long sourceX, long sourceY)
    {
        var waypoint = bpmnEdge.waypoint();
        AddBpmnDiagramWayPointNameSpace(waypoint);
        waypoint.x = sourceX;
        waypoint.y = sourceY;
    }

    private class SequencesDraw
    {
        internal static void Draw(List<SequenceDraw> sequences)
        {
            sequences.Sort(SortSequenceFlows);
            foreach (var sequence in sequences)
            {
                var options = sequence.FetchOptions();
                foreach (var option in options)
                {
                    foreach (var crossSequence in sequences.Where(c => c != sequence))
                        crossSequence.CheckCrossConnect(option);
                }

                options.Sort(Fitness);
                sequence.LineParts = options.FirstOrDefault()?.LineParts;
                if (sequence.LineParts != null && sequence.LineParts.Count > 0)
                {
                    var i = 0;
                    for (; i < sequence.LineParts.Count; i++)
                    {
                        var linePart = sequence.LineParts[i];
                        AddWaypoint(sequence.BPMNEdge, linePart.From.X, linePart.From.Y);
                    }

                    AddWaypoint(sequence.BPMNEdge, sequence.LineParts[i - 1].To.X, sequence.LineParts[i - 1].To.Y);
                }
            }
        }

        private static int SortSequenceFlows(SequenceDraw x, SequenceDraw y)
        {
            if (x.Source.XCoefficient < y.Source.XCoefficient)
                return -1;
            if (x.Target.XCoefficient - x.Source.XCoefficient < y.Target.XCoefficient - y.Source.XCoefficient)
                return -1;
            if (x.Target.YCoefficient == x.Source.YCoefficient && y.Target.YCoefficient != y.Source.YCoefficient)
                return -1;
            if (x.Target.Lane == x.Source.Lane && y.Target.Lane != y.Source.Lane)
                return -1;
            return x.Target.XCoefficient == x.Source.XCoefficient && y.Target.XCoefficient != y.Source.XCoefficient
                ? -1
                : 1;
        }

        private static int Fitness(LinePartsOption y, LinePartsOption x)
        {
            if (x == null || y == null) return 0;
            if (x.CrossConnectWithNotation != y.CrossConnectWithNotation)
                return x.CrossConnectWithNotation ? -1 : 1;
            var crossConnectCountDiff = y.CrossConnectCount - x.CrossConnectCount;
            if (crossConnectCountDiff == 0)
            {

                var priorityDiff = y.Priority - x.Priority;
                if (priorityDiff == 0)
                {
                    var linePartsCountDiff = y.LineParts.Count - x.LineParts.Count;
                    if (linePartsCountDiff == 0)
                        return -1;
                    return linePartsCountDiff;
                }
                return priorityDiff;
            }

            return crossConnectCountDiff;
        }

        public static Point SetNodePoint(FlowNodeSymbol symbol, SideNodePoint.SequenceSideNodeDirection direction)
        {
            var s = new Point();
            switch (symbol.FlowNode.flowElementType)
            {
                case FlowElement.eFlowElementType.Gateway:
                    switch (direction)
                    {
                        case SideNodePoint.SequenceSideNodeDirection.Left:
                            s.X = symbol.XDiagramPosition;
                            s.Y = symbol.YDiagramPosition + GatewayHeight / 2;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Right:
                            s.X = symbol.XDiagramPosition + GatewayWidth;
                            s.Y = symbol.YDiagramPosition + GatewayHeight / 2;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Up:
                            s.X = symbol.XDiagramPosition + GatewayWidth / 2;
                            s.Y = symbol.YDiagramPosition;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Down:
                            s.X = symbol.XDiagramPosition + GatewayWidth / 2;
                            s.Y = symbol.YDiagramPosition + GatewayHeight;
                            break;
                    }

                    break;
                case FlowElement.eFlowElementType.Activity:
                    switch (direction)
                    {
                        case SideNodePoint.SequenceSideNodeDirection.Left:
                            s.X = symbol.XDiagramPosition;
                            s.Y = symbol.YDiagramPosition + ActivityHeight / 2;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Right:
                            s.X = symbol.XDiagramPosition + ActivityWidth;
                            s.Y = symbol.YDiagramPosition + ActivityHeight / 2;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Up:
                            s.X = symbol.XDiagramPosition + ActivityWidth / 2;
                            s.Y = symbol.YDiagramPosition;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Down:
                            s.X = symbol.XDiagramPosition + ActivityWidth / 2;
                            s.Y = symbol.YDiagramPosition + ActivityHeight;
                            break;
                    }

                    break;
                case FlowElement.eFlowElementType.Event:
                    switch (direction)
                    {
                        case SideNodePoint.SequenceSideNodeDirection.Left:
                            s.X = symbol.XDiagramPosition;
                            s.Y = symbol.YDiagramPosition + EventHeight / 2;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Right:
                            s.X = symbol.XDiagramPosition + EventWidth;
                            s.Y = symbol.YDiagramPosition + EventHeight / 2;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Up:
                            s.X = symbol.XDiagramPosition + EventWidth / 2;
                            s.Y = symbol.YDiagramPosition;
                            break;
                        case SideNodePoint.SequenceSideNodeDirection.Down:
                            s.X = symbol.XDiagramPosition + EventWidth / 2;
                            s.Y = symbol.YDiagramPosition - EventHeight;
                            break;
                    }

                    break;
            }

            return s;
        }
    }
    private class SequenceDraw(List<LaneSymbol> laneDiagrams, dynamic bpmnEdge, FlowNodeSymbol source, FlowNodeSymbol target)
    {
        internal FlowNodeSymbol Source { get; } = source;
        internal FlowNodeSymbol Target { get; } = target;
        internal dynamic BPMNEdge { get; } = bpmnEdge;
        internal List<SequenceLinePart> LineParts { get; set; }

        internal List<LinePartsOption> FetchOptions()
        {
            var options = new List<LinePartsOption>();
            options.AddRange(DrawInComingNodes());
            options.AddRange(DrawOutGoingNodes());

            return options;
        }

        private IEnumerable<LinePartsOption> DrawInComingNodes()
        {
            var options = new List<LinePartsOption>();
            var sourceYCoefficient = CalcParticipantYPosition(Source);
            var targetYCoefficient = CalcParticipantYPosition(Target);
            if (Source.XCoefficient < Target.XCoefficient && sourceYCoefficient == targetYCoefficient)
            {
                //source up - target up
                //source down - target down
            }
            else if (Source.XCoefficient < Target.XCoefficient && sourceYCoefficient < targetYCoefficient)
            {
                options.Add(Create2LineOption(1, SideNodePoint.SequenceSideNodeDirection.Right,
                    SideNodePoint.SequenceSideNodeDirection.Up, new Point(Target.MiddleX, Source.MiddleRightY)));
                options.Add(Create2LineOption(2, SideNodePoint.SequenceSideNodeDirection.Down,
                    SideNodePoint.SequenceSideNodeDirection.Left, new Point(Source.MiddleX, Target.MiddleLeftY)));
                options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Right,
                    SideNodePoint.SequenceSideNodeDirection.Left, new Point(Source.Right + SequenceFlowDefaultDistance, Source.MiddleRightY),
                    new Point(Source.Right + SequenceFlowDefaultDistance, Target.MiddleLeftY)));
            }
            else if (Source.XCoefficient < Target.XCoefficient && sourceYCoefficient > targetYCoefficient)
            {
                options.Add(Create2LineOption(1, SideNodePoint.SequenceSideNodeDirection.Right,
                    SideNodePoint.SequenceSideNodeDirection.Down, new Point(Target.MiddleX, Source.MiddleRightY)));
                options.Add(Create2LineOption(2, SideNodePoint.SequenceSideNodeDirection.Up,
                    SideNodePoint.SequenceSideNodeDirection.Left, new Point(Source.MiddleX, Target.MiddleLeftY)));
                options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Right,
                    SideNodePoint.SequenceSideNodeDirection.Left, new Point(Source.Right + SequenceFlowDefaultDistance, Source.MiddleRightY),
                    new Point(Source.Right + SequenceFlowDefaultDistance, Target.MiddleLeftY)));
            }

            return options;
        }
        public long CalcParticipantYPosition(FlowNodeSymbol symbol)
        {
            var y = laneDiagrams.Where(l => l.Index < symbol.Lane.Index)
                        .Sum(lane => lane.FlowNodeSymbols.Max(f => f.YCoefficient)) + symbol.YCoefficient;
            return y;
        }

        private IEnumerable<LinePartsOption> DrawOutGoingNodes()
        {
            var options = new List<LinePartsOption>();
            var sourceYCoefficient = CalcParticipantYPosition(Source);
            var targetYCoefficient = CalcParticipantYPosition(Target);
            if (Source.XCoefficient < Target.XCoefficient && sourceYCoefficient == targetYCoefficient)
            {
                options.Add(DefaultStraightLine(SideNodePoint.SequenceSideNodeDirection.Right, SideNodePoint.SequenceSideNodeDirection.Left));
            }
            else if (Source.XCoefficient < Target.XCoefficient)
            {
                if (sourceYCoefficient < targetYCoefficient)
                {
                    options.Add(Create2LineOption(1, SideNodePoint.SequenceSideNodeDirection.Right,
                        SideNodePoint.SequenceSideNodeDirection.Up, new Point(Target.MiddleX, Source.MiddleRightY)));
                    options.Add(Create2LineOption(2, SideNodePoint.SequenceSideNodeDirection.Down,
                        SideNodePoint.SequenceSideNodeDirection.Left, new Point(Target.MiddleX, Source.Down)));
                    options.Add(Create2LineOption(3, SideNodePoint.SequenceSideNodeDirection.Right,
                        SideNodePoint.SequenceSideNodeDirection.Left, new Point(Target.MiddleX, Source.MiddleRightY)));
                }
                else if (sourceYCoefficient > targetYCoefficient)
                {
                    options.Add(Create2LineOption(1, SideNodePoint.SequenceSideNodeDirection.Up,
                        SideNodePoint.SequenceSideNodeDirection.Left, new Point(Source.MiddleX, Target.MiddleLeftY)));
                    options.Add(Create2LineOption(2, SideNodePoint.SequenceSideNodeDirection.Right,
                        SideNodePoint.SequenceSideNodeDirection.Down, new Point(Target.MiddleX, Source.MiddleRightY)));
                    options.Add(Create2LineOption(3, SideNodePoint.SequenceSideNodeDirection.Right,
                        SideNodePoint.SequenceSideNodeDirection.Left, new Point(Source.Right, Target.MiddleLeftY)));
                }
            }
            else if (Source.XCoefficient > Target.XCoefficient)
            {
                if (sourceYCoefficient == targetYCoefficient)
                {
                    options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Up,
                        SideNodePoint.SequenceSideNodeDirection.Up, new Point(Source.MiddleX, Source.YDiagramPosition - SequenceFlowDefaultDistance)
                        , new Point(Target.MiddleX, Target.YDiagramPosition - SequenceFlowDefaultDistance)));
                    options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Down,
                        SideNodePoint.SequenceSideNodeDirection.Down, new Point(Target.MiddleX, Source.YDiagramPosition + SequenceFlowDefaultDistance)
                        , new Point(Target.MiddleX, Target.YDiagramPosition + SequenceFlowDefaultDistance)));
                }
                else if (sourceYCoefficient < targetYCoefficient)
                {
                    options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Up,
                        SideNodePoint.SequenceSideNodeDirection.Up, new Point(Source.MiddleX, Source.YDiagramPosition - SequenceFlowDefaultDistance)
                        , new Point(Target.MiddleX, Source.YDiagramPosition - SequenceFlowDefaultDistance)));
                    options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Down,
                        SideNodePoint.SequenceSideNodeDirection.Up, new Point(Target.MiddleX, Source.YDiagramPosition + SequenceFlowDefaultDistance)
                        , new Point(Target.MiddleX, Target.YDiagramPosition + SequenceFlowDefaultDistance)));
                }
                else if (sourceYCoefficient > targetYCoefficient)
                {
                    options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Up,
                        SideNodePoint.SequenceSideNodeDirection.Down, new Point(Source.MiddleX, Source.YDiagramPosition - SequenceFlowDefaultDistance)
                        , new Point(Target.MiddleX, Source.YDiagramPosition - SequenceFlowDefaultDistance)));
                    options.Add(Create3LineOption(1, SideNodePoint.SequenceSideNodeDirection.Down,
                        SideNodePoint.SequenceSideNodeDirection.Down, new Point(Target.MiddleX, Source.YDiagramPosition + SequenceFlowDefaultDistance)
                        , new Point(Target.MiddleX, Source.YDiagramPosition + SequenceFlowDefaultDistance)));
                }

            }
            else if (Source.XCoefficient == Target.XCoefficient)
            {
                if (sourceYCoefficient < targetYCoefficient)
                {
                    options.Add(DefaultStraightLine(SideNodePoint.SequenceSideNodeDirection.Down, SideNodePoint.SequenceSideNodeDirection.Up));
                }
                else if (sourceYCoefficient > targetYCoefficient)
                {
                    options.Add(DefaultStraightLine(SideNodePoint.SequenceSideNodeDirection.Up, SideNodePoint.SequenceSideNodeDirection.Down));
                }
            }
            return options;
        }

        private LinePartsOption Create3LineOption(int priority, SideNodePoint.SequenceSideNodeDirection sourceDirection,
            SideNodePoint.SequenceSideNodeDirection targetDirection, Point firstMiddlepoint, Point secondMiddlepoint)
        {
            var option = CreateLinePartsOption(priority, sourceDirection,
                targetDirection);
            option.LineParts =
            [
                new SequenceLinePart {From = option.Source.Point, To = firstMiddlepoint},
                new SequenceLinePart {From = firstMiddlepoint, To = secondMiddlepoint},
                new SequenceLinePart {From = secondMiddlepoint, To = option.Target.Point}
            ];
            return option;
        }

        private LinePartsOption Create2LineOption(int priority, SideNodePoint.SequenceSideNodeDirection sourceDirection, SideNodePoint.SequenceSideNodeDirection targetDirection,
            Point point)
        {
            var option = CreateLinePartsOption(priority, sourceDirection,
                targetDirection);
            option.LineParts =
            [
                new SequenceLinePart {From = option.Source.Point, To = point},
                new SequenceLinePart {From = point, To = option.Target.Point}
            ];
            return option;
        }

        private LinePartsOption DefaultStraightLine(SideNodePoint.SequenceSideNodeDirection sourceDirection, SideNodePoint.SequenceSideNodeDirection targetDirection)
        {
            var l = CreateLinePartsOption(1, sourceDirection, targetDirection);
            l.LineParts = [new SequenceLinePart { From = l.Source.Point, To = l.Target.Point }];
            return l;
        }

        private LinePartsOption CreateLinePartsOption(int priority, SideNodePoint.SequenceSideNodeDirection sourceDirection,
            SideNodePoint.SequenceSideNodeDirection targetDirection)
        {
            return new LinePartsOption(priority)
            {
                Source = new SideNodePoint
                {
                    Direction = sourceDirection,
                    Point = SequencesDraw.SetNodePoint(Source, sourceDirection),
                },
                Target = new SideNodePoint
                {
                    Direction = targetDirection,
                    Point = SequencesDraw.SetNodePoint(Target, targetDirection),
                },
            };
        }


        internal void CheckCrossConnect(LinePartsOption option)
        {
            if (option.LineParts == null)
                throw new Exception();
            foreach (var otherSequenceLinePart in option.LineParts)
            {
                option.CrossConnectCount += CheckCrossConnect(otherSequenceLinePart, out var crossConnectWithNotation);
                if (crossConnectWithNotation)
                    option.CrossConnectWithNotation = true;
            }
        }

        private int CheckCrossConnect(SequenceLinePart otherSequenceLinePart, out bool crossConnectWithNotation)
        {
            crossConnectWithNotation = CheckNodeCrossConnect(otherSequenceLinePart);
            var crossConnectCount = 0;
            if (LineParts != null)
            {
                foreach (var sequenceLinePart in LineParts)
                    crossConnectCount += new SequenceLinePartCrossConnect(sequenceLinePart).CheckCrossConnect(otherSequenceLinePart) ? 1 : 0;
            }

            return crossConnectCount;
        }

        private bool CheckNodeCrossConnect(SequenceLinePart otherSequenceLinePart)
        {
            return SequenceLinePartCrossConnect.Check(otherSequenceLinePart, Source) || SequenceLinePartCrossConnect.Check(otherSequenceLinePart, Target);
        }
    }

    public class SideNodePoint
    {
        internal SequenceSideNodeDirection Direction { get; set; }
        internal Point Point { get; set; }

        internal enum SequenceSideNodeDirection
        {
            Left = 1,
            Right = -1,
            Up = 2,
            Down = -2
        }
    }

    private class SequenceLinePartCrossConnect
    {
        public SequenceLinePartCrossConnect(SequenceLinePart linePart)
        {
            LinePart = linePart;
        }

        private SequenceLinePartCrossConnect(long fromX, long fromY, long toX, long toY)
        {
            LinePart = new SequenceLinePart
            {
                From = new Point { X = fromX, Y = fromY },
                To = new Point { X = toX, Y = toY }
            };
        }

        private SequenceLinePart LinePart { get; }

        public bool CheckCrossConnect(SequenceLinePart otherSequenceLinePart)
        {
            var p = FetchCrossConnect(otherSequenceLinePart);
            if (p == null) return false;
            var r = new Rectangle(new Point
            {
                X = Math.Min(LinePart.From.X, LinePart.To.X),
                Y = Math.Min(LinePart.From.Y, LinePart.To.Y)
            },
                new Point
                {
                    X = Math.Max(LinePart.From.X, LinePart.To.X),
                    Y = Math.Max(LinePart.From.Y, LinePart.To.Y)
                }
            );
            return r.Contains(p);
        }

        private static bool Check(SequenceLinePart otherSequenceLinePart, long fromX, long fromY, long toX, long toY)
        {
            return new SequenceLinePartCrossConnect(fromX, fromY, toX, toY).CheckCrossConnect(otherSequenceLinePart);
        }

        private Point FetchCrossConnect(SequenceLinePart line)
        {
            double CalcCoef(SequenceLinePart l)
            {
                if (l.From.X - l.To.X == 0) return 0;
                // ReSharper disable once PossibleLossOfFraction
                return (l.From.Y - l.To.Y) / (l.From.X - l.To.X);
            }

            var p = new Point();
            if (LinePart.From.X == LinePart.To.X)
            {
                p.X = LinePart.From.X;
                if (line.From.X == line.To.X)
                {
                    if (LinePart.From.X != line.From.X)
                        p = null;
                    else if (Math.Max(LinePart.From.Y, LinePart.To.Y) > Math.Min(line.From.Y, line.To.Y))
                        p.Y = (Math.Max(LinePart.From.Y, LinePart.To.Y) + Math.Min(line.From.Y, line.To.Y)) / 2;
                    else if (Math.Max(line.From.Y, line.To.Y) > Math.Min(LinePart.From.Y, LinePart.To.Y))
                        p.Y = (Math.Max(line.From.Y, line.To.Y) + Math.Min(LinePart.From.Y, LinePart.To.Y)) / 2;

                    else if (Math.Max(LinePart.From.Y, LinePart.To.Y) >= Math.Max(line.From.Y, line.To.Y)
                                && Math.Min(LinePart.From.Y, LinePart.To.Y) <= Math.Min(line.From.Y, line.To.Y))
                        p.Y = line.From.Y;
                    else if (Math.Max(line.From.Y, line.To.Y) >= Math.Max(LinePart.From.Y, LinePart.To.Y)
                                && Math.Min(line.From.Y, line.To.Y) <= Math.Min(LinePart.From.Y, LinePart.To.Y))
                        p.Y = LinePart.From.Y;

                    else p = null;
                }
                else if (line.From.Y == line.To.Y)
                {
                    if (LinePart.From.X > Math.Min(line.From.X, line.To.X) && LinePart.From.X < Math.Max(line.From.X, line.To.X))
                        p.Y = line.From.Y;
                    else p = null;
                }
                else
                {
                    var coefLine = CalcCoef(line);
                    p.Y = (long)coefLine * (p.X - line.From.X) + line.From.Y;
                }
            }
            else if (LinePart.From.Y == LinePart.To.Y)
            {
                p.Y = LinePart.From.Y;
                if (line.From.Y == line.To.Y)
                {
                    if (LinePart.From.Y != line.From.Y)
                        p = null;
                    else if (Math.Max(LinePart.From.X, LinePart.To.X) > Math.Min(line.From.X, line.To.X))
                        p.X = (Math.Max(LinePart.From.X, LinePart.To.X) + Math.Min(line.From.X, line.To.X)) / 2;
                    else if (Math.Max(line.From.X, line.To.X) > Math.Min(LinePart.From.X, LinePart.To.X))
                        p.X = (Math.Max(line.From.X, line.To.X) + Math.Min(LinePart.From.X, LinePart.To.X)) / 2;
                    else p = null;
                }
                else if (line.From.X == line.To.X)
                {
                    if (LinePart.From.Y > Math.Min(line.From.Y, line.To.Y) && LinePart.From.Y < Math.Max(line.From.Y, line.To.Y))
                        p.X = line.From.X;
                    if (line.From.Y > Math.Min(LinePart.From.Y, LinePart.To.Y) && line.From.Y < Math.Max(LinePart.From.Y, LinePart.To.Y))
                        p.X = line.From.X;
                    else p = null;
                }
                else
                {
                    var coefLine = CalcCoef(line);
                    p.X = (long)((p.Y - line.From.Y) / coefLine + line.From.X);
                }
            }
            else
            {
                var coefBaseLine = CalcCoef(LinePart);
                var coefLine = CalcCoef(line);
                p.X = (long)((coefBaseLine * LinePart.From.X - LinePart.From.Y + line.From.Y - coefLine * line.From.X) / (coefBaseLine - coefLine));
                p.Y = (long)(coefBaseLine * p.X - coefBaseLine * LinePart.From.X + LinePart.From.Y);
            }
            return p;
        }

        public static bool Check(SequenceLinePart otherSequenceLinePart, FlowNodeSymbol node)
        {
            long Minus10Percentage(long num)
            {
                return num - num * 10 / 100;
            }

            var left = Minus10Percentage(node.Left);
            var right = Minus10Percentage(node.Right);
            var yDiagramPosition = Minus10Percentage(node.YDiagramPosition);
            if (Check(otherSequenceLinePart, left, yDiagramPosition, right, yDiagramPosition))//up
                return true;
            var nodeHeight = Minus10Percentage(node.Height);
            var nodeWidth = Minus10Percentage(node.Width);
            if (Check(otherSequenceLinePart, left, yDiagramPosition + nodeHeight, right,
                yDiagramPosition + nodeHeight + nodeWidth))//down
                return true;
            if (Check(otherSequenceLinePart, left, yDiagramPosition, left,
                yDiagramPosition + nodeHeight))//left
                return true;
            if (Check(otherSequenceLinePart, right, yDiagramPosition, right,
                yDiagramPosition + nodeHeight))//right
                return true;
            return false;
        }
    }

    private class SequenceLinePart
    {
        internal Point From { get; set; }
        internal Point To { get; set; }
    }

    internal class Point
    {
        public Point()
        {

        }
        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }
        internal long X { get; set; }
        internal long Y { get; set; }
    }

    private class LinePartsOption(int priority)
    {
        public SideNodePoint Source { get; set; }
        public SideNodePoint Target { get; set; }
        internal List<SequenceLinePart> LineParts { get; set; }
        public int CrossConnectCount { get; set; }
        public int Priority { get; } = priority;
        public bool CrossConnectWithNotation { get; set; }
    }

    internal class Rectangle(Point leftUpPoint, Point rightDownPoint)
    {
        public Point LeftUpPoint { get; set; } = leftUpPoint;
        public Point RightDownPoint { get; set; } = rightDownPoint;

        public bool Contains(Point point)
        {
            if (point.X < LeftUpPoint.X)
                return false;
            if (point.Y < LeftUpPoint.Y)
                return false;
            if (point.X > RightDownPoint.X)
                return false;
            if (point.Y > RightDownPoint.Y)
                return false;
            return true;
        }
    }
}
