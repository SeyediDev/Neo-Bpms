using System.Globalization;

namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions : IFunctionImplementations
{
    #region feel conversion functions

    public static DateTime feel_date(string from)
    {
        return DateTime.Parse(from).Date;
    }

    public static DateTime feel_date(DateTime from)
    {
        return from.Date;
    }

    public static DateTime feel_dateandtime(string from)
    {
        return DateTime.Parse(from);
    }

    public static TimeSpan feel_time(string from)
    {
        return DateTime.Parse(from).TimeOfDay;
    }

    public static TimeSpan feel_time(DateTime from)
    {
        return from.TimeOfDay;
    }

    public static double feel_decimal(double from, int scale)
    {
        double pscale = Math.Pow(10, scale);
        return Math.Floor(from * pscale) / pscale;
    }

    public static double feel_number(string from, string groupSeperator, string decimalSeperator)
    {
        NumberFormatInfo formatInfo = new()
        {
            NumberGroupSeparator = groupSeperator,
            NumberDecimalSeparator = decimalSeperator
        };
        if (!double.TryParse(from,
            NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands,
            formatInfo, out double d))
            d = 0;
        return d;
    }

    public static string feel_groupdigit(double from)
    {
        NumberFormatInfo formatInfo = new() { NumberGroupSeparator = ",", NumberDecimalSeparator = "." };
        return from.ToString(formatInfo);
    }

    public static double feel_number(string from)
    {
        return feel_number(from.Replace("/", "."), ",", ".");
    }

    public static string feel_getalphabetic(double from)
    {
        long d1 = (long)Math.Floor(from),
            d2 = (long)((from - d1) * 1000); //read only 3 floating digits
        if (d2 > 0)
        {
            if (d2 % 100 == 0) d2 /= 100;
            else if (d2 % 10 == 0) d2 /= 10;
            string s = getalphabetic(d1) + " ممیز " + getalphabetic(d2);
            if (((from - d1) * 1000) > d2)
                s += "..."; //if more than 3 floating digits
            return s;
        }

        return getalphabetic(d1);
    }

    private static string[] thousands = ["هزار", "میلیون", "میلیارد", "بیلیون",];

    static readonly string[] hundreds =
        ["یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد",];

    static readonly string[] tens = ["ده", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتصاد", "نود",];

    static readonly string[] ones =
    [
        "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه", "ده",
        "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفتده", "هجده", "نوزده",
    ];

    private static string getalphabetic(long d)
    {
        string s = "";
        if (d < 1000)
        {
            if (d >= 100)
            {
                s += hundreds[d / 100];
                d %= 100;
                if (d == 0) return s;
                s += " و ";
            }

            if (d == 10 || d >= 20)
            {
                s += tens[d / 10];
                d %= 10;
                if (d == 0) return s;
                s += " و ";
            }

            return d == 0 ? "صفر" : ones[d];
        }

        if (d > 999999999999999)
        {
            //read very big numbers 3 digits by 3 digits:
            while (d > 0)
            {
                long d1 = d % 1000;
                d /= 1000;
                if (d1 == 0) s += "سه صفر";
                else if (d1 < 10) s = s + "دو صفر " + getalphabetic(d1);
                else if (d1 < 100) s = s + "صفر " + getalphabetic(d1);
                else s += getalphabetic(d1);
                if (d > 0)
                    s += "، ";
            }
        }

        if (d > 999999999999)
        {
            s = getalphabetic(d / 1000000000000) + " بیلیون";
            d %= 1000000000000;
            if (d == 0) return s;
            s += " و ";
        }

        if (d > 999999999)
        {
            s = getalphabetic(d / 1000000000) + " میلیارد";
            d %= 1000000000;
            if (d == 0) return s;
            s += " و ";
        }

        if (d > 999999)
        {
            s = getalphabetic(d / 1000000) + " میلیون";
            d %= 1000000;
            if (d == 0) return s;
            s += " و ";
        }

        if (d > 999)
        {
            s = getalphabetic(d / 1000) + " هزار";
            d %= 1000;
            if (d == 0) return s;
            s += " و ";
        }

        return s + getalphabetic(d);
    }

    public static int feel_int(string from)
    {
        return int.Parse(from);
    }

    public string feel_string(object from)
    {
        return from.ToString();
    }

    public static TimeSpan duration(string from)
    {
        return TimeSpan.Parse(from);
    }

    #endregion feel conversion functions

    #region extended conversion functions

    public static bool feel_bool(object from)
    {
        return from is bool
            ? (bool)from
            : from is decimal || from is double || from is float || from is int
            ? (bool)((dynamic)from != 0)
            : from is string ? !string.IsNullOrWhiteSpace(from as string) && (from as string != "0") : from != null;
    }

    public static IEnumerable<object> feel_list(object from)
    {
        IEnumerable<object> list = from as IEnumerable<object>;
        if (list != null)
            return feel_flatten(list);
        List<object> list1 = [@from];
        return list1;
    }

    public static DateTime feel_shamsidate(string from)
    {
        string[] b = from.Split('/');
        if (b.Length < 3) return DateTime.MinValue;
        int year = Convert.ToInt16(b[0]), month = Convert.ToInt16(b[1]), day = Convert.ToInt16(b[2]);
        PersianCalendar pc = new();
        return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
    }

    public static DateTime feel_shamsidateandtime(string from)
    {
        char[] sep = [' '];
        string[] a = from.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        if (a.Length < 1) return DateTime.MinValue;
        string[] b = a[0].Split('/');
        if (b.Length < 3) return DateTime.MinValue;
        int year = Convert.ToInt16(b[0]), month = Convert.ToInt16(b[1]), day = Convert.ToInt16(b[2]);
        int hour = 0, min = 0, sec = 0;
        if (a.Length > 1)
        {
            b = a[1].Split(':');
            if (b.Length > 1)
            {
                hour = Convert.ToInt16(b[0]);
                min = Convert.ToInt16(b[1]);
                if (b.Length > 2)
                    sec = Convert.ToInt16(b[2]);
            }
        }

        PersianCalendar pc = new();
        return pc.ToDateTime(year, month, day, hour, min, sec, 0);
    }

    #endregion extended conversion functions

    public static IEnumerable<object> List(params object[] inLists)
    {
        if (inLists == null || inLists.Length == 0)
            return new List<object>();
        if (inLists.Length != 1)
            return new List<object>(inLists);
        object inList = inLists[0];
        if (inList == null || inList.ToString() == "undefined")
            return new List<object>();
        if (inList is IEnumerable<object>)
            return (IEnumerable<object>)inList;
        if (inList is string)
        {
            string sList = inList.ToString();
            if (sList.StartsWith("{") && sList.EndsWith("}"))
                sList = sList[1..^1];
            return sList.Split(',');
        }

        return new List<object> { inList };
    }
}