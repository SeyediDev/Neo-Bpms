namespace Neo.Bpms.Domain.Entities.Bpmn.Execution;

public enum UserTaskInstanceStateId
{
    None,

    [EnumDescription("ایجاد شده", "Created")]
    Created = 1,

    OfferedToASingleResource = 11,
    OfferedToMultipleResources,

    [EnumDescription("تخصیص داده شده به منبع کار", "AllocatedToASingleResource")]
    AllocatedToASingleResource = 21,

    [EnumDescription("شروع شده", "Started")]
    Started = 31,
    Suspended,

    [EnumDescription("تکمیل شده", "Completed")]
    Completed = 41,
    [EnumDescription("ناموفق", "Failed")]
    Failed,
    Withdrawn,


    [EnumDescription("غیر فعال شده بصورت دستی", "ManualDisable")]
    ManualDisable1 = 101,

    ManualDisable2 = 102,

    ManualDisable3 = 103,
}