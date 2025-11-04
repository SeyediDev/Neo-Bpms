namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    ~QueryUtility()
    {
        if (_dataSource != null)
        {
            Logger.LogError("~QueryUtility({0}, {1}) data source dont release by code.", Entity?.Id, Name);
            ReleaseQuery();
        }
    }

    public void ReleaseQuery()
    {
        Release();
    }
    public override void Release()
    {
        foreach (var subQuery in subQuerys?.Values ?? Enumerable.Empty<SubQueryDefinition>())
        {
            try
            {
                subQuery.Query?.Release();
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
        }

        base.Release();
    }

    protected override void ReleaseConnection()
    {
        foreach (var subQuery in subQuerys?.Values ?? Enumerable.Empty<SubQueryDefinition>())
        {
            try
            {
                subQuery.Query?.ReleaseConnection();
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
        }
        base.ReleaseConnection();
    }
}