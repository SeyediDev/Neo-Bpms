using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.JobScheduler;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;

public class ReportTimer(
    TakingScheduledReport takingReport,
    ScheduledReportConfigBackupRestore scheduledReportConfigBackupRestore
    ) : JobTimer
{
    protected override async Task<(bool, ElasticObject)> Take(DateTime dt, Job job)
    {
        try
        {
            (bool, ElasticObject) result = await takingReport.TakeReportAsync(job.JobSchedule.ConfigId,
                job.JobSchedule.Id, null, null, default);
            return result;
        }
        catch (Exception exception)
        {
            string eventText = $"One exception catched on ReportTimer {job.JobSchedule.Id} - {job.JobSchedule.Name} at " +
                                 DateTime.Now.ToString("G");
            _ = $"<div>{eventText}</div><div>{exception}</div>";
            return (false, null);
        }
    }

    protected override void RecordLog(Job record, ElasticObject logRecord)
    {
        ApplyUtility log = ApplyUtility<ScheduledReportLog>.New();
        log.Insert(logRecord);
    }

    protected override DateTime? FetchLatestLogDate(Job job, out long repeatCount)
    {
        repeatCount = 0;
        ElasticObject record = QueryUtility<ScheduledReportLog>
            .Where($"{nameof(ScheduledReportLog.ScheduledReportId)}=={job.JobSchedule.Id}")
            .GroupBy(nameof(ScheduledReportLog.ScheduledReportId), eAggregationFunctions.GroupByItem)
            .Max(nameof(ScheduledReportLog.Date), nameof(ScheduledReportLog.Date))
            .Count("*", "repeatCount")
            .FirstOrDefault();
        if (record != null)
            repeatCount = record.GetLong("repeatCount");
        return record?.GetDateTime(nameof(ScheduledReportLog.Date));
    }

    protected override async Task<IEnumerable<Job>> FetchSchedulesList(DateTime currentDate, CancellationToken cancellationToken)
    {
        List<ConfiguredScheduledReport> items = await scheduledReportConfigBackupRestore.Configurations(currentDate, cancellationToken);
        return [.. items.Select(c => new Job { JobSchedule = c })];
    }

    //public override void OnStop()
    //{
    //	//todo
    //	base.OnStop();
    //}
}
