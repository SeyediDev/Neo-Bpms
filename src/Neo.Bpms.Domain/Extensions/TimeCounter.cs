namespace Neo.Bpms.Domain.Extensions;

/// <summary>
/// a cronometer utility
/// </summary>
public class TimeCounter
{
    private bool bStarted;
    private DateTime TimeStart;
    private DateTime TimeStop;
    public TimeSpan TotTime;
    public string FuncName;
    public string Duration
    {
        get
        {
            return ConvertToTime(TotTime);
        }
    }
    /// <summary>
    /// Starts this instance.
    /// </summary>
    /// <returns></returns>
    private bool Start()
    {
        if (bStarted)
            return true;
        bStarted = true;
        TimeStart = DateTime.UtcNow;
        return true;
    }
    /// <summary>
    /// Starts the specified fun name.
    /// </summary>
    /// <param name="funName">Name of the fun.</param>
    /// <returns></returns>
    public bool Start(string funName)
    {
        FuncName = funName;
        Start();
        return true;
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    /// <returns></returns>
    public bool Stop()
    {
        if (!bStarted)
            return true;
        bStarted = false;
        TimeStop = DateTime.UtcNow;
        TotTime += TimeStop - TimeStart;
        FuncName = "";
        return true;
    }
    /// <summary>
    /// Converts to time string.
    /// </summary>
    /// <param name="MyTotTime">My tot time.</param>
    /// <returns></returns>
    public string ConvertToTime(TimeSpan MyTotTime)
    {
        return (MyTotTime.Days * 24 + MyTotTime.Hours).ToString() +
                   ":" + MyTotTime.Minutes + ":" + MyTotTime.Seconds + "." +
                   (TotTime.Milliseconds * 100).ToString()[..1];
    }
}
