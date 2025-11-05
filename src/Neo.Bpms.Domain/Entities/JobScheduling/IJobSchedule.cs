namespace Neo.Bpms.Domain.Entities.JobScheduling;
public class JobSchedule : IJobSchedule
{
    public long Id { get; set; }
    public string ConfigId { get; set; }
    public string Name { get; set; }
    public ScheduleTypeId ScheduleTypeId { get; set; }
    public JobFrequencyId JobFrequencyId { get; set; }
    public long MaxRepeatCount { get; set; }
    public DailyFrequencyId DailyFrequencyId { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? Time { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public WeekDaysId OnWeekDaysId { get; set; }
    public long Every { get; set; }
    public MonthlyFrequencyId MonthlyFrequencyId { get; set; }
    public long DayOfMonth { get; set; }
    public WeekFrequencyId OnWeekFrequencyId { get; set; }
    public long HourlyEvery { get; set; }
    public TimeSpan? Duration { get; set; }
}
