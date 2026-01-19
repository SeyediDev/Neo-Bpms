namespace Neo.Bpms.Domain.Utility.PersianDateTime;

public partial class DateUtil
{
    public static DateTime TodayDate => DateTime.UtcNow.Date;

    public static void SplitDate(string date, out int year, out int month, out int day)
    {
        year = month = day = 0;
        string[] arrDate = date.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (arrDate.Length != 3)
        {
            arrDate = date.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        if (arrDate.Length != 3)
        {
            return;
        }

        year = Convert.ToInt32(arrDate[0]);
        month = Convert.ToInt32(arrDate[1]);
        day = Convert.ToInt32(arrDate[2]);
    }

    /// <summary>
    /// با فرمت 
    /// <para>yyyy/mm/dd</para>
    /// </summary>
    /// <param name="date"></param>
    public static DateTime GetDate(string date)
    {
        SplitDate(date, out int year, out int month, out int day);
        return year == 0 || month == 0 || day == 0 ? new DateTime() : new DateTime(year, month, day);
    }
    /// <summary>
    /// با فرمت 
    /// <para>yyyy/mm/dd hh:mm</para>
    /// </summary>
    /// <param name="date"></param>
    public static DateTime GetDateTime(string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.MinValue;
        }

        int hour = 0;
        int minutes = 0, second = 0;
        SplitDate(date, out int year, out int month, out int day);
        if (year == 0 || month == 0 || day == 0)
        {
            return new DateTime();
        }

        string[] arrDate0 = date.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (arrDate0.Length <= 1)
        {
            return new DateTime(year, month, day, hour, minutes, second);
        }

        string[] arrDate = arrDate0[1].Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (arrDate.Length > 0)
        {
            hour = NumberUtill.ToInt(arrDate[0]);
        }

        if (arrDate.Length > 1)
        {
            minutes = NumberUtill.ToInt(arrDate[1]);
        }

        if (arrDate.Length > 2)
        {
            second = NumberUtill.ToInt(arrDate[2]);
        }

        return new DateTime(year, month, day, hour, minutes, second);
    }

    public static void SplitTime(string Time, out string Hour, out string Minutes, out string Seccond)
    {
        Hour = Minutes = Seccond = "";
        string[] arrDate = Time.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (arrDate.Length != 3)
        {
            return;
        }

        Hour = arrDate[0];
        Minutes = arrDate[1];
        Seccond = arrDate[2];
    }

    public static string GetShortDateTime(DateTime dt)
    {
        return dt.Date.ToShortDateString() + " " + dt.TimeOfDay.ToString()[..8];
    }


    public static int GetLongDateTime(string strTime)
    {
        if (string.IsNullOrEmpty(strTime))
        {
            return 0;
        }

        int Hour = 0;
        int Minute = 0;
        string[] arr = strTime.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (arr.Length > 0)
        {
            Hour = NumberUtill.ToInt(arr[0]);
        }

        if (arr.Length > 1)
        {
            Minute = NumberUtill.ToInt(arr[1]);
        }

        return (Hour * 60) + Minute;

    }
    /// <summary>
    /// return (Hour + Minute + Second + Millisecond) days not support
    /// </summary>
    /// <param name="ServerTime"></param>
    /// <returns></returns>
    public static int GetMiliSeconds(DateTime ServerTime)
    {
        int outVal = ServerTime.Millisecond;
        outVal += ServerTime.Second * 1000;
        outVal += ServerTime.Minute * 60 * 1000;
        outVal += ServerTime.Hour * 60 * 60 * 1000;
        return outVal;
    }


}
