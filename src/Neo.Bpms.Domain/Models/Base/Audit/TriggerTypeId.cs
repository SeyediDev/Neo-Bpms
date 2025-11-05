namespace Neo.Bpms.Domain.Models.Base.Audit;

public enum TriggerTypeId
{
    CreateInstanceByUser = 1,
    CreateAndCompleteTaskByUser,
    CompleteTaskByUser,
    EditTaskByUser,
    CompleteTaskByMachine,
    SaveRecordAndCompleteTask,
    ChangeUser,
    SetTaskStartTime,
    ChangeStateByUser,
    ProcessParticipant,
    Condition,
    Timer,
    Revision,
    Unload,
    AddComment,
    RestoreTask,
    BeginBatchInApplyUtility,
    ApplyUtilityConstructor,
    EngineBusiness,
    Business,
    CreateForm,
    EditForm,
    DeleteForm,
    ProcessCreateForm,
    SendMessage,
    View,
    Test
}