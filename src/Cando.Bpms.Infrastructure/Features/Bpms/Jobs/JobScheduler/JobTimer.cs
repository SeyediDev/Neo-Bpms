using Neo.Bpms.Domain.Entities.JobScheduling;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.Scheduler;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.JobScheduler;

public abstract partial class JobTimer : TimerRoutineAsync
{
    public override async Task<bool> DoTimerRoutineAsync(DateTime dt)
    {
        var now = DateTime.Now;
        var jobs = await FetchSchedulesList(now, default);
        foreach (var job in jobs)
        {
            try
            {
                if (ShouldBeTaken(job, now))
                {
                    var result = await CallTakeAsync(now, job);
                    if (result.isSuccessful)
                        CallRecordLog(job, result.logRecord);
                }
            }
            catch (Exception exception)
            {
                LogException(exception, job, "TimerRoutine");
            }
        }
        return true;
    }

    private async Task<(bool isSuccessful, ElasticObject logRecord)> CallTakeAsync(DateTime now, Job job)
    {
        try
        {
            return await Take(now, job);
        }
        catch (Exception exception)
        {
            LogException(exception, job, "Take");
        }
        return (false, null);
    }

    private void CallRecordLog(Job job, ElasticObject logRecord)
    {
        try
        {
            RecordLog(job, logRecord);
        }
        catch (Exception exception)
        {
            LogException(exception, job, "RecordLog");
        }
    }
    private void LogException(Exception exception, Job job, string location)
    {
        Logger.LogCritical(exception, $"{GetType().Name}: One exception catched on {location} in JobTimer. JobSchedule {job.JobSchedule.Id} - {job.JobSchedule.Name} {job.JobItem} at " + DateTime.Now.ToString("G"));
    }

    protected abstract void RecordLog(Job record, ElasticObject logRecord);
    protected abstract DateTime? FetchLatestLogDate(Job job, out long repeatCount);
    protected abstract Task<(bool, ElasticObject)> Take(DateTime dt, Job job);
    protected abstract Task<IEnumerable<Job>> FetchSchedulesList(DateTime currentDate, CancellationToken cancellationToken);

    protected bool ShouldBeTaken(Job job, DateTime now)
    {
        var jobSchedule = job.JobSchedule;
        DateTime? latest = FetchLatestLogDate(job, out long repeatCount);
        if (jobSchedule.MaxRepeatCount > 0 && repeatCount >= jobSchedule.MaxRepeatCount)
        {
            return false;
        }

        if (!HasBeginedAndNotExpiredYet(jobSchedule, now))
        {
            return false;
        }

        return (object)jobSchedule.ScheduleTypeId switch
        {
            ScheduleTypeId.Once => latest == null &&
                                         (jobSchedule.Date == null ||
                                          jobSchedule.Date.Value.Add(jobSchedule.Time ?? TimeSpan.Zero) <= now),
            ScheduleTypeId.Duration => CheckDuration(jobSchedule, now, latest),
            ScheduleTypeId.Recurring => ShouldBeTakenRecurring(jobSchedule, now, latest),
            _ => false,
        };
    }

    private static bool ShouldBeTakenRecurring(IJobSchedule jobSchedule, DateTime now, DateTime? latest)
    {
        if (latest != null)
        {
            DateTime timeToPass = DateTime.MaxValue;
            switch (jobSchedule.JobFrequencyId)
            {
                case JobFrequencyId.Duration:
                    timeToPass = latest.Value.AddTicks(jobSchedule.Every);
                    break;
                case JobFrequencyId.Daily:
                    timeToPass = latest.Value.AddDays(jobSchedule.Every - 1);
                    break;
                case JobFrequencyId.Weekly:
                    timeToPass = latest.Value.AddDays((jobSchedule.Every - 1) * 7 + 1);
                    break;
                case JobFrequencyId.Monthly:
                    timeToPass = latest.Value.AddMonths((int)jobSchedule.Every).AddDays(1 - timeToPass.Day);
                    break;
            }
            if (now <= timeToPass)
            {
                return false;
            }
        }

        switch (jobSchedule.JobFrequencyId)
        {
            case JobFrequencyId.Weekly:
                if (!CheckDayOfWeek(jobSchedule, now))
                {
                    return false;
                }

                break;
            case JobFrequencyId.Monthly:
                if (!CheckMonthlyFrequency(jobSchedule, latest, now))
                {
                    return false;
                }

                break;
        }

        switch (jobSchedule.JobFrequencyId)
        {
            case JobFrequencyId.Weekly:
            case JobFrequencyId.Monthly:
            case JobFrequencyId.Daily:
                if (!CheckDailyFrequency(jobSchedule, latest, now))
                {
                    return false;
                }

                break;
        }
        return true;
    }

    private static bool CheckDuration(IJobSchedule jobSchedule, DateTime now, DateTime? latest)
    {
        return jobSchedule.Duration == null || latest == null || (now - latest.Value).Ticks > jobSchedule.Duration.Value.Ticks;
    }

