using System.Globalization;
using System.Text.RegularExpressions;

namespace Neo.Bpms.Domain.Utility.PersianDateTime;

public partial class DateUtil
{
    /// <summary>

    /// Gets the persian shamsi name of the month.

    /// </summary>

    /// <param name="month">The month.</param>

    /// <returns></returns>

    /// <exception cref="ArgumentOutOfRangeException">Invalid month number</exception>

    public static string GetMonthName(int month)
    {
        return month switch
        {
            1 => "فروردین",
            2 => "اردیبهشت",
            3 => "خرداد",
            4 => "تیر",
            5 => "مرداد",
            6 => "شهریور",
            7 => "مهر",
            8 => "آبان",
            9 => "آذر",
            10 => "دی",
            11 => "بهمن",
            12 => "اسفند",
            _ => throw new ArgumentOutOfRangeException(nameof(month)),
        };
    }


    /// <summary>

    /// Gets the abbreviated name of the month.

    /// </summary>

    /// <param name="month">The month.</param>

    /// <returns></returns>

    /// <exception cref="ArgumentOutOfRangeException">Invalid month number</exception>

    public static string GetAbbreviatedMonthName(int month)
    {
        return month switch
        {
            1 => "فر",
            2 => "ار",
            3 => "خر",
            4 => "تی",
            5 => "مر",
            6 => "شه",
            7 => "مه",
            8 => "آب",
            9 => "آذ",
            10 => "دی",
            11 => "به",
            12 => "اس",
            _ => throw new ArgumentOutOfRangeException(nameof(month)),
        };
    }

    /// <summary>

    /// Gets the persian day of week.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetPersianDayOfWeek(DateTime christDateTime)
    {
        PersianCalendar calendar = new();

        DayOfWeek dayOfWeek = calendar.GetDayOfWeek(christDateTime);
        return GetDayName(dayOfWeek);
    }

    /// <summary>

    /// Gets the name of the day.

    /// </summary>

    /// <param name="day">The day.</param>

    /// <returns></returns>

    /// <exception cref="ArgumentOutOfRangeException">Invalid day of week</exception>

