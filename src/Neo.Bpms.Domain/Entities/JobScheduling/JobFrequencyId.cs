namespace Neo.Bpms.Domain.Entities.JobScheduling;

[Description("دوره تکرار")]
public enum JobFrequencyId
{
    [Description("روزانه")]
    Daily = 1,

    [Description("هفتگی")]
    Weekly = 2,

    [Description("ماهانه")]
    Monthly = 3,

    [Description("مدت زمان")]
    Duration = 4
}
