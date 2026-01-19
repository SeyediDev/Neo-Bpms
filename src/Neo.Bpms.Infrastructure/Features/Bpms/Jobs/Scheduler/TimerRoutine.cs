using System.Timers;
using Timer = System.Timers.Timer;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.Scheduler;

public class TimerRoutine
{
    #region Initialize
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    public string Name;

    public void Initialize(long intervalMinute, string name, long intervalUnitInSeconds = 60)
    {
        Name = name;
        _ttTimer = StartTimerThread(intervalMinute, intervalUnitInSeconds);
        AddNotification("Started", null, null);
    }
    #endregion Initialize

    #region timer
    protected internal Timer _ttTimer;
    protected internal volatile bool _timerIsRunning;

    protected internal virtual Timer StartTimerThread(long intervalMinute, long intervalUnitInSeconds = 60)
    {
        _ttTimer = new Timer { Interval = intervalMinute * intervalUnitInSeconds * 1000 };
        _ttTimer.Elapsed += TTTimer_Elapsed;
        _ttTimer.Start();
        return _ttTimer;
    }

    private readonly object _criticalSection = new();
    private readonly object _bodyCriticalSection = new();

    private void TTTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        lock (_criticalSection)
        {
            if (_timerIsRunning) return;
            _timerIsRunning = true;
        }
        lock (_bodyCriticalSection)
        {
            try
            {
                var dt = DateTime.UtcNow;
                doTimerRoutine(dt);
            }
            catch (Exception exception)
            {
                var values = new LocalParameters
                {
                    {"Exception", exception.ToString()}
                };
                AddNotification("Exception", "09127165496", values);
                Logger.LogError(exception, Name);
            }
        }
        lock (_criticalSection)
        {
            _timerIsRunning = false;
        }
    }

    #endregion

    #region Timer Routine
    public virtual bool doTimerRoutine(DateTime dt)
    {
        return true;
    }

    public virtual void OnStop()
    {
        AddNotification("Stopped", null, null);
    }

    protected void AddNotification(string code, string smsNumber, LocalParameters values)
    {
        values ??= [];
        var time = DateTime.UtcNow;
        values.Add("Name", Name);
        values.Add("MachineName", Environment.MachineName);
        values.Add("ProcessName", System.Diagnostics.Process.GetCurrentProcess()?.ProcessName);
        values.Add("Time", time);
        var subject = $"Timer {Name} {code} at " + time.ToString("F");
        var emailText =
            $"<div>{subject}</div>{string.Join("", values.Select(localParameter => $"<div>{localParameter.Key} : {localParameter.Value}</div>"))}";
        //TODO NotifierEngine.AddNotification(subject, emailText, subject,
        //	$"Timer.{code}", code, null, smsNumber, "JavadSayedi@yahoo.com", null, values);
        LogInfo(subject);
    }

    protected void LogError(string error)
    {
        lock (_bodyCriticalSection)
        {
            //				EventLog.WriteEntry(Name, "Error:" + error);
            Logger.LogError(Name + ":" + error);
        }
    }

    protected void LogInfo(string info)
    {
        lock (_bodyCriticalSection)
        {
            //				EventLog.WriteEntry(Name, "Info:" + info);
            Logger.LogInformation(Name + ":" + info);
        }
    }

    protected void LogTrace(string trace)
    {
        lock (_bodyCriticalSection)
        {
            //				EventLog.WriteEntry(Name, "Trace:" + trace);
            Logger.LogTrace(Name + ":" + trace);
        }
    }

    #endregion Timer Routine
}
