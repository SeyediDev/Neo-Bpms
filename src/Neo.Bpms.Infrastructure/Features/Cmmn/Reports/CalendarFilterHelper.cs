namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

/// <summary>
/// Helper to inject calendar date range filter for Calendar chart type.
/// </summary>
public static class CalendarFilterHelper
{
    public static void InjectMonthRange(ElasticObject filters, int year, int month, string calendarType = "shamsi")
    {
        if (filters == null) return;
        var cal = NormalizeCalendarType(calendarType);
        var pc = new PersianCalendar();
        DateTime startDt, endDt;
        if (cal == "shamsi")
        {
            startDt = pc.ToDateTime(year, month, 1, 0, 0, 0, 0, PersianCalendar.PersianEra);
            var daysInMonth = pc.GetDaysInMonth(year, month);
            endDt = pc.ToDateTime(year, month, daysInMonth, 23, 59, 59, 0, PersianCalendar.PersianEra);
        }
        else
        {
            startDt = new DateTime(year, month, 1, 0, 0, 0);
            endDt = startDt.AddMonths(1).AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        filters.SetField("__CalendarDateFrom", startDt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        filters.SetField("__CalendarDateTo", endDt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }

    public static (int Year, int Month) GetCurrentMonth(string calendarType = "shamsi")
    {
        var pc = new PersianCalendar();
        var now = DateTime.Now;
        return NormalizeCalendarType(calendarType) == "shamsi"
            ? (pc.GetYear(now), pc.GetMonth(now))
            : (now.Year, now.Month);
    }

    private static string NormalizeCalendarType(string calendar)
    {
        if (string.IsNullOrEmpty(calendar)) return "shamsi";
        return calendar.Equals("miladi", StringComparison.OrdinalIgnoreCase) ? "gregorian" : calendar.ToLowerInvariant();
    }
}
