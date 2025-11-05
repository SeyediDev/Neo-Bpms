namespace Neo.Bpms.Domain.Models.JobScheduling;
public interface IJobSchedule
{
    long Id { get; set; }
    string ConfigId { get; set; }

    string Name { get; set; }

    ScheduleTypeId ScheduleTypeId { get; set; }

    JobFrequencyId JobFrequencyId { get; set; }

    long MaxRepeatCount { get; set; }

    DailyFrequencyId DailyFrequencyId { get; set; }

    //for daily and duration frequency every
    TimeSpan? StartTime { get; set; }

    //for daily and duration frequency every
    TimeSpan? EndTime { get; set; }

    //for ScheduleTypeId.Once
    DateTime? Date { get; set; }

    //for ScheduleTypeId.Once and daily frequency once
    TimeSpan? Time { get; set; }

    //duration
    DateTime? StartDate { get; set; }

    //duration
    DateTime? EndDate { get; set; }

    WeekDaysId OnWeekDaysId { get; set; }

    long Every { get; set; }

    MonthlyFrequencyId MonthlyFrequencyId { get; set; }

    long DayOfMonth { get; set; }

    WeekFrequencyId OnWeekFrequencyId { get; set; }

    long HourlyEvery { get; set; }
    TimeSpan? Duration { get; set; }
}
