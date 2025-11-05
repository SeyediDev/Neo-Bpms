namespace Neo.Bpms.Domain.Models.JobScheduling;

[Description("نوع انتخاب روز در ماه")]
public enum MonthlyFrequencyId
{
    [Description("روز ماه")]
    Day = 1,

    [Description("اولین")]
    TheFirst = 2,

    [Description("دومین")]
    TheSecond = 3,

    [Description("سومین")]
    TheThird = 4,

    [Description("چهارمین")]
    TheFourth = 5,

    [Description("آخرین")]
    TheLast = 6,
}