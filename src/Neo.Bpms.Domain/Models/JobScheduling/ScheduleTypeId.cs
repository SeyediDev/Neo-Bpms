namespace Neo.Bpms.Domain.Models.JobScheduling;

public enum ScheduleTypeId
{
    [Description("یک بار")]
    Once = 1,

    [Description("تکرار شونده")]
    Recurring = 2,

    [Description("مدت زمان")]
    Duration = 3
}