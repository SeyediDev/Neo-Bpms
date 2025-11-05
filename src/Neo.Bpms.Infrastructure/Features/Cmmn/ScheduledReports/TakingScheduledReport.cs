using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Domain.Modeling.Entities.CmmnConfig;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;

public class TakingScheduledReport(
    ScheduledReportLoader scheduledReportLoader, IIdentityUserService identityUserService,
    ReportStructRoutines reportStructRoutines,
    ReportDataRoutines reportDataRoutines)
{
    public async Task<(bool, ElasticObject)> TakeReportAsync(string reportConfigId,
        long scheduledReportId, string userId, string culture, CancellationToken cancellationToken=default)
    {
        IdentityUser user = await identityUserService.GetIdentityUserAsync(userId, cancellationToken);
        (ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport, _, bool result) 
            = await scheduledReportLoader.FetchScheduledReport(reportConfigId, scheduledReportId, user);
        if (result)
        {
            ElasticObject logRecord = await TakeReportAsync(configuredReport, configuredScheduledReport, user, culture, cancellationToken);
            return (true, logRecord);
        }

        return (false, null);
    }

    public async Task<ElasticObject> TakeReportAsync(ConfiguredReport config, 
        ConfiguredScheduledReport scheduledReport,
        IdentityUser user, string culture, CancellationToken cancellationToken)
    {
        user ??= await identityUserService.GetIdentityUserAsync(scheduledReport.UserId, cancellationToken);
        DateTime nowDateTime = DateTime.Now;
        int recordsCount = scheduledReport.MaxRecordCount > 0 ? (int)scheduledReport.MaxRecordCount : 10000;

        FetchScheduledReportFilterValues(scheduledReport, out ElasticObject filterValues);

        (ReportStructure structure, _) = await reportStructRoutines.GetReportStructure(
            config, null, culture, user);
        TakeConfigQuery takeConfigQuery = new(cancellationToken, config, filterValues, user, culture,
            structure, reportDataRoutines);

        ScheduledReportAction reportAction = new()
        {
            ScheduledReport = scheduledReport,
            ScheduledReportId = scheduledReport.Id,
            ScheduledReportName = scheduledReport.Name ?? config.Name,
            date = nowDateTime
        };
        reportAction.InitReport(cancellationToken,
            takeConfigQuery.Structure, [.. takeConfigQuery.Structure.Columns], user?.Culture,
            ProjectDefinition.Project.DefaultCalendar);
        reportAction.ReportView.Init();
        reportAction.ReportView.GenerateHeader();

        ElasticObject totalRecord = takeConfigQuery.FetchTotalRecord();
        if (totalRecord != null)
            reportAction.ReportView.GenerateTotalRow(totalRecord);
        takeConfigQuery.TakeQueryPageByPage(recordsCount, reportAction.AddRows);
        reportAction.ReportView.GenerateFooter();
        reportAction.ReportView.Release();
        (bool, LocalParameters) result = await reportAction.DoAction();
        bool successful = result.Item1;
        LocalParameters outParameters = result.Item2;
        return new ElasticObject
        {
            [nameof(ScheduledReportLog.ScheduledReportId)] = scheduledReport.Id,
            [nameof(ScheduledReportLog.Date)] = nowDateTime,
            [nameof(ScheduledReportLog.FileName)] = outParameters?.GetString(nameof(ScheduledReportLog.FileName)),
            [nameof(ScheduledReportLog.Successfull)] = successful
        };
    }

    private static void FetchScheduledReportFilterValues(ConfiguredScheduledReport scheduledReport,
        out ElasticObject filterValues)
    {
        filterValues = new ElasticObject();
        foreach (ConfiguredFilterValue configuredFilterValue in scheduledReport.Values ?? Enumerable.Empty<ConfiguredFilterValue>())
        {
            if (configuredFilterValue.Value != null)
                filterValues.SetField(configuredFilterValue.FieldId, configuredFilterValue.Value);
        }
    }
}
