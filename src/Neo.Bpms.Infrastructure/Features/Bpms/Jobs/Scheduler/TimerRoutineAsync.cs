using System.Timers;
using Timer = System.Timers.Timer;
namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.Scheduler;

public abstract class TimerRoutineAsync : TimerRoutine
{
    #region timer
    protected internal override Timer StartTimerThread(long intervalMinute, long intervalUnitInSeconds = 60)
    {
        _ttTimer = new Timer { Interval = intervalMinute * intervalUnitInSeconds * 1000 };
        _ttTimer.Elapsed += TTTimer_Elapsed;
        _ttTimer.Start();
        return _ttTimer;
    }

    private readonly object _criticalSection = new();
    private async void TTTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        lock (_criticalSection)
        {
            if (_timerIsRunning) return;
            _timerIsRunning = true;
        }
        try
        {
            var dt = DateTime.UtcNow;
            await DoTimerRoutineAsync(dt);
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

        lock (_criticalSection)
        {
            _timerIsRunning = false;
        }
    }

    #endregion
    public abstract Task DoTimerRoutineAsync(DateTime dt);
}