    public static string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Saturday => "شنبه",
            DayOfWeek.Sunday => "یکشنبه",
            DayOfWeek.Monday => "دوشنبه",
            DayOfWeek.Tuesday => "سه شنبه",
            DayOfWeek.Wednesday => "چهارشنبه",
            DayOfWeek.Thursday => "پنجشنبه",
            DayOfWeek.Friday => "جمعه",
            _ => throw new ArgumentOutOfRangeException(nameof(day)),
        };
    }
    /// <summary>

    /// Gets the name of the abbreviated day.

    /// </summary>

    /// <param name="day">The day.</param>

    /// <returns></returns>

    /// <exception cref="ArgumentOutOfRangeException">Invalid day of week</exception>

    public static string GetAbbreviatedDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Saturday => "ش",
            DayOfWeek.Sunday => "ی",
            DayOfWeek.Monday => "د",
            DayOfWeek.Tuesday => "س",
            DayOfWeek.Wednesday => "چ",
            DayOfWeek.Thursday => "پ",
            DayOfWeek.Friday => "ج",
            _ => throw new ArgumentOutOfRangeException(nameof(day)),
        };
    }
    /// <summary>

    /// Gets the first day of week.

    /// </summary>

    /// <value>

    /// The first day of week.

    /// </value>

    public static DayOfWeek FirstDayOfWeek => DayOfWeek.Saturday;

    /// <summary>

    /// Gets the short persian time string.

    /// </summary>

    /// <param name="Time">The time.</param>

    /// <returns></returns>

    public static string GetShortPersianTimeString(int Time)
    {
        int min = Time / 60;
        int sec = Time % 60;
        return min + ":" + sec;
    }
    /// <summary>

    /// Gets the short persian date string.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetShortPersianDateString(string christDateTime)
    {
        if (string.IsNullOrEmpty(christDateTime))
        {
            return "";
        }

        string[] arrDate = christDateTime.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (IsShamsi(arrDate))
        {
            return christDateTime;
        }

        if (arrDate.Length < 3)
        {
            return "";
        }

        int year = NumberUtill.ToInt(arrDate[0]);
        int month = NumberUtill.ToInt(arrDate[1]);
        int day = NumberUtill.ToInt(arrDate[2]);

        if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31)
        {
            return "";
        }

        DateTime dt = new(year, month, day);
        return GetShortPersianDateString(dt);
    }

    /// <summary>

    /// Determines whether the specified date is shamsi.

    /// </summary>

    /// <param name="arrDate">The arr date.</param>

    /// <returns></returns>

    public static bool IsShamsi(string[] arrDate)
    {
        if (arrDate.Length < 3)
        {
            return false;
        }

        string YearFirstPart = arrDate[0][..2];
        return YearFirstPart is not "19" and not "20" and not "21";
    }


    /// <summary>

    /// Gets the short persian date string.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetShortPersianDateString(DateTime christDateTime)
    {
        PersianCalendar calendar = new();

        string Month = calendar.GetMonth(christDateTime).ToString();
        Month = Month.Length == 1 ? ("0" + Month) : Month;

        string Day = calendar.GetDayOfMonth(christDateTime).ToString();
        Day = Day.Length == 1 ? ("0" + Day) : Day;

        return $@"{calendar.GetYear(christDateTime)}/{Month}/{Day}";
    }

    /// <summary>

    /// Gets the long persian date string.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetLongPersianDateString(DateTime christDateTime)
    {
        PersianCalendar calendar = new();
        return
            $@"{calendar.GetYear(christDateTime)} {GetMonthName(calendar.GetMonth(christDateTime))} {calendar.GetDayOfMonth(christDateTime)}";
    }
    /// <summary>

    /// Gets the short persian date string reverse.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetShortPersianDateStringReverse(DateTime christDateTime)
    {
        PersianCalendar calendar = new();
        string Month = calendar.GetMonth(christDateTime).ToString();
        Month = Month.Length == 1 ? ("0" + Month) : Month;

        string Day = calendar.GetDayOfMonth(christDateTime).ToString();
        Day = Day.Length == 1 ? ("0" + Day) : Day;


        return string.Format(@"{2}/{1}/{0}", calendar.GetYear(christDateTime),
             Month, Day);
    }

    /// <summary>

    /// Gets the long persian date string reverse.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetLongPersianDateStringReverse(DateTime christDateTime)
    {
        PersianCalendar calendar = new();
        return
            $@"{calendar.GetDayOfMonth(christDateTime)} {GetMonthName(calendar.GetMonth(christDateTime))} {calendar.GetYear(christDateTime)}";
    }

    /// <summary>

    /// Checks the date range.

    /// </summary>

    /// <param name="month">The month.</param>

    /// <param name="day">The day.</param>

    /// <exception cref="ApplicationException">Not a valid Hijri Shamsi Sate string</exception>

    private static void CheckDateRange(int month, int day)
    {
        if (month is <= 0 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        if (month > 12 || month < 1 || day > 31 || day < 1 || (month > 6 && day > 30))
        {
            throw new ApplicationException("Not a valid Hijri Shamsi Sate string");
        }

    }

    /// <summary>

    /// Parses the specified persian date.

    /// </summary>

    /// <param name="persianDate">The persian date.</param>

    /// <returns></returns>

    public static DateTime Parse(string persianDate)
    {
        Regex re = new(@"(\d{2,4})/(\d{1,2})/(\d{1,2})");
        Regex revRegex = new(@"(\d{1,2})/(\d{1,2})/(\d{2,4})");
        Match m;
        int Year = 0;
        int Month = 0;
        int Day = 0;
        if (re.IsMatch(persianDate))
        {
            m = re.Match(persianDate);
            Year = int.Parse(m.Groups[1].Value);
            Month = int.Parse(m.Groups[2].Value);
            Day = int.Parse(m.Groups[3].Value);
        }
        else if (revRegex.IsMatch(persianDate))
        {
            m = revRegex.Match(persianDate);
            Year = int.Parse(m.Groups[3].Value);
            Month = int.Parse(m.Groups[2].Value);
            Day = int.Parse(m.Groups[1].Value);
        }
        CheckDateRange(Month, Day);
        PersianCalendar calendar = new();
        return calendar.ToDateTime(Year, Month, Day, 0, 0, 0, 0);
    }

    /// <summary>

    /// Gets the persian month of year.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetPersianMonthOfYear(DateTime christDateTime)
    {
        PersianCalendar calendar = new();
        int month = calendar.GetMonth(christDateTime);
        return GetMonthName(month);
    }
    /// <summary>

    /// Gets the persian year.

    /// </summary>

    /// <param name="christDateTime">The christ date time.</param>

    /// <returns></returns>

    public static string GetPersianYear(DateTime christDateTime)
    {
        PersianCalendar calendar = new();
        string year = calendar.GetYear(christDateTime).ToString();
        return year;
    }


    /// <summary>

    /// converts Shamsi date string to miladi date.

    /// </summary>

    /// <param name="shamsi">The shamsi date.</param>

    /// <returns></returns>

    public static DateTime Shamsi2Miladi(string shamsi)
    {
        if (shamsi == string.Empty)
        {
            return DateTime.UtcNow;
        }
        else
        {
            try
            {
                PersianCalendar Mdate = new();
                string[] prdate = shamsi.Split('/');
                DateTime shamsidate = Mdate.ToDateTime(Convert.ToInt32(prdate[0]), Convert.ToInt32(prdate[1]), Convert.ToInt32(prdate[2]), 1, 1, 1, 1, GregorianCalendar.ADEra);
                return Convert.ToDateTime(shamsidate.ToShortDateString());
            }
            catch { return DateTime.UtcNow; }
        }
    }
}
