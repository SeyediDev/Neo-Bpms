namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions : IFunctionImplementations
{
    /// <summary>
    /// دریافت تاریخ و زمان سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime ServerDateTime()
    {
        return DateTime.Now;
    }

    /// <summary>
    /// دریافت تاریخ و زمان سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime serverdatetime()
    {
        return DateTime.Now;
    }

    /// <summary>
    /// دریافت تاریخ سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime ServerDate()
    {
        return DateTime.Now.Date;
    }

    /// <summary>
    /// دریافت تاریخ سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime serverdate()
    {
        return DateTime.Now.Date;
    }

    /// <summary>
    /// دریافت زمان سرور
    /// </summary>
    /// <returns></returns>
    public static TimeSpan ServerTime()
    {
        return DateTime.Now.TimeOfDay;
    }

    /// <summary>
    /// دریافت زمان سرور
    /// </summary>
    /// <returns></returns>
    public static TimeSpan servertime()
    {
        return DateTime.Now.TimeOfDay;
    }
}
