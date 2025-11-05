using Neo.Bpms.Domain.Models.Bpmn.Iso8601;
using Neo.Bpms.Domain.Models.JobScheduling;

namespace Neo.Bpms.Domain.Model.Iso8601;

internal sealed partial class IntervalVisitor : Visitor
{
    private readonly string[] _items;

    private IntervalVisitor(string interval)
    {
        _items = interval?.Split('/');
        IsValid = true;
    }

    private void Visit()
    {
        if (_items == null || _items.Length == 0)
        {
            IsValid = false;
            return;
        }
        jobSchedule = new JobSchedule { ScheduleTypeId = ScheduleTypeId.Once };

        foreach (string item in _items)
        {
            if (!IsValid)
            {
                return;
            }

            if (string.IsNullOrEmpty(item))
            {
                IsValid = false;
                return;
            }
            char itemStarter = item.ToUpper()[0];
            switch (itemStarter)
            {
                case 'P':
                    jobSchedule.ScheduleTypeId = ScheduleTypeId.Recurring;
                    Duration = Iso8601Duration.Parse(item);
                    if (Duration != null)
                    {
                        jobSchedule.JobFrequencyId = JobFrequencyId.Duration;
                        jobSchedule.Every = Duration.Value.Ticks;
                    }
                    continue;
                case 'R':
                    jobSchedule.ScheduleTypeId = ScheduleTypeId.Recurring;
                    Repeat = Convert.ToInt64(item[1..]);
                    jobSchedule.MaxRepeatCount = Repeat.Value;
                    continue;
                case 'E':
                    jobSchedule.ScheduleTypeId = ScheduleTypeId.Recurring;
                    jobSchedule.JobFrequencyId = JobFrequencyId.Duration;
                    JobScheduleParse(item);
                    continue;
            }
            if (DateTime.TryParse(item, out DateTime dateTime))
            {
                if (Duration == null)
                {
                    StartDateTime = dateTime;
                    jobSchedule.StartDate = StartDateTime;
                }
                else
                {
                    EndDateTime = dateTime;
                    jobSchedule.EndDate = EndDateTime;
                }
            }
        }
    }

    private void JobScheduleParse(string neoConvention)
    {
        tokens = [.. neoConvention.Skip(1)];

        //"E1DT13H40M00S"
        //"E1DE13H40M00S|13H40M00S|13H40M00S"
        //"E2W2DW3DW" [DW is repeatable for each day of week]
        //"E3M25D"
        //"E3M2F2DW" [f:{1-5},DW:{1-9}] 
        for (int i = 0; i < tokens.Length; i++)
        {
            char token = tokens[i];
            if (!IsValid)
            {
                return;
            }

            double every = 0;
            switch (token)
            {
                case 'D':
                case 'W':
                case 'M':
                    IsValid = HandleDateDesignator(ref every);
                    jobSchedule.Every = (long)every;

                    switch (token)
                    {
                        case 'D':
                            jobSchedule.JobFrequencyId = JobFrequencyId.Daily;
                            i += 1;
                            ParseDailyFrequency(ref i);
                            break;
                        case 'W':
                            jobSchedule.JobFrequencyId = JobFrequencyId.Weekly;
                            i += 1;
                            ParseWeeklyFrequency(ref i);
                            break;
                        case 'M':
                            jobSchedule.JobFrequencyId = JobFrequencyId.Monthly;
                            i += 1;
                            ParseMonthlyFrequency(ref i);
                            break;
                    }
                    continue;
            }
            currentDigits.Add(token);
        }
        IsValid &= currentDigits.Count == 0;
    }

