using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class TakeConfigQuery(CancellationToken cancellationToken, ConfiguredReport config,
    ElasticObject filterValues, IdentityUser user, string culture,
    ReportStructure structure, ReportDataRoutines reportDataRoutines)
{
    public ReportStructure Structure { get; } = structure;

    public ElasticObject FetchTotalRecord()
    {
        ReportData result = new(config, Structure, filterValues);
        LocalParameters lp = ReportDataRoutines.GetLocalParameters(user);
        ReportTotalRecord.GetTotalRecord(cancellationToken, result, config.Report.Entity, null, config, null, null, lp);
        return result.TotalRecord;
    }

    public void TakeQueryPageByPage(int recordsCount, Action<IList<ReportRowInfo>> addRows,
        int recordsCountPerIteration = 100000)
    {
        if (recordsCountPerIteration <= 0)
            recordsCountPerIteration = 100000;
        if (recordsCountPerIteration > recordsCount)
            recordsCountPerIteration = recordsCount;
        for (int i = 0; i * recordsCountPerIteration < recordsCount; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            IList<ReportRowInfo> rows = TakeReportQueryPage(i + 1, recordsCountPerIteration);
            addRows(rows);
            if (rows.Count < recordsCountPerIteration)
                break;
        }
    }

    private IList<ReportRowInfo> TakeReportQueryPage(int pageNo, int recordsCount)
    {
        ReportData result = new(config, Structure, filterValues);
        LocalParameters lp = ReportDataRoutines.GetLocalParameters(user);
        int recordsPerPage = recordsCount;
        int pageNumber = pageNo;
        _ = reportDataRoutines.RunQueryPage(cancellationToken, result,
            config, null, null, null,
            recordsPerPage, pageNumber,
            culture, false, lp, user, config.Report.Entity);
        return result.Rows;
    }
}
