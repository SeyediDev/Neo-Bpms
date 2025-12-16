using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public static class ReportTotalRecord
{
    public static void GetTotalRecord(CancellationToken cancellationToken,
        ReportData result, Entity entity, IList<object> ids, ConfiguredReport config,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        LocalParameters lp)
    {
        if (config.ViewType == ReportViewType.List || !result.Structure.SelectedColumns.Any(col =>
                col.aggrType != eAggregationFunctions.InColumn &&
                col.aggrType != eAggregationFunctions.GroupByItem))
            return;
        QueryUtility q = EstablishReportQuery.GetTotalQuery(result, entity, ids, config, parentReportConfig, subReport, lp);
        q.CancellationToken = cancellationToken;
        try
        {
            if (q.GetDocuments(result.Structure.FilterValues, lp))
            {
                ElasticObject r = q.GetRecord();
                if (r != null)
                    result.TotalRecord = r;
            }

            if (!string.IsNullOrEmpty(q.ErrorText))
                result.Errors.AddError(q.ErrorText, "Query", "13.0.3", q.ErrorText);
            result.QueryInfo.AddByQueryUtility(q);
        }
        catch (Exception e)
        {
            result.Errors.AddError(e.Message, "Query", "13.0.4", e.ToString());
        }

        q.ReleaseQuery();
    }

    public static long GetRecordCount(CancellationToken cancellationToken,
        ReportData result, Entity entity, IList<object> ids, ConfiguredReport config,
        LocalParameters lp)
    {
        try
        {
            QueryUtility qCount = EstablishReportQuery.GetRecordCountQuery(result, entity, ids, config, lp);
            qCount.CancellationToken = cancellationToken;
            result.RecordCount = (int)qCount.GetRecordCount(result.Structure.FilterValues, lp);
            if (result.RecordCount == 0 && !string.IsNullOrEmpty(qCount.ErrorText))
            {
                //TODO Temp : result.Errors.AddError(qCount.ErrorText, "Query", "13.0.2", qCount.ErrorText);
            }
        }
        catch (Exception e)
        {
            result.Errors.AddError(e.Message, "Query", "13.0.1", e.ToString());
            return 0;
        }

        return result.RecordCount;
    }
}
