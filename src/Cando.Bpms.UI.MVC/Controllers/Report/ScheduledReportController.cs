using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Domain.Entities.Security.Authorization;
using Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;

namespace Neo.Bpms.UI.MVC.Controllers;

[Authorize]
public class ScheduledReportController(
    ScheduledReportConfigBackupRestore scheduledReportConfigBackupRestore,
    ScheduledReportLoader scheduledReportLoader,
    TakingScheduledReport reportManager
    ) : ControllerBaseMVC
{
    [HttpPost]
    public async Task<JsonResult> SaveScheduledReport(ScheduledReportViewModel scheduledReportViewModel, CancellationToken cancellationToken)
    {
        string reportConfigId = scheduledReportViewModel.ReportConfigId;
        ConfiguredScheduledReport scheduledReport = scheduledReportViewModel.ToConfiguredScheduledReport(ProjectDefinition.Project.DefaultCalendar);

        (ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool result)
            = await FetchScheduledReportObject(reportConfigId, scheduledReport.Id);
        if (!result)
        {
            return Json(errorString);
        }
        IdentityUser user = GetUser();

        Report report = configuredReport.Report;
        if (report == null)
        {
            return Json("خطا: شما دسترسی طراحی گزارش ندارید.");
        }

        if (scheduledReport.Id <= 0)
        {
            scheduledReport.UserId = user?.Id;
            await scheduledReportConfigBackupRestore.Save(scheduledReport, cancellationToken);
            scheduledReport.Id = scheduledReport.Id;
            configuredReport.ScheduledReports ??= [];
            configuredReport.ScheduledReports.Add(scheduledReport);
        }
        else
        {
            if (configuredScheduledReport != null)
            {
                if (string.IsNullOrEmpty(scheduledReport.UserId))
                {
                    scheduledReport.UserId = user?.Id;
                }

                configuredScheduledReport.Copy(scheduledReport);
                await scheduledReportConfigBackupRestore.Save(configuredScheduledReport, cancellationToken);
            }
            else
            {
                return Json("خطا: کد طراحی معتبر نیست.");
            }
        }
        return Json(new { scheduledReport.Id });
    }

    [HttpPost]
    public async Task<JsonResult> SaveScheduledFilterValues(ScheduledReportViewModel scheduledReportViewModel)
    {
        string reportConfigId = scheduledReportViewModel.ReportConfigId;
        ConfiguredScheduledReport scheduledReport = scheduledReportViewModel.ToConfiguredScheduledReport(ProjectDefinition.Project.DefaultCalendar);

        (ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool result)
            = await FetchScheduledReportObject(reportConfigId, scheduledReport.Id);
        if (!result)
        {
            return Json(errorString);
        }

        Report report = configuredReport.Report;
        if (report == null)
        {
            return Json("خطا: شما دسترسی طراحی گزارش ندارید.");
        }

        if (configuredScheduledReport != null)
        {
            if (string.IsNullOrEmpty(scheduledReport.UserId))
            {
                scheduledReport.UserId = GetUser()?.Id;
            }

            configuredScheduledReport.CopyValues(scheduledReport);
            await scheduledReportConfigBackupRestore.Save(configuredScheduledReport);
        }
        else
        {
            return Json("خطا: کد طراحی معتبر نیست.");
        }

        return Json(new { scheduledReport.Id });
    }

    [HttpPost]
    public async Task<JsonResult> DeleteScheduledReport(string ConfigReportId, long ScheduledReportId)
    {
        (ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool result)
            = await FetchScheduledReportObject(ConfigReportId, ScheduledReportId);
        if (!result)
        {
            return Json(errorString);
        }

        if (configuredScheduledReport != null)
        {
            await scheduledReportConfigBackupRestore.RemoveConfig(configuredScheduledReport);
        }
        else
        {
            return Json("خطا: کد طراحی گزارش زمانبندی شده معتبر نیست.");
        }

        _ = (configuredReport.ScheduledReports?.Remove(configuredScheduledReport));
        return Json("");
    }

    private async Task<(ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport,
        string errorString, bool Result)> 
        FetchScheduledReportObject(string configReportId, long scheduledReportId)
    {
        IdentityUser user = GetUser();
        ViewBag.user = user;
        return !CheckAccess(user, SystemFeatureId.ScheduledReportDesign)
            ? throw new HttpException("شما دسترسی تنظیم زمانبندی طراحی گزارش‌ها را ندارید.")
            : await scheduledReportLoader.FetchScheduledReport(configReportId, scheduledReportId, user);
    }

    [HttpGet]
    public async Task<JsonResult> FetchScheduledReport(string ConfigReportId, long ScheduledReportId)
    {
        (_, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool result)
            = await FetchScheduledReportObject(ConfigReportId, ScheduledReportId);
        return !result
            ? Json(errorString)
            : Json(new ScheduledReportViewModel(configuredScheduledReport));
    }

    [HttpPost]
    public async Task<JsonResult> TakeReport(string ConfigReportId, long ScheduledReportId, CancellationToken cancellationToken)
    {
        (ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool result)
            = await FetchScheduledReportObject(ConfigReportId, ScheduledReportId);
        if (!result)
        {
            return Json(errorString);
        }

        string culture = CultureHelper.GetCurrentNeutralCulture();
        IdentityUser user = GetUser();
        Task<ElasticObject> logRecord = reportManager.TakeReportAsync(configuredReport,
            configuredScheduledReport, user, culture, cancellationToken);

        if (logRecord != null)
        {
            ApplyUtility log = new("SystemConfigs", "ScheduledReportLog");
            _ = log.Insert(logRecord);
        }
        return Json("OK");
    }
}
