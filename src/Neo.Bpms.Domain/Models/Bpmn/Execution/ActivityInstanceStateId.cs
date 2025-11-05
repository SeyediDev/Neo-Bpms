namespace Neo.Bpms.Domain.Models.Bpmn.Execution;

public enum ActivityInstanceStateId
{
    /// <summary>
    /// A Token Arrives
    /// </summary>
    [EnumDescription("آماده", "Ready")]
    Ready = 1,

    /// <summary>
    /// Data Input Set available after "Ready"
    /// </summary>
    [EnumDescription("فعال", "Active")]
    Active = 11,

    //// <summary>
    //// Activity's work completed after "Active"
    //// </summary>
    //Completing = 21,

    /// <summary>
    /// Completing Requirements done, Assignments Completed after "Completing"
    /// </summary>
    [EnumDescription("تکمیل شده", "Completed")]
    Completed = 22,

    /// <summary>
    /// An alternative path for incoming event gateway selected after "Ready" or "Active"
    /// </summary>
    Withdrawn = 31,

    /// <summary>
    /// Non-Error interrupting event after "Ready" or "Active" or "Completing"
    /// </summary>
    Terminating = 32,

    /// <summary>
    /// Terminating Requirements done after "Terminating" or Compensation interrupted after "Compensating"
    /// </summary>
    [EnumDescription("خاتمه یافته", "Terminating")]
    Terminated = 33,

    /// <summary>
    /// Error interrupting event after "Ready" or "Active" or "Completing"
    /// </summary>
    Failing = 41,

    /// <summary>
    /// Failing Requirements done after "Failing"  or Compensation Failed after "Compensating"
    /// </summary>
    [EnumDescription("ناموفق", "Failed")]
    Failed = 42,

    /// <summary>
    /// compensation occurs after "Completed"
    /// </summary>
    Compensating = 51,

    /// <summary>
    /// Compensation completes after "Compensating"
    /// </summary>
    Compensated = 52,

    /// <summary>
    /// Manual Disable1
    /// </summary>
    [EnumDescription("غیر فعال شده بصورت دستی", "ManualDisable")]
    ManualDisable1 = 101,
    /// <summary>
    /// Manual Disable2
    /// </summary>
    ManualDisable2 = 102,
    /// <summary>
    /// Manual Disable3
    /// </summary>
    ManualDisable3 = 103,
}
