namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions : IFunctionImplementations
{
    /// <summary>
    /// دریافت تاریخ و زمان سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime ServerDateTime()
    {
        return DateTime.UtcNow;
    }

    /// <summary>
    /// دریافت تاریخ و زمان سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime serverdatetime()
    {
        return DateTime.UtcNow;
    }

    /// <summary>
    /// دریافت تاریخ سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime ServerDate()
    {
        return DateTime.UtcNow.Date;
    }

    /// <summary>
    /// دریافت تاریخ سرور
    /// </summary>
    /// <returns></returns>
    public static DateTime serverdate()
    {
        return DateTime.UtcNow.Date;
    }

    /// <summary>
    /// دریافت زمان سرور
    /// </summary>
    /// <returns></returns>
    public static TimeSpan ServerTime()
    {
        return DateTime.UtcNow.TimeOfDay;
    }

    /// <summary>
    /// دریافت زمان سرور
    /// </summary>
    /// <returns></returns>
    public static TimeSpan servertime()
    {
        return DateTime.UtcNow.TimeOfDay;
    }
}
