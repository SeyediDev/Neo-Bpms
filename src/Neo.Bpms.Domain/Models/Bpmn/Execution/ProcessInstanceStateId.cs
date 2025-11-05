namespace Neo.Bpms.Domain.Models.Bpmn.Execution;

public enum ProcessInstanceStateId
{
    [EnumDescription("فعال", "Activated")]
    Activated = 1,
    //InExecution = 2,
    [EnumDescription("تکمیل شده", "Completed")]
    Completed = 22,
    InCancel = 31,
    [EnumDescription("لغو شده", "Cancelled")]
    Cancelled = 33,
    InCompensation = 51,//todo این وضعیت هیچ وقت حاصل نمی شود
    Compensation = 52,//todo این وضعیت هیچ وقت حاصل نمی شود

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