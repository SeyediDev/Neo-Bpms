using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;

namespace Neo.Bpms.Domain.Utility;

public static class DateTimeBindingExtensions
{
    public static TimeSpan ToTimeSpan(this string timeString)
    {
        if (string.IsNullOrEmpty(timeString)) throw new ArgumentNullException();
        DateTime dt = DateTime.ParseExact(timeString, "H:mm", CultureInfo.InvariantCulture);
        //            var dt = DateTime.ParseExact(timeString, "h:mm tt", CultureInfo.InvariantCulture);
        return dt.TimeOfDay;
    }

    public static string ToTimeInputValue(this TimeSpan time)
    {
        return time.ToString(time.Days > 0 ? "dd\\:hh\\:mm\\:ss\\.fff" : "hh\\:mm\\:ss\\.fff");
    }

    /// <summary>
    /// based on html5 time input type
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string ToHtmlInputValue(this TimeSpan time)
    {
        return time.Hours.ToString("d2") + ":" + time.Minutes.ToString("d2");
    }

    /// <summary>
    /// based on our convention
    /// </summary>
    /// <param name="date"></param>
    /// <param name="calendarType">miladi or shamsi</param>
    /// <returns></returns>
    public static string ToHtmlInputValue(this DateTime date, string calendarType)
    {
        //if(calendarType == null)
        //    calendarType = ProjectDefinition.Project.DefaultCalendar;
        if (calendarType.Equals("miladi"))
            return date.ToString("yyyy/MM/dd");
        if (calendarType.Equals("shamsi"))
        {
            PersianCalendar pc = new();
            string month = pc.GetMonth(date).ToString("d2");
            string day = pc.GetDayOfMonth(date).ToString("d2");
            return pc.GetYear(date).ToString() + '/' + month + '/' + day;
        }

        throw new ArgumentException($"{nameof(calendarType)} isn't valid: {calendarType}");
    }

    public static string ToIso8601(this DateTime date)
    {
        return date.ToString("o");
    }

    /// <summary>
    /// based on our convention
    /// </summary>
    /// <param name="date"></param>
    /// <param name="calendarType">miladi or shamsi</param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static string ToReadableText(this DateTime date,
        string calendarType, ReadablePrecision precision = ReadablePrecision.Time)
    {
        if (date == DateTime.MinValue)
            return string.Empty;
        //if (calendarType == null)
        //    calendarType = ProjectDefinition.Project.DefaultCalendar;
        if (calendarType.Equals("miladi"))
            return precision == ReadablePrecision.Time
                ? date.ToString(CultureInfo.InvariantCulture)
                : date.ToString("d");
        if (calendarType.Equals("shamsi"))
        {
            PersianCalendar pc = new();
            string month = pc.GetMonth(date).ToString("d2");
            string day = pc.GetDayOfMonth(date).ToString("d2");
            return $"{pc.GetYear(date)}/{month}/{day}" +
                   (precision == ReadablePrecision.Time ? $" {date.Hour}:{date.Minute}:{date.Second}" : "");
        }

        throw new ArgumentException($"{nameof(calendarType)} isn't valid: {calendarType}");
    }

    public static DateTime ToDateTime(this string dateString, string calendarType)
    {
        return string.IsNullOrEmpty(dateString)
            ? throw new ArgumentNullException()
            : calendarType.Equals("miladi")
            ? dateString.ToDateTimeFromMiladi()
            : calendarType.Equals("shamsi")
            ? dateString.ToDateTimeFromShamsi()
            : throw new ArgumentException($"{nameof(calendarType)} isn't valid: {calendarType}");
    }

    public static DateTime ToDateTimeFromMiladi(this string dateString)
    {
        return Convert.ToDateTime(dateString);
    }

    public static DateTime ToDateTimeFromShamsi(this string dateString)
    {
        string[] dateParts = dateString.Split('/');
        if (dateParts.Length != 3) throw new ArgumentException();

        PersianCalendar pc = new();
        DateTime dt = pc.ToDateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]),
            Convert.ToInt32(dateParts[2]), 0, 0, 0, 0);
        return dt;
    }
}

public enum ReadablePrecision
{
    Date,
    Time
}
