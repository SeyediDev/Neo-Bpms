namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

public class QueryInfo
{
    public QueryInfo(bool shouldAddQueryTexts = false)
    {
        _shouldAddQueryTexts = shouldAddQueryTexts;
    }

    public long QueryTime { get; private set; }
    private List<string> QueryList { get; set; }
    private readonly bool _shouldAddQueryTexts;

    public void AddQueryText(string queryText)
    {
        if (!_shouldAddQueryTexts)
            return;
        QueryList ??= [];
        QueryList.Add(queryText);
    }
    public void AddQueryTime(long queryTime)
    {
        QueryTime += queryTime;
    }
    public void AddByQueryUtility(QueryUtility queryUtility)
    {
        AddQueryTime(queryUtility.QueryTime);
        AddQueryText(queryUtility.CommandTxt);
    }
    public IEnumerable<string> GetQueries()
    {
        return QueryList?.Select(qi => qi) ?? [];
    }
}