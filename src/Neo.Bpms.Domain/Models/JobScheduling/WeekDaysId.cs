namespace Neo.Bpms.Domain.Models.JobScheduling;

[Description("روزهای هفته")]
[Flags]
public enum WeekDaysId
{
    [Description("شنبه")]
    Saturday = 1,

    [Description("یکشنبه")]
    Sunday = 2,

    [Description("دوشنبه")]
    Monday = 4,

    [Description("سه شنبه")]
    Tuesday = 8,

    [Description("چهارشنبه")]
    Wednesday = 16,

    [Description("پنجشنبه")]
    Thursday = 32,

    [Description("جمعه")]
    Friday = 64,
}