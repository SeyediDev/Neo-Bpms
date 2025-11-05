using System.Xml;

namespace Neo.Bpms.Domain.Extensions;

public static class ConvUtill
{
    public static bool ToBoolean(object value)
    {
        try
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return false;
            return value.ToString() == "1" || value.ToString().ToLower() == "true"
                ? true
                : value.ToString() == "0" || value.ToString() == "2" || value.ToString().ToLower() == "false"
                ? false
                : value is int i ? i != 0 : Convert.ToBoolean(value);
        }
        catch
        {
            return false;
        }
    }

    public static TimeSpan TimeSpanOf(object oTimeSpan)
    {
        try
        {
            TimeSpan timeSpan;
            if (oTimeSpan is long || oTimeSpan is int || oTimeSpan is short || oTimeSpan is ulong ||
                oTimeSpan is uint ||
                oTimeSpan is char)
            {
                timeSpan = TimeSpan.FromTicks((long)oTimeSpan);
            }
            else
            {
                //todo: IExpressionValue
                //if(oTimeSpan is IExpressionValue)
                //	oTimeSpan=((IExpressionValue)oTimeSpan).InternalValue;
                //else
                {
                    if (!TimeSpan.TryParse(oTimeSpan.ToString(), out timeSpan))
                        timeSpan = XmlConvert.ToTimeSpan(oTimeSpan.ToString());
                }
            }

            return timeSpan;
        }
        catch
        {
            return oTimeSpan is string ? FetchTimeSpan(oTimeSpan.ToString()) : TimeSpan.MinValue;
        }
    }

    public static TimeSpan FetchTimeSpan(string timeSpan)
    {
        try
        {
            string[] v1 = timeSpan.Split('.');
            int milisecond = v1.Length > 1 ? Convert.ToInt32(v1[1]) : 0;
            string[] v2 = v1[0].Split(':');
            int days = 0, hours = 0, minutes = 0, seconds = 0;
            if (v2.Length >= 4)
            {
                days = Convert.ToInt32(v2[0]);
                hours = Convert.ToInt32(v2[1]);
                minutes = Convert.ToInt32(v2[2]);
                seconds = Convert.ToInt32(v2[3]);
            }
            else if (v2.Length == 3)
            {
                hours = Convert.ToInt32(v2[0]);
                minutes = Convert.ToInt32(v2[1]);
                seconds = Convert.ToInt32(v2[2]);
            }
            else if (v2.Length == 2)
            {
                minutes = Convert.ToInt32(v2[0]);
                seconds = Convert.ToInt32(v2[1]);
            }
            else if (v2.Length == 1)
                seconds = Convert.ToInt32(v2[0]);

            return new TimeSpan(days, hours, minutes, seconds, milisecond);
        }
        catch
        {
            return TimeSpan.MinValue;
        }
    }

    public static double ToDouble(object value)
    {
        return string.IsNullOrEmpty(value?.ToString().Trim()) ? 0 : Convert.ToDouble(value);
    }

    public static long ToInt64(object value)
    {
        return string.IsNullOrEmpty(value?.ToString().Trim()) ? 0 : Convert.ToInt64(value);
    }
    public static int ToInt32(object value)
    {
        return string.IsNullOrEmpty(value?.ToString().Trim()) ? 0 : Convert.ToInt32(value);
    }
}
