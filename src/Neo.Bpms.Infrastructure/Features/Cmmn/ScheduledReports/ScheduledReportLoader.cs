using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Domain.Models.Security.Authorization;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;

public class ScheduledReportLoader(IAccessServices accessServices,
    ReportConfigManager reportConfigManager,
    ScheduledReportConfigBackupRestore scheduledReportConfigBackupRestore)
{
    public async Task<
        (ConfiguredReport configuredReport, ConfiguredScheduledReport configuredScheduledReport, string errorString, bool Result)> 
        FetchScheduledReport(string reportConfigId, long scheduledReportId, IdentityUser user)
    {
        ConfiguredReport configuredReport = null;
        ConfiguredScheduledReport configuredScheduledReport = null;
        if (user == null)
        {
            return (null,null,"کاربر معتبر نمی باشد!", false);
        }

        GetReportConfigResult getReportConfigResult = new(reportConfigId);
        if (!await reportConfigManager.GetReportConfig("", "", "", getReportConfigResult, user))
        {
            return (null, null, "چنین طراحی موجود نیست!", false);
        }
        if (configuredReport.IsMeta)
        {
            return (null, null, "امکان افزودن زمانبندی به طراحی گزارش از پیش تعریف شده وجود ندارد.!", false);
        }

        Report report = configuredReport.Report;
        if (report == null)
        {
            return (null, null, "Error: شما دسترسی طراحی گزارش ندارید.", false);
        }

        if (!accessServices.CheckReportAccess(user, report))
        {
            return (null, null, "Error: شما به این گزارش دسترسی ندارید.", false);
        }
        if (!accessServices.CheckSystemFeatureAccess(user, SystemFeatureId.ScheduledReportDesign))
        {
            return (null, null, "Error: شما دسترسی طراحی گزارش ندارید.", false);
        }

        if (configuredReport.IsPublic && !accessServices.CheckSystemFeatureAccess(user, SystemFeatureId.PublishConfigs))
        {
            return (null, null, "Error: شما دسترسی انتشار طراحی گزارش ندارید.", false);
        }
        if (configuredReport.Roles != null && configuredReport.Roles.Any() && !user.IsAdmin &&
             !configuredReport.Roles.Any(roleId => user.CheckRole(roleId)))
        {
            return (null, null, "Error: شما دسترسی به گروه کاربری فوق را ندارید.", false);
        }

        configuredReport.ScheduledReports ??=
                [.. await scheduledReportConfigBackupRestore.Configurations(reportConfigId)];
        configuredScheduledReport = configuredReport.ScheduledReports?.FirstOrDefault(f => f.Id == scheduledReportId);
        return (configuredReport, configuredScheduledReport, 
            configuredScheduledReport == null && scheduledReportId > 0 ? "کد زمانبندی اجرای گزارش شده صحیح نیست" : null, 
            configuredScheduledReport != null || scheduledReportId == 0);
    }
}