    private void ParseMonthlyFrequency(ref int i)
    {
        //"E3M25D"
        //"E3M2F2DW" [f:{1-5},DW:{1-9}] 
        bool seenF = false;
        bool complete = false;
        for (; i < tokens.Length; i++)
        {
            char token = tokens[i];
            if (!IsValid)
            {
                return;
            }

            if (complete)
            {
                break;
            }

            double every = 0;
            switch (token)
            {
                case 'D':
                    if (!seenF)
                    {
                        IsValid = HandleDateDesignator(ref every);
                        jobSchedule.DayOfMonth = (long)every;
                        jobSchedule.MonthlyFrequencyId = MonthlyFrequencyId.Day;
                        complete = true;
                    }
                    else
                    {
                        IsValid = tokens[i + 1] == 'W';
                    }

                    continue;
                case 'F':
                    IsValid = HandleDateDesignator(ref every);
                    jobSchedule.MonthlyFrequencyId = (MonthlyFrequencyId)((long)every + 1);
                    seenF = true;
                    continue;
                case 'W':
                    IsValid = HandleDateDesignator(ref every);
                    jobSchedule.OnWeekFrequencyId = (WeekFrequencyId)(long)(every + 1);
                    complete = true;
                    continue;
            }
            currentDigits.Add(token);
        }
        if (complete)
        {
            ParseDailyFrequency(ref i);
        }
    }

    private void ParseWeeklyFrequency(ref int i)
    {
        //"E2W2DW3DW" [DW is repeateable for each day of week]
        bool complete = false;
        jobSchedule.OnWeekDaysId = 0;
        for (; i < tokens.Length; i++)
        {
            char token = tokens[i];
            if (!IsValid)
            {
                return;
            }

            double every = 0;
            switch (token)
            {
                case 'D':
                    IsValid = tokens[i + 1] == 'W';
                    continue;
                case 'W':
                    IsValid = HandleDateDesignator(ref every);
                    switch ((WeekFrequencyId)(long)(every + 1))
                    {
                        case WeekFrequencyId.Saturday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Saturday;
                            break;
                        case WeekFrequencyId.Sunday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Sunday;
                            break;
                        case WeekFrequencyId.Monday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Monday;
                            break;
                        case WeekFrequencyId.Tuesday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Tuesday;
                            break;
                        case WeekFrequencyId.Wednesday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Wednesday;
                            break;
                        case WeekFrequencyId.Thursday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Thursday;
                            break;
                        case WeekFrequencyId.Friday:
                            jobSchedule.OnWeekDaysId |= WeekDaysId.Friday;
                            break;
                    }
                    complete = true;
                    continue;
            }
            if (complete && !char.IsDigit(token))
            {
                break;
            }

            currentDigits.Add(token);
        }
        if (complete)
        {
            ParseDailyFrequency(ref i);
        }
    }
    private void ParseDailyFrequency(ref int i)
    {
        //"E1D|13H40M00S"
        //"E1D13H|13H40M00S|13H40M00S"
        jobSchedule.DailyFrequencyId = DailyFrequencyId.OnceAtSpesificTime;
        bool seenStart = false;
        for (; i < tokens.Length; i++)
        {
            char token = tokens[i];
            if (!IsValid)
            {
                return;
            }

            double every = 0;
            switch (token)
            {
                case 'H':
                    jobSchedule.DailyFrequencyId = DailyFrequencyId.Hourly;
                    IsValid = HandleDateDesignator(ref every);
                    jobSchedule.HourlyEvery = (long)every;
                    continue;
                case '|':
                    string[] ramain = new string(tokens)[(i + 1)..].Split('|');
                    i += ramain[0].Length;
                    TimeSpan time = Iso8601Duration.Parse("PT" + ramain[0]);
                    if (jobSchedule.DailyFrequencyId == DailyFrequencyId.OnceAtSpesificTime)
                    {
                        jobSchedule.Time = time;
                    }
                    else
                    {
                        if (seenStart)
                        {
                            jobSchedule.EndTime = time;
                        }
                        else
                        {
                            jobSchedule.StartTime = time;
                            seenStart = true;
                        }
                    }
                    continue;
            }
            currentDigits.Add(token);
        }
    }
}
