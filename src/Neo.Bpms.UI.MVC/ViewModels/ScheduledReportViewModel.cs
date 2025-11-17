using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Domain.Models.JobScheduling;

namespace Neo.Bpms.UI.MVC.ViewModels;

public class ScheduledReportViewModel
{
    public ScheduledReportViewModel()
    {
    }

    public ScheduledReportViewModel(ConfiguredScheduledReport csr)
    {
        Id = csr.Id;
        Name = csr.Name;
        IsDisable = csr.IsDisable;

        ScheduleTypeId = csr.ScheduleTypeId;
        JobFrequencyId = (long)csr.JobFrequencyId;
        MaxRepeatCount = csr.MaxRepeatCount;
        DailyFrequencyId = (long)csr.DailyFrequencyId;
        StartTime = csr.StartTime?.ToHtmlInputValue();
        EndTime = csr.EndTime?.ToHtmlInputValue();
        Date = csr.Date?.ToHtmlInputValue(ProjectDefinition.Project.DefaultCalendar);
        Time = csr.Time?.ToHtmlInputValue();
        StartDate = csr.StartDate?.ToHtmlInputValue(ProjectDefinition.Project.DefaultCalendar);
        EndDate = csr.EndDate?.ToHtmlInputValue(ProjectDefinition.Project.DefaultCalendar);
        OnWeekDaysId = (long)csr.OnWeekDaysId;
        Every = csr.Every;
        MonthlyFrequencyId = (long)csr.MonthlyFrequencyId;
        DayOfMonth = csr.DayOfMonth;
        OnWeekFrequencyId = (long)csr.OnWeekFrequencyId;
        HourlyEvery = csr.HourlyEvery;

        ActionName = csr.ActionName;
        FileTypeId = (long)csr.FileTypeId;
        OutputTypeId = (long)csr.OutputTypeId;
        UserGroupId = csr.UserGroupId;
        Username = csr.UserId;
        MaxRecordCount = csr.MaxRecordCount;
        DestinationUserName = csr.DestinationUserName;
        DestinationPassword = csr.DestinationPassword;
    }

    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsDisable { get; set; } //todo
    public ScheduleTypeId ScheduleTypeId { get; set; }
    public long JobFrequencyId { get; set; }
    public long MaxRepeatCount { get; set; }
    public long DailyFrequencyId { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long OnWeekDaysId { get; set; }
    public long Every { get; set; }
    public long MonthlyFrequencyId { get; set; }
    public long DayOfMonth { get; set; }
    public long OnWeekFrequencyId { get; set; }
    public long HourlyEvery { get; set; }

    public string ReportConfigId { get; set; }
    public string ActionName { get; set; }
    public long FileTypeId { get; set; }
    public long OutputTypeId { get; set; }
    public long UserGroupId { get; set; }
    public string Username { get; set; }
    public long MaxRecordCount { get; set; }
    public string DestinationPath { get; set; }
    public string DestinationUserName { get; set; }
    public string DestinationPassword { get; set; }
    public List<ConfiguredFilterValue> FilterValues { get; set; }

    public ConfiguredScheduledReport ToConfiguredScheduledReport(string calendarType)
    {
        ConfiguredScheduledReport result = new()
        {
            Id = Id,
            Name = Name,
            IsDisable = IsDisable,
            ScheduleTypeId = ScheduleTypeId,
            JobFrequencyId = (JobFrequencyId)JobFrequencyId,
            StartTime = StartTime?.ToTimeSpan(),
            EndTime = EndTime?.ToTimeSpan(),
            Date = Date?.ToDateTime(calendarType),
            Time = Time?.ToTimeSpan(),
            StartDate = StartDate?.ToDateTime(calendarType),
            EndDate = EndDate?.ToDateTime(calendarType),
            OnWeekDaysId = (WeekDaysId)OnWeekDaysId,
            Every = Every,
            MonthlyFrequencyId = (MonthlyFrequencyId)MonthlyFrequencyId,
            DayOfMonth = DayOfMonth,
            OnWeekFrequencyId = (WeekFrequencyId)OnWeekFrequencyId,
            DailyFrequencyId = (DailyFrequencyId)DailyFrequencyId,
            HourlyEvery = HourlyEvery,

            ReportConfigId = ReportConfigId,
            ActionName = ActionName,
            FileTypeId = (ScheduledReportFileTypeId)FileTypeId,
            OutputTypeId = (ScheduledReportOutputTypeId)OutputTypeId,
            UserGroupId = UserGroupId,
            UserId = Username,
            MaxRecordCount = MaxRecordCount,
            DestinationPath = DestinationPath,
            DestinationUserName = DestinationUserName,
            DestinationPassword = DestinationPassword,
        };
        if (FilterValues != null)
            result.Values.AddRange(FilterValues.Where(f => !string.IsNullOrEmpty(f.Value)));
        else
            result.Values = null;
        return result;
    }
}
