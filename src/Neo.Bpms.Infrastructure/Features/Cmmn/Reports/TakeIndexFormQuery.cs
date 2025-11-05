namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class TakeIndexFormQuery
{
    private readonly Form _form;
    private readonly ElasticObject _filterValues;
    private readonly IdentityUser _user;
    private readonly string _culture;
    private readonly string _sortFields;
    private readonly CancellationToken _cancellationToken;
    public CommonFormStructure Structure { get; }

    public TakeIndexFormQuery(
        CommonFormStructure structure, CancellationToken cancellationToken, Form form, string sortFields,
        ElasticObject filterValues, IdentityUser user, string culture)
    {
        _form = form;
        _filterValues = filterValues;
        _user = user;
        _culture = culture;
        _sortFields = sortFields;
        Structure = structure;
        _cancellationToken = cancellationToken;
        Structure = structure;
    }

    public ElasticObject FetchTotalRecord()
    {
        return null;
    }

    public async Task TakeQueryPageByPage(FormDataRoutines formDataRoutines, int recordsCount, Action<IList<ReportRowInfo>> addRows,
        int recordsCountPerIteration = 100000)
    {
        if (recordsCountPerIteration <= 0)
            recordsCountPerIteration = 100000;
        if (recordsCountPerIteration > recordsCount)
            recordsCountPerIteration = recordsCount;
        for (int i = 0; i * recordsCountPerIteration < recordsCount; i++)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;
            IList<ReportRowInfo> rows = await TakeReportQueryPage(formDataRoutines, i + 1, recordsCountPerIteration);
            addRows(rows);
            if (rows.Count < recordsCountPerIteration)
                break;
        }
    }

    private async Task<IList<ReportRowInfo>> TakeReportQueryPage(FormDataRoutines formDataRoutines, int pageNo, int recordsCount)
    {
        IndexFormData records = await formDataRoutines.GetRecordsWithoutJoin(Structure,
            _form.entity, _form, _culture, _filterValues,
            _sortFields, pageNo, recordsCount, null,
            GetLocalParameters(_user, null), null, _user, true, true, _cancellationToken);

        return [.. records.Rows.Select(r => new ReportRowInfo
        {
            Data = r
        })];
    }
    protected static LocalParameters GetLocalParameters(IdentityUser user, ElasticObject record)
    {
        LocalParameters lp = new() { { "user", user }, { "userId", user?.Id } };
        if (record != null)
            lp.Add("q", record);
        return lp;
    }
}
