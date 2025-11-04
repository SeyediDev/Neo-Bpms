using Neo.Bpms.Domain.Repository.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ScheduledReport;

public class ConfiguredScheduledReport : JobSchedule, IConfiguredScheduledAction, IConfig
{
    public ConfiguredScheduledReport(long id, string name)
    {
        Id = id;
        Name = name;
        Values = [];
    }

    public ConfiguredScheduledReport()
    {
        Values = [];
    }

    public bool IsDisable { get; set; }
    public string ReportConfigId { get; set; }
    public string ActionName { get; set; }
    public ScheduledReportFileTypeId FileTypeId { get; set; }
    public ScheduledReportOutputTypeId OutputTypeId { get; set; }
    public string UserId { get; set; }
    public long UserGroupId { get; set; }
    public long? ContactFunctionTypeId { get; set; }
    public long MaxRecordCount { get; set; }
    public string DestinationPath { get; set; }
    public string DestinationUserName { get; set; }
    public string DestinationPassword { get; set; }
    public List<ConfiguredFilterValue> Values { get; set; }

    public void Copy(ConfiguredScheduledReport scheduledReport)
    {
        Name = scheduledReport.Name;

        ScheduleTypeId = scheduledReport.ScheduleTypeId;
        JobFrequencyId = scheduledReport.JobFrequencyId;
        MaxRepeatCount = scheduledReport.MaxRepeatCount;
        DailyFrequencyId = scheduledReport.DailyFrequencyId;
        StartTime = scheduledReport.StartTime;
        EndTime = scheduledReport.EndTime;
        Date = scheduledReport.Date;
        Time = scheduledReport.Time;
        StartDate = scheduledReport.StartDate;
        EndDate = scheduledReport.EndDate;
        OnWeekDaysId = scheduledReport.OnWeekDaysId;
        Every = scheduledReport.Every;
        MonthlyFrequencyId = scheduledReport.MonthlyFrequencyId;
        DayOfMonth = scheduledReport.DayOfMonth;
        OnWeekFrequencyId = scheduledReport.OnWeekFrequencyId;
        HourlyEvery = scheduledReport.HourlyEvery;
        Duration = scheduledReport.Duration;

        IsDisable = scheduledReport.IsDisable;
        ReportConfigId = scheduledReport.ReportConfigId;
        ActionName = scheduledReport.ActionName;
        FileTypeId = scheduledReport.FileTypeId;
        OutputTypeId = scheduledReport.OutputTypeId;
        UserId = scheduledReport.UserId;
        UserGroupId = scheduledReport.UserGroupId;
        MaxRecordCount = scheduledReport.MaxRecordCount;
        DestinationPath = scheduledReport.DestinationPath;
        DestinationUserName = scheduledReport.DestinationUserName;
        DestinationPassword = scheduledReport.DestinationPassword;
        CopyValues(scheduledReport);
    }

    public void CopyValues(ConfiguredScheduledReport scheduledReport)
    {
        if (scheduledReport.Values == null)
            return;
        List<ConfiguredFilterValue> oldValues = Values;
        foreach (ConfiguredFilterValue filterValue in oldValues.Where(old => scheduledReport.Values.All(cv => cv.FieldId != old.FieldId)))
            Values.Remove(filterValue);
        foreach (ConfiguredFilterValue filterValue in scheduledReport.Values)
        {
            ConfiguredFilterValue value = Values.FirstOrDefault(cv => cv.FieldId == filterValue.FieldId);
            if (value != null)
                value.Value = filterValue.Value;
            else
                Values.Add(new ConfiguredFilterValue
                {
                    FieldId = filterValue.FieldId,
                    Value = filterValue.Value
                });
        }
    }
}
