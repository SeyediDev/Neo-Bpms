namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.JoinQuery;

public class ReferData
{
    public ReferData(string id)
    {
        Id = id;
    }

    public readonly string Id;
    public ElasticObject Data;
}
public class ReferList : List<ReferData>
{
}