    private static bool CheckMonthlyFrequency(IJobSchedule jobSchedule, DateTime? latest, DateTime now)
    {
        if (latest?.Date == now.Date)
        {
            return false;
        }

        if (jobSchedule.MonthlyFrequencyId == MonthlyFrequencyId.Day)
        {
            if (now.Day != jobSchedule.DayOfMonth)
            {
                return false;
            }
        }
        else
        {
            if (!CheckWeekFrequency(jobSchedule, now))
            {
                return false;
            }
        }
        DateTime firstOfMonth = now.AddDays(1 - now.Day);
        int weekCounter = 1;
        for (DateTime dayOfMonth = firstOfMonth; dayOfMonth <= now; dayOfMonth = dayOfMonth.AddDays(1))
        {
            if (dayOfMonth.DayOfWeek == now.DayOfWeek)
            {
                weekCounter++;
            }
        }

        switch (jobSchedule.MonthlyFrequencyId)
        {
            case MonthlyFrequencyId.TheFirst:
                if (weekCounter != 1)
                {
                    return false;
                }

                break;
            case MonthlyFrequencyId.TheSecond:
                if (weekCounter != 2)
                {
                    return false;
                }

                break;
            case MonthlyFrequencyId.TheThird:
                if (weekCounter != 3)
                {
                    return false;
                }

                break;
            case MonthlyFrequencyId.TheFourth:
                if (weekCounter != 4)
                {
                    return false;
                }

                break;
            case MonthlyFrequencyId.TheLast:
                if (weekCounter is not 4 and not 5)
                {
                    return false;
                }

                break;
        }
        return true;
    }

    private static bool CheckWeekFrequency(IJobSchedule scheduledReport, DateTime now)
    {
        switch (scheduledReport.OnWeekFrequencyId)
        {
            case WeekFrequencyId.Saturday:
                if (now.DayOfWeek != DayOfWeek.Saturday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Sunday:
                if (now.DayOfWeek != DayOfWeek.Sunday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Monday:
                if (now.DayOfWeek != DayOfWeek.Monday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Tuesday:
                if (now.DayOfWeek != DayOfWeek.Tuesday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Wednesday:
                if (now.DayOfWeek != DayOfWeek.Wednesday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Thursday:
                if (now.DayOfWeek != DayOfWeek.Thursday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Friday:
                if (now.DayOfWeek != DayOfWeek.Friday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Weekday:
                if (now.DayOfWeek is DayOfWeek.Thursday or
                    DayOfWeek.Friday)
                {
                    return false;
                }

                break;
            case WeekFrequencyId.Weekend:
                if (now.DayOfWeek is not DayOfWeek.Thursday and
                    not DayOfWeek.Friday)
                {
                    return false;
                }

                break;
        }
        return true;
    }

    private static bool CheckDayOfWeek(IJobSchedule scheduledReport, DateTime now)
    {
        switch (now.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Saturday) == 0)
                {
                    return false;
                }

                break;
            case DayOfWeek.Sunday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Sunday) == 0)
                {
                    return false;
                }

                break;
            case DayOfWeek.Monday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Monday) == 0)
                {
                    return false;
                }

                break;
            case DayOfWeek.Tuesday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Tuesday) == 0)
                {
                    return false;
                }

                break;
            case DayOfWeek.Wednesday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Wednesday) == 0)
                {
                    return false;
                }

                break;
            case DayOfWeek.Thursday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Thursday) == 0)
                {
                    return false;
                }

                break;
            case DayOfWeek.Friday:
                if (((long)scheduledReport.OnWeekDaysId & (long)WeekDaysId.Friday) == 0)
                {
                    return false;
                }

                break;
        }
        return true;
    }

    private static bool CheckDailyFrequency(IJobSchedule jobSchedule, DateTime? latest, DateTime now)
    {
        switch (jobSchedule.DailyFrequencyId)
        {
            case DailyFrequencyId.OnceAtSpesificTime:
                if (now.TimeOfDay < jobSchedule.Time)
                {
                    return false;
                }

                if (latest?.Date == now.Date)
                {
                    return false;
                }

                break;
            case DailyFrequencyId.Hourly:
                if (now.TimeOfDay < jobSchedule.StartTime || now.TimeOfDay > jobSchedule.EndTime)
                {
                    return false;
                }

                if (jobSchedule.HourlyEvery <= 0)
                {
                    jobSchedule.HourlyEvery = 1;
                }

                if (latest != null &&
                     (now - latest.Value).Hours < jobSchedule.HourlyEvery)
                {
                    return false;
                }

                break;
        }
        return true;
    }

    public static bool HasBeginedAndNotExpiredYet(IJobSchedule jobSchedule, DateTime now)
    {
        return (jobSchedule.StartDate == null || jobSchedule.StartDate <= now) && (jobSchedule.EndDate == null || jobSchedule.EndDate >= now);
    }
}
