namespace Neo.Bpms.Domain.Models.JobScheduling;

[Description("روز هفته")]
public enum WeekFrequencyId
{
    [Description("شنبه")]
    Saturday = 1,

    [Description("یکشنبه")]
    Sunday = 2,

    [Description("دوشنبه")]
    Monday = 3,

    [Description("سه شنبه")]
    Tuesday = 4,

    [Description("چهارشنبه")]
    Wednesday = 5,

    [Description("پنجشنبه")]
    Thursday = 6,

    [Description("جمعه")]
    Friday = 7,

    [Description("روز کاری هفته")]
    Weekday = 8,

    [Description("تعطیلی آخر هفته")]
    Weekend = 9
}