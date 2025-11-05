namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal class LanePostionFinder
{
    public LanePostionFinder(List<LaneSymbol> laneDiagrams)
    {
        LanePostions = [.. laneDiagrams.Select(l => new LanePostion
        {
            Lane = l,
            FlowElementCount = l.FlowNodeSymbols.Count,
            Index = 0
        })];
    }
    public List<LanePostion> LanePostions;
    public LaneSymbol FindLane()
    {
        foreach (var lanePostion in LanePostions.Where(l => !l.Lane.Paste))
        {
            CalcRelationWithPasteLanes(lanePostion);
            CalcRelationWithNotPasteLanes(lanePostion);
        }
        var foundLane = LanePostions.Where(l => !l.Lane.Paste).OrderBy(l => l).FirstOrDefault()?.Lane;
        SetLaneIndex(foundLane);
        return foundLane;
    }

    private void SetLaneIndex(LaneSymbol laneSymbol)
    {
        var lanePosition = LanePostions.FirstOrDefault(l => l.Lane == laneSymbol);
        if (lanePosition == null) return;
        if (!LanePostions.Exists(l => l.Index != 0))
            lanePosition.Index = 100;
        else
        {
            var nearlane = CalcNearestLane(lanePosition);
            if (nearlane == null)
                lanePosition.Index = LanePostions.Max(lp => lp.Index) + 1;
            else
            {
                var minpos = LanePostions.Where(lp => lp.Index != 0).Min(lp => lp.Index);
                var maxpos = LanePostions.Max(lp => lp.Index);
                if (nearlane.Index == minpos)
                    lanePosition.Index = minpos - 1;
                else if (nearlane.Index == maxpos)
                    lanePosition.Index = maxpos + 1;
                else if (nearlane.Index - minpos - 1 <= maxpos + 1 - nearlane.Index)
                    lanePosition.Index = minpos - 1;
                else
                    lanePosition.Index = maxpos + 1;
            }
        }

        laneSymbol.Index = lanePosition.Index;
    }

    public LanePostion CalcNearestLane(LanePostion currentLane)
    {
        LanePostion lp = null;
        var count = 0;
        foreach (var lane in LanePostions.Where(l => l.Lane.Paste))
        {
            var rcount = lane.Lane.FlowNodeSymbols
                .Sum(f => f.Incoming?.Count(i => i.Lane == currentLane.Lane) ?? 0
                          + f.Outgoing?.Count(o => o.Lane == currentLane.Lane)) ?? 0;
            if (count < rcount)
                lp = lane;
        }
        return lp;
    }
    private void CalcRelationWithPasteLanes(LanePostion lanePostion)
    {
        lanePostion.RelationWithPasteLanes = LanePostions.Where(l => l.Lane.Paste && l.Lane.Id != lanePostion.Lane.Id).SelectMany(pasteLane => pasteLane.Lane.FlowNodeSymbols)
            .Sum(f => f.Incoming.Count(i => i.Lane == lanePostion.Lane) + f.Outgoing.Count(o => o.Lane == lanePostion.Lane));
    }

    private void CalcRelationWithNotPasteLanes(LanePostion lanePostion)
    {
        int sum = 0;
        foreach (LanePostion l in LanePostions)
        {
            if (!l.Lane.Paste && l.Lane.Id != lanePostion.Lane.Id)
                foreach (var symbol in l.Lane.FlowNodeSymbols)
                    sum += symbol.Incoming?.Count(i => i.Lane == lanePostion.Lane) ?? 0 + symbol.Outgoing?.Count(o => o.Lane == lanePostion.Lane) ?? 0;
        }

        lanePostion.RelationWithNotPasteLanes = sum;
    }
}
internal class LanePostion : IComparable<LanePostion>
{
    public LaneSymbol Lane { get; set; }
    public int RelationWithPasteLanes { get; set; }
    public int RelationWithNotPasteLanes { get; set; }
    public int FlowElementCount { get; set; }
    public int Index { get; set; }

    public int CompareTo(LanePostion other)
    {
        var c = other.RelationWithPasteLanes - RelationWithPasteLanes;
        if (c != 0)
            return c;
        c = other.RelationWithNotPasteLanes - RelationWithNotPasteLanes;
        if (c != 0)
            return c;
        return other.FlowElementCount - FlowElementCount;
    }
}
