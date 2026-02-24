using System.Globalization;
using System.Text.Json;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Infrastructure.Features.Cmmn.Reports;
using Neo.Bpms.UI.MVC.Controls;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ReportController
{
    /// <summary>
    /// API endpoint for SPA calendar: returns JSON with calendar data and month summary.
    /// Used when user changes month or calendar type - no page reload.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CalendarData(CancellationToken cancellationToken,
        string NamespaceId, string EntityId, string ReportId, string ConfigId,
        int calendarYear, int calendarMonth, string calendarType,
        string ParentReportIds, string ParentFilterValues,
        [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject re)
    {
        var user = GetUser();
        if (user == null || !User.Identity.IsAuthenticated)
            return Json(new { error = "Unauthorized" });

        var config = await CheckReportAccess(NamespaceId, EntityId, ReportId, ConfigId, user);
        if (config.ChartType != ChartType.Calendar)
            return Json(new { error = "Report is not a calendar type" });

        var report = config.Report;
        var configuredFilters = await filterConfigBackupRestore.Configurations("Report", report.NamespaceId, report.EntityId, report.Id, cancellationToken);
        (ElasticObject filters, _) = await filterController.GetConfiguredFilterValues(null, configuredFilters, user, GetReportPersistence(report), re ?? new ElasticObject());
        if (!string.IsNullOrEmpty(ParentReportIds) && !string.IsNullOrEmpty(ParentFilterValues))
            filters = controllerMethods.DecodeFilterValues(ParentFilterValues);

        var culture = CultureHelper.GetCurrentNeutralCulture() ?? "fa";
        // Inject calendar month range filter (fixed filter for date dimension)
        await InjectCalendarDateFilterAsync(filters, config, calendarYear, calendarMonth, calendarType ?? "shamsi", culture, user, cancellationToken);
        List<object>? listIds = ParentReportIds?.Split(',').Select(c=>(object)c).ToList();
        var reportData = await reportDataRoutines.GetReportData(config, true, filters,
            1, null, null, null, listIds,
            culture, false, user, 5000, false, cancellationToken);

        if (reportData.Errors != null && reportData.Errors.Count > 0)
            return Json(new { error = string.Join("; ", reportData.Errors.Select(e => e.Text ?? e.GeneralText)) });

        var cal = NormalizeCalendarType(calendarType ?? "shamsi");
        var renderer = new CalendarRenderer(reportData, cal);
        var calendarData = renderer.GetCalendarData(calendarYear, calendarMonth);
        var monthSummary = renderer.GetMonthSummary(calendarYear, calendarMonth, calendarData);
        var primaryColumnAlias = renderer.GetPrimaryColumnAlias();

        var response = new
        {
            calendarData = calendarData.Select(d => new
            {
                d.Date,
                d.GregorianDate,
                d.PersianDate,
                d.DayOfMonth,
                d.DayOfWeek,
                d.HasData,
                d.PrimaryValue,
                d.PrimaryValueFormatted,
                primaryColumnAlias = primaryColumnAlias,
                items = d.Items.Select(i => new { i.Label, i.Value, i.Type }).ToList()
            }).ToList(),
            monthSummary = new
            {
                monthSummary.MonthName,
                monthSummary.Year,
                monthSummary.TotalDaysWithData,
                monthSummary.TotalPrimaryValue,
                monthSummary.TotalPrimaryValueFormatted,
                monthSummary.AverageValue,
                monthSummary.AverageValueFormatted,
                monthSummary.MaxValue,
                monthSummary.MaxValueFormatted,
                monthSummary.MinValue,
                monthSummary.MinValueFormatted,
                primaryColumnAlias
            },
            weekdays = cal == "shamsi"
                ? new[] { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" }
                : new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }
        };

        return Json(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    private static string NormalizeCalendarType(string calendar)
    {
        if (string.IsNullOrEmpty(calendar)) return "shamsi";
        return calendar.Equals("miladi", StringComparison.OrdinalIgnoreCase) ? "gregorian" : calendar.ToLowerInvariant();
    }

    /// <summary>
    /// Injects __CalendarDateFrom and __CalendarDateTo into filters for backend date dimension filter.
    /// EstablishReportQuery will apply this when building the query.
    /// </summary>
    private Task InjectCalendarDateFilterAsync(ElasticObject filters, ConfiguredReport config, int year, int month, string calendarType, string culture, IdentityUser user, CancellationToken cancellationToken)
    {
        if (config.ChartType != ChartType.Calendar) return Task.CompletedTask;
        CalendarFilterHelper.InjectMonthRange(filters, year, month, calendarType);
        return Task.CompletedTask;
    }
}
