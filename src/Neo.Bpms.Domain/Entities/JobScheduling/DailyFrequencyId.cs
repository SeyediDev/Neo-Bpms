namespace Neo.Bpms.Domain.Entities.JobScheduling;

[Description("نوع تکرار در روز")]
public enum DailyFrequencyId
{
    [Description("در ساعت مشخص")]
    OnceAtSpesificTime = 1,

    [Description("ساعتی")]
    Hourly = 2,
}
