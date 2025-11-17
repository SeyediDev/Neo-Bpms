using System.Globalization;

namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions
{
    enum eTimeParamType
    {
        DateTime = 11,
        Date = 12,
        HourMinute = 15
    }

    /// <summary>
    /// دریافت ساعت از ورودی تاریخ و زمان
    /// </summary>
    /// <param name="odate">ورودی تاریخ و زمان</param>
    /// <returns></returns>
    public static int gethour(object odate)
    {
        try
        {
            DateTime date = FetchDateTime(odate);
            return date.Hour;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// دریافت ساعت از ورودی تاریخ و زمان
    /// </summary>
    /// <param name="odate">ورودی تاریخ و زمان</param>
    /// <returns></returns>
    public static int hour(object odate)
    {
        try
        {
            DateTime date = FetchDateTime(odate);
            return date.Hour;
        }
        catch
        {
            return 0;
        }
    }

    public static int gethour(string time, int type)
    {
        if ((eTimeParamType)type == eTimeParamType.HourMinute)
            return Convert.ToInt16(time) / 60;
        if ((eTimeParamType)type != eTimeParamType.Date && (eTimeParamType)type != eTimeParamType.DateTime)
            return 0;
        DateTime date = DateTime.Parse(time);
        return date.Hour;
    }

    /// <summary>
    /// دریافت تعداد دقیقه های زمان ورودی
    /// </summary>
    /// <param name="date">زمان ورودی</param>
    /// <returns></returns>
    public static int gethourminute(DateTime date)
    {
        return date.Hour * 60 + date.Minute;
    }

    public static int gethourminute(string time, int type)
    {
        if ((eTimeParamType)type == eTimeParamType.HourMinute)
            return Convert.ToInt16(time);
        if ((eTimeParamType)type != eTimeParamType.Date && (eTimeParamType)type != eTimeParamType.DateTime)
            return 0;
        DateTime date = DateTime.Parse(time);
        return gethourminute(date);
    }

    /// <summary>
    /// دریافت سال از تاریخ و زمان ورودی و نوع تقویم (در صورتی که نوع تقویم رشته 'S' باشد خروجی شمسی و در غیر اینصورت میلادی خواهد یود.)
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ و زمان ورودی</param>
    /// <returns></returns>
    public static int getyear(string Calendar, string Date)
    {
        DateTime date = (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
            ? feel_shamsidate(Date)
            : feel_date(Date);
        return date.Year;
    }

    /// <summary>
    /// دریافت سال از تاریخ و زمان ورودی و نوع تقویم (در صورتی که نوع تقویم رشته 'S' باشد خروجی شمسی و در غیر اینصورت میلادی خواهد یود.)
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ و زمان ورودی</param>
    /// <returns></returns>
    public static int getyeardatestr(string Calendar, string Date)
    {
        return getyear(Calendar, Date);
    }

    /// <summary>
    /// دریافت ماه از تاریخ و زمان ورودی و نوع تقویم (در صورتی که نوع تقویم رشته 'S' باشد خروجی شمسی و در غیر اینصورت میلادی خواهد یود.)
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ و زمان ورودی</param>
    /// <returns></returns>
    public static int getymonth(string Calendar, string Date)
    {
        DateTime date = (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
            ? feel_shamsidate(Date)
            : feel_date(Date);
        return date.Month;
    }

    /// <summary>
    /// دریافت ماه از تاریخ و زمان ورودی و نوع تقویم (در صورتی که نوع تقویم رشته 'S' باشد خروجی شمسی و در غیر اینصورت میلادی خواهد یود.)
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ و زمان ورودی</param>
    /// <returns></returns>
    public static int getymonthdatestr(string Calendar, string Date)
    {
        return getymonth(Calendar, Date);
    }

    /// <summary>
    /// دریافت روز از تاریخ و زمان ورودی و نوع تقویم (در صورتی که نوع تقویم رشته 'S' باشد خروجی شمسی و در غیر اینصورت میلادی خواهد یود.)
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ و زمان ورودی</param>
    /// <returns></returns>
    public static int getday(string Calendar, string Date)
    {
        DateTime date = (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
            ? feel_shamsidate(Date)
            : feel_date(Date);
        return date.Day;
    }

    /// <summary>
    /// دریافت روز از تاریخ و زمان ورودی و نوع تقویم (در صورتی که نوع تقویم رشته 'S' باشد خروجی شمسی و در غیر اینصورت میلادی خواهد یود.)
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ و زمان ورودی</param>
    /// <returns></returns>
    public static int getdaydatestr(string Calendar, string Date)
    {
        return getday(Calendar, Date);
    }

    /// <summary>
    /// دریافت تاریخ و زمان ورودی در رشته
    /// </summary>
    /// <param name="Date">تاریخ ورودی</param>
    /// <param name="time">زمان ورودی</param>
    /// <returns></returns>
    public static string getdaydatestr(string Date, int time = 0)
    {
        DateTime date = feel_date(Date);
        return date.Year + "/" + date.Month + "/" + date.Day +
               ((time != 0) ? ("<BR>" + time / 60 + ":" + time % 60) : ""); //todo <BR> ??
    }

    /// <summary>
    /// دریافت تاریخ و زمان ورودی در رشته
    /// </summary>
    /// <param name="date">تاریخ ورودی</param>
    /// <param name="time">زمان ورودی</param>
    /// <returns></returns>
    public static string getdaydatestr(DateTime date, int time = 0)
    {
        return date.Year + "/" + date.Month + "/" + date.Day +
               ((time != 0) ? ("<BR>" + time / 60 + ":" + time % 60) : "");
    }

    /// <summary>
    /// دریافت تاریخ ورودی بصورت شمسی با فرمت حروف (مثال :شنبه ۱۲ فروردین ۱۳۹۸)
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string getdatealphabetic(DateTime date)
    {
        return
            formatdate(date, "S", "fa", "dddd d mmmm yyyy");
    }

    private static DateTime getdate(string Calendar, string Date)
    {
        //todo what?
        string[] a = Date.Split('/');
        int year = Convert.ToInt16(a[0]), month = Convert.ToInt16(a[1]), day = Convert.ToInt16(a[2]);
        DateTime date;
        if (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            PersianCalendar pc = new();
            date = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        else
            date = new DateTime(year, month, day);

        return date;
    }

    public static long actualdate(string Calendar, string Date)
    {
        DateTime date = getdate(Calendar, Date);
        return date.ToFileTime();
    }

    public static DateTime samedayinfuturemonth(string Calendar, string Date, int MonthCount)
    {
        //todo what?
        string[] a = Date.Split('/');
        int year = Convert.ToInt16(a[0]), month = Convert.ToInt16(a[1]), day = Convert.ToInt16(a[2]);
        DateTime date;
        if (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            PersianCalendar pc = new();
            date = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            date = pc.AddMonths(date, MonthCount);
        }
        else
        {
            date = new DateTime(year, month, day);
            date = date.AddMonths(MonthCount);
        }

        return date;
    }

    /// <summary>
    /// دریافت چندمین هفته سال در نوع تقویم شمسی و میلادی
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="date">تاریخ ورودی</param>
    /// <returns></returns>
    public static int getweek(string Calendar, DateTime date)
    {
        if (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            PersianCalendar pc = new();
            return pc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Saturday);
        }
        GregorianCalendar c = new();
        return c.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }

    /// <summary>
    /// دریافت چندمین هفته سال در نوع تقویم شمسی و میلادی
    /// </summary>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Date">تاریخ ورودی</param>
    /// <returns></returns>
    public static int getweek(string Calendar, string Date)
    {
        DateTime date = getdate(Calendar, Date);
        return getweek(Calendar, date);
    }

    /// <summary>
    ///  دریافت چندمین ماه در تقویم شمسی و میلادی
    /// </summary>
    /// <param name="Calendar"></param>
    /// <param name="date">تاریخ ورودی</param>
    /// <returns></returns>
    public static int getmonth(string Calendar, DateTime date)
    {
        if (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            PersianCalendar pc = new();
            return pc.GetMonth(date);
        }

        return date.Month;
    }

    /// <summary>
    ///  دریافت چندمین ماه در تقویم شمسی و میلادی
    /// </summary>
    /// <param name="Calendar"></param>
    /// <param name="Date">تاریخ ورودی</param>
    /// <returns></returns>
    public static int getmonth(string Calendar, string Date)
    {
        DateTime date = getdate(Calendar, Date);
        return getmonth(Calendar, date);
    }

    public static int getseason(string Calendar, DateTime date) //todo get season ?
    {
        if (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            PersianCalendar pc = new();
            return pc.GetMonth(date) / 3;
        }

        return date.Month / 3;
    }

    public static int getseason(string Calendar, string Date)
    {
        DateTime date = getdate(Calendar, Date);
        return getseason(Calendar, date);
    }

    /// <summary>
    /// دریافت تاریخ ورودی بصورت شمسی و میلادی با فرمت حروف برای انتخاب شمسی مقدار نوع تقویم برابر 'S' و برای نوشتن فارسی نوع زبان  'Fa' میبایست باشد. 
    /// </summary>
    /// <param name="Date">تاریخ ورودی</param>
    /// <param name="Calendar">نوع تقویم</param>
    /// <param name="Lang">نوع زبان</param>
    /// <param name="Format">نوع فرمت خروجی</param>
    /// <returns></returns>
    public static string formatdate(string Date, string Calendar, string Lang, string Format)
    {
        DateTime date = getdate(Calendar, Date);
        return formatdate(date, Calendar, Lang, Format);
    }

    public static string formatdate(DateTime date, string Calendar, string Lang, string Format)
    {
        string output = "";
        int year, month, day, dy, hour, min, sec;
        DayOfWeek dw;
        if (Calendar.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            PersianCalendar pc = new();
            year = pc.GetYear(date);
            month = pc.GetMonth(date);
            day = pc.GetDayOfMonth(date);
            dw = pc.GetDayOfWeek(date);
            dy = pc.GetDayOfYear(date);
        }
        else
        {
            year = date.Year;
            month = date.Month;
            day = date.Day;
            dw = date.DayOfWeek;
            dy = date.DayOfYear;
        }

        hour = date.Hour;
        min = date.Minute;
        sec = date.Second;

        for (int i = 0, len = Format.Length; i < len; i++)
        {
            char c = Format[i];
            switch (c)
            {
                case 'y':
                case 's':
                case 'm':
                case 'w':
                case 'd':
                case 'H':
                case 'M':
                case 'S':
                    break;
                default:
                    output += c;
                    continue;
            }

            int j = 0;
            for (; Format[i + j] == c; j++)
            {
            }

            switch (c)
            {
                case 'y':
                    //AddFunctionArgumentValue("yy", "سال را به صورت 00-99 نشان می دهد");
                    //AddFunctionArgumentValue("yyyy", "سال را به صورت 1900-9999 نشان می دهد");
                    output += j switch
                    {
                        1 or 2 => (year / 100),
                        _ => year,
                    };
                    break;
                case 's':
                    //AddFunctionArgumentValue("s", "فصل از سال را به صورت 1-4 نشان می دهد");
                    //AddFunctionArgumentValue("ss", "فصل از سال را به صورت Spring-Winter نشان می دهد");
                    if (j == 2)
                    {
                        if (Lang == "fa")
                        {
                            if (month < 4)
                                output += "بهار";
                            else if (month < 8)
                                output += "تابستان";
                            else if (month < 12)
                                output += "پاییز";
                            else
                                output += "زمستان";
                        }
                        else
                        {
                            if (month < 4)
                                output += "Spring";
                            else if (month < 8)
                                output += "Summer";
                            else if (month < 12)
                                output += "Autumn";
                            else
                                output += "Winter";
                        }
                    }
                    else
                        output += (month / 3);

                    break;
                case 'm':
                    //AddFunctionArgumentValue("m", "ماه ها به صورت 1-12 نشان می دهد");
                    //AddFunctionArgumentValue("mm", "ماه ها به صورت 01-12 نشان می دهد");
                    //AddFunctionArgumentValue("mmm", "ماه ها به صورت jan-dec نشان می دهد");
                    //AddFunctionArgumentValue("mmmm", "ماه ها به صورت janury-december نشان می دهد");
                    //AddFunctionArgumentValue("mmmmm", "اولین حرف ماه ها را نشان می دهد");
                    switch (j)
                    {
                        case 1:
                            output += month;
                            break;
                        case 2:
                            if (month < 10)
                                output += "0" + month;
                            else
                                output += month;
                            break;
                        case 3:
                            if (Lang == "fa")
                            {
                                switch (month)
                                {
                                    case 1:
                                        output += "فرو";
                                        break;
                                    case 2:
                                        output += "ارد";
                                        break;
                                    case 3:
                                        output += "خرد";
                                        break;
                                    case 4:
                                        output += "تیر";
                                        break;
                                    case 5:
                                        output += "مرد";
                                        break;
                                    case 6:
                                        output += "شهر";
                                        break;
                                    case 7:
                                        output += "مهر";
                                        break;
                                    case 8:
                                        output += "آبن";
                                        break;
                                    case 9:
                                        output += "آذر";
                                        break;
                                    case 10:
                                        output += "دی";
                                        break;
                                    case 11:
                                        output += "بمن";
                                        break;
                                    case 12:
                                        output += "اسف";
                                        break;
                                }
                            }
                            else
                            {
                                switch (month)
                                {
                                    case 1:
                                        output += "jan";
                                        break;
                                    case 2:
                                        output += "feb";
                                        break;
                                    case 3:
                                        output += "mar";
                                        break;
                                    case 4:
                                        output += "apr";
                                        break;
                                    case 5:
                                        output += "may";
                                        break;
                                    case 6:
                                        output += "jun";
                                        break;
                                    case 7:
                                        output += "jul";
                                        break;
                                    case 8:
                                        output += "aug";
                                        break;
                                    case 9:
                                        output += "sep";
                                        break;
                                    case 10:
                                        output += "oct";
                                        break;
                                    case 11:
                                        output += "nov";
                                        break;
                                    case 12:
                                        output += "dec";
                                        break;
                                }
                            }

                            break;
                        default:
                            if (Lang == "fa")
                            {
                                switch (month)
                                {
                                    case 1:
                                        output += "فروردین";
                                        break;
                                    case 2:
                                        output += "اردیبهشت";
                                        break;
                                    case 3:
                                        output += "خرداد";
                                        break;
                                    case 4:
                                        output += "تیر";
                                        break;
                                    case 5:
                                        output += "مرداد";
                                        break;
                                    case 6:
                                        output += "شهریور";
                                        break;
                                    case 7:
                                        output += "مهر";
                                        break;
                                    case 8:
                                        output += "آبان";
                                        break;
                                    case 9:
                                        output += "آذر";
                                        break;
                                    case 10:
                                        output += "دی";
                                        break;
                                    case 11:
                                        output += "بهمن";
                                        break;
                                    case 12:
                                        output += "اسفند";
                                        break;
                                }
                            }
                            else
                            {
                                switch (month)
                                {
                                    case 1:
                                        output += "januray";
                                        break;
                                    case 2:
                                        output += "februray";
                                        break;
                                    case 3:
                                        output += "march";
                                        break;
                                    case 4:
                                        output += "april";
                                        break;
                                    case 5:
                                        output += "may";
                                        break;
                                    case 6:
                                        output += "june";
                                        break;
                                    case 7:
                                        output += "july";
                                        break;
                                    case 8:
                                        output += "august";
                                        break;
                                    case 9:
                                        output += "september";
                                        break;
                                    case 10:
                                        output += "october";
                                        break;
                                    case 11:
                                        output += "november";
                                        break;
                                    case 12:
                                        output += "december";
                                        break;
                                }
                            }

                            break;
                    }

                    break;
                case 'w':
                    //AddFunctionArgumentValue("w", "هفته از سال را به صورت 0-52 نشان می دهد");
                    //AddFunctionArgumentValue("ww", "هفته از سال را به صورت 00-52 نشان می دهد");
                    int w = getweek(Calendar, date);
                    if (w < 10 && j != 1)
                        output += "0";
                    output += w;
                    break;
                case 'd':
                    //AddFunctionArgumentValue("d", "روزها به صورت 1-31 نشان می دهد");
                    //AddFunctionArgumentValue("dd", "روزها به صورت 01-31 نشان می دهد");
                    //AddFunctionArgumentValue("ddd", "روزها به صورت sun-sat نشان می دهد");
                    //AddFunctionArgumentValue("dddd", "روزها به صورت sunday-saturday نشان می دهد");
                    //AddFunctionArgumentValue("ddddd", "شماره روز هفته را به صورت 1-7 نشان می دهد");
                    //AddFunctionArgumentValue("dddddd", "روزها به صورت 1-366 نشان می دهد");
                    //AddFunctionArgumentValue("ddddddd", "روزها به صورت 001-366 نشان می دهد");
                    switch (j)
                    {
                        case 1:
                            output += day;
                            break;
                        case 2:
                            if (day < 10)
                                output += "0";
                            output += day;
                            break;
                        case 3:
                            if (Lang == "fa")
                            {
                                switch (dw)
                                {
                                    case DayOfWeek.Friday:
                                        output += "ج";
                                        break;
                                    case DayOfWeek.Monday:
                                        output += "2ش";
                                        break;
                                    case DayOfWeek.Saturday:
                                        output += "ش";
                                        break;
                                    case DayOfWeek.Sunday:
                                        output += "1ش";
                                        break;
                                    case DayOfWeek.Thursday:
                                        output += "5ش";
                                        break;
                                    case DayOfWeek.Tuesday:
                                        output += "3ش";
                                        break;
                                    case DayOfWeek.Wednesday:
                                        output += "4ش";
                                        break;
                                }
                            }
                            else
                            {
                                switch (dw)
                                {
                                    case DayOfWeek.Friday:
                                        output += "fri";
                                        break;
                                    case DayOfWeek.Monday:
                                        output += "mon";
                                        break;
                                    case DayOfWeek.Saturday:
                                        output += "sat";
                                        break;
                                    case DayOfWeek.Sunday:
                                        output += "sun";
                                        break;
                                    case DayOfWeek.Thursday:
                                        output += "thu";
                                        break;
                                    case DayOfWeek.Tuesday:
                                        output += "tue";
                                        break;
                                    case DayOfWeek.Wednesday:
                                        output += "wed";
                                        break;
                                }
                            }

                            break;
                        case 4:
                            if (Lang == "fa")
                            {
                                switch (dw)
                                {
                                    case DayOfWeek.Friday:
                                        output += "جمعه";
                                        break;
                                    case DayOfWeek.Monday:
                                        output += "دوشنبه";
                                        break;
                                    case DayOfWeek.Saturday:
                                        output += "شنبه";
                                        break;
                                    case DayOfWeek.Sunday:
                                        output += "یکشنبه";
                                        break;
                                    case DayOfWeek.Thursday:
                                        output += "پنج‌شنبه";
                                        break;
                                    case DayOfWeek.Tuesday:
                                        output += "سه‌شنبه";
                                        break;
                                    case DayOfWeek.Wednesday:
                                        output += "چهارشنبه";
                                        break;
                                }
                            }
                            else
                            {
                                switch (dw)
                                {
                                    case DayOfWeek.Friday:
                                        output += "friday";
                                        break;
                                    case DayOfWeek.Monday:
                                        output += "monday";
                                        break;
                                    case DayOfWeek.Saturday:
                                        output += "saturday";
                                        break;
                                    case DayOfWeek.Sunday:
                                        output += "sunday";
                                        break;
                                    case DayOfWeek.Thursday:
                                        output += "thursday";
                                        break;
                                    case DayOfWeek.Tuesday:
                                        output += "tuesday";
                                        break;
                                    case DayOfWeek.Wednesday:
                                        output += "wednesday";
                                        break;
                                }
                            }

                            break;
                        case 5:
                            output += (int)dw;
                            break;
                        case 6:
                            output += dy;
                            break;
                        case 7:
                            if (dy < 10)
                                output += "00";
                            else if (dy < 100)
                                output += "0";
                            output += dy;
                            break;
                    }

                    break;
                case 'H':
                    //AddFunctionArgumentValue("H", "ساعت را به صورت 0-23 نشان می دهد");
                    //AddFunctionArgumentValue("HH", "ساعت را به صورت 00-23 نشان می دهد");
                    //AddFunctionArgumentValue("HHH", "ساعت را به صورت 0-12 نشان می دهد");
                    //AddFunctionArgumentValue("HHHH", "ساعت را به صورت 00-12 نشان می دهد");
                    //AddFunctionArgumentValue("HHHHH", "ساعت را به صورت 0-12 am/pm نشان می دهد");
                    //AddFunctionArgumentValue("HHHHHH", "ساعت را به صورت 00-12 am/pm نشان می دهد");
                    switch (j)
                    {
                        case 1:
                            output += hour;
                            break;
                        case 2:
                            if (hour < 10)
                                output += "0";
                            output += hour;
                            break;
                        case 3:
                        case 4:
                            if (hour == 12)
                                output += hour;
                            else
                            {
                                hour %= 12;
                                if (j == 4 && hour < 10)
                                    output += "0";
                                output += hour;
                            }

                            break;
                        case 6:
                        case 5:
                            bool am = hour <= 12;
                            if (hour == 12)
                                output += hour + " " + (Lang == "fa" ? "ظ" : "n");
                            else
                            {
                                hour %= 12;
                                if (j == 6 && hour < 10)
                                    output += "0";
                                output += hour;
                            }

                            if (am)
                                output += " " + (Lang == "fa" ? "ق‌ظ" : "am");
                            else
                                output += " " + (Lang == "fa" ? "ب‌ظ" : "pm");
                            break;
                    }

                    break;
                case 'M':
                    //AddFunctionArgumentValue("M", "دقیقه را به صورت 0-59 نشان می دهد");
                    //AddFunctionArgumentValue("MM", "دقیقه را به صورت 00-59 نشان می دهد");
                    //AddFunctionArgumentValue("MMM", "دقیقه روز را به صورت 0-1439 نشان می دهد");
                    switch (j)
                    {
                        default: //case 1:
                            output += min;
                            break;
                        case 2:
                            if (min < 10)
                                output += "0";
                            output += min;
                            break;
                        case 3:
                            output += hour * 60 + min;
                            break;
                    }

                    break;
                case 'S':
                    //AddFunctionArgumentValue("S", "ثانیه را به صورت 0-59 نشان می دهد");
                    //AddFunctionArgumentValue("SS", "ثانیه را به صورت 00-59 نشان می دهد");
                    //AddFunctionArgumentValue("SSS", "ثانیه ساعت را به صورت 0-3599 نشان می دهد");
                    //AddFunctionArgumentValue("SSSS", "ثانیه روز را به صورت 0-86399 نشان می دهد");
                    switch (j)
                    {
                        default: //case 1:
                            output += sec;
                            break;
                        case 2:
                            if (sec < 10)
                                output += "0";
                            output += sec;
                            break;
                        case 3:
                            output += min * 60 + sec;
                            break;
                        case 4:
                            output += (hour * 60 + min) * 60 + sec;
                            break;
                    }

                    break;
                default:
                    //if (c == '\\')
                    //	output += Format[++i];
                    //else
                    output += c;
                    continue;
            }
        }

        return output;
    }

    /// <summary>
    /// دریافت تفاوت تاریخ های ورودی بصورت زمان
    /// </summary>
    /// <param name="date1">تاریخ ورودی</param>
    /// <param name="date2">تاریخ ورودی</param>
    /// <returns></returns>
    public static TimeSpan differ(DateTime date1, DateTime date2)
    {
        return (date1 > date2) ? (date1 - date2) : (date2 - date1);
    }

    /// <summary>
    /// دریافت تفاوت سال تاریخ های ورودی بصورت زمان
    /// </summary>
    /// <param name="date1">تاریخ ورودی</param>
    /// <param name="date2">تاریخ ورودی</param>
    /// <returns></returns>
    public static int yeardiffer(DateTime date1, DateTime date2)
    {
        return date1.Year - date2.Year;
    }

    /// <summary>
    /// دریافت تفاوت ماه تاریخ های ورودی بصورت زمان
    /// </summary>
    /// <param name="date1">تاریخ ورودی</param>
    /// <param name="date2">تاریخ ورودی</param>
    /// <returns></returns>
    public static int monthdiffer(DateTime date1, DateTime date2)
    {
        return date1.Year * 12 + date1.Month - (date2.Year * 12 + date2.Month);
    }

    /// <summary>
    /// دریافت تفاوت روز تاریخ های ورودی بصورت زمان
    /// </summary>
    /// <param name="date1">تاریخ ورودی</param>
    /// <param name="date2">تاریخ ورودی</param>
    /// <returns></returns>
    public static int DayDiffer(object date1, object date2)
    {
        DateTime d1 = DateTimeOf(date1);
        DateTime d2 = DateTimeOf(date2);
        return (d1 - d2).Days;
    }

    /// <summary>
    /// دریافت تفاوت ماه تاریخ های ورودی با نوع تقویم بصورت زمان
    /// </summary>
    /// <param name="Date1">تاریخ ورودی</param>
    /// <param name="Date2">تاریخ ورودی</param>
    /// <param name="Calendar">نوع تقویم</param>
    /// <returns></returns>
    public static int monthdiffer(string Calendar, string Date1, string Date2)
    {
        DateTime date1 = getdate(Calendar, Date1);
        DateTime date2 = getdate(Calendar, Date2);
        return monthdiffer(date1, date2);
    }

    /// <summary>
    /// دریافت تفاوت روز تاریخ های ورودی با نوع تقویم بصورت زمان
    /// </summary>
    /// <param name="Date1">تاریخ ورودی</param>
    /// <param name="Date2">تاریخ ورودی</param>
    /// <param name="Calendar">نوع تقویم</param>
    /// <returns></returns>
    public static int daydiffer(string Calendar, string Date1, string Date2)
    {
        DateTime date1 = getdate(Calendar, Date1);
        DateTime date2 = getdate(Calendar, Date2);
        return DayDiffer(date1, date2);
    }

    /// <summary>
    /// دریافت سال تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static int year(object odate)
    {
        try
        {
            DateTime date = FetchDateTime(odate);
            return date.Year;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// دریافت ماه تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static int month(object odate)
    {
        try
        {
            DateTime date = FetchDateTime(odate);
            return date.Month;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// دریافت روز تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static int day(object odate)
    {
        try
        {
            DateTime date = FetchDateTime(odate);
            return date.Day;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// اضافه کردن تعداد روز به تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <param name="adddays">تعداد روز</param>
    /// <returns></returns>
    public static DateTime AddDays(object odate, object adddays)
    {
        try
        {
            adddays = Pure(adddays);
            DateTime oldDate = FetchDateTime(odate);
            return oldDate.AddDays(Convert.ToInt32(adddays));
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// کم کردن تعداد روز به تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <param name="minusDays">تعداد روز</param>
    /// <returns></returns>
    public static DateTime MinusDays(object odate, object minusDays)
    {
        try
        {
            minusDays = Pure(minusDays);
            DateTime oldDate = FetchDateTime(odate);
            return oldDate.AddDays(-Convert.ToInt32(minusDays));
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// دریافت تفاضل روز های تاریخ های ورودی
    /// </summary>
    /// <param name="date1">تاریخ ورودی</param>
    /// <param name="date2">تاریخ ورودی</param>
    /// <returns></returns>
    public static DateTime MinusDate(object date1, object date2)
    {
        try
        {
            DateTime oldDate1 = FetchDateTime(date1);
            DateTime oldDate2 = FetchDateTime(date2);
            int d = (oldDate1 - oldDate2).Days;
            return oldDate1.AddDays(-d);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// دریافت تاریخ روردی در ماه آینده
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static DateTime NextMonthDate(object odate)
    {
        try
        {
            DateTime oldDate = FetchDateTime(odate);
            int y = oldDate.Year + ((oldDate.Month == 12) ? 1 : 0);
            int m = (oldDate.Month == 12) ? 1 : (oldDate.Month + 1);
            return new DateTime(y, m, 1);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// دربافت اولین روز ماه تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static DateTime FirstOfMonthDate(object odate)
    {
        try
        {
            DateTime oldDate = FetchDateTime(odate);
            return new DateTime(oldDate.Year, oldDate.Month, 1);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    public static DateTime NextMonthDate2(object oyear, object omonth, object oday)
    {
        FetchYearMonthDay(ref oyear, ref omonth, ref oday, out int year, out int month, out int day);
        int y = year + ((month == 12) ? 1 : 0);
        int m = (month == 12) ? 1 : (month + 1);
        try
        {
            return new DateTime(y, m, day);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// دریافت فیلد تاریخی
    /// </summary>
    /// <param name="oyear">سال</param>
    /// <param name="omonth">ماه</param>
    /// <param name="oday">روز</param>
    /// <returns></returns>
    public static DateTime GetDate(object oyear, object omonth, object oday)
    {
        FetchYearMonthDay(ref oyear, ref omonth, ref oday, out int year, out int month, out int day);
        try
        {
            return new DateTime(year, month, day);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// دریافت روز تاریخ ورودی
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static DateTime DateOf(object odate)
    {
        return DateTimeOf(odate).Date;
    }

    /// <summary>
    /// دریافت خروجی از نوع تاریخ
    /// </summary>
    /// <param name="odate">تاریخ ورودی</param>
    /// <returns></returns>
    public static DateTime DateTimeOf(object odate)
    {
        try
        {
            return FetchDateTime(odate);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// دریافت خروجی از نوع مدت
    /// </summary>
    /// <param name="oTimeSpan">مدت ورودی</param>
    /// <returns></returns>
    public static TimeSpan TimeSpanOf(object oTimeSpan)
    {
        oTimeSpan = Pure(oTimeSpan);
        return ConvUtill.TimeSpanOf(oTimeSpan);
    }

    /// <summary>
    /// دریافت کوچکترین تاریخ
    /// </summary>
    /// <returns></returns>
    public static DateTime DateTimeMinValue()
    {
        return DateTime.MinValue;
    }

    /// <summary>
    /// دریافت بزرگترین تاریخ
    /// </summary>
    /// <returns></returns>
    public static DateTime DateTimeMaxValue()
    {
        return DateTime.MaxValue;
    }

    public static string ToTimeInputValue(TimeSpan time)
    {
        return time.ToTimeInputValue();
    }
    /// <summary>
    /// دریافت روز در سال
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int DayOfYear(object dateTime)
    {
        return DateTime.TryParse(dateTime.ToString(), out DateTime d) ? d.DayOfYear : 0;
    }

    private static DateTime FetchDateTime(object odate)
    {
        odate = Pure(odate);
        DateTime oldDate;
        if (odate is string)
            DateTime.TryParse(odate.ToString(), out oldDate);
        else
            oldDate = Convert.ToDateTime(odate);
        return oldDate;
    }

    private static void FetchYearMonthDay(ref object oyear, ref object omonth, ref object oday, out int year,
        out int month, out int day)
    {
        oyear = Pure(oyear);
        omonth = Pure(omonth);
        oday = Pure(oday);
        year = Convert.ToInt16(oyear);
        month = Convert.ToInt16(omonth);
        day = Convert.ToInt16(oday);
    }

    private static object Pure(object obj)
    {
        return obj is IExpressionValue value ? value.InternalValue : obj;
    }
}