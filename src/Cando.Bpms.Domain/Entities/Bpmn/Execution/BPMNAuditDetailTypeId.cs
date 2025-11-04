using Neo.Bpms.Domain.Entities.Base.Audit;

namespace Neo.Bpms.Domain.Entities.Bpmn.Execution;
public enum BPMNAuditDetailTypeId
{
    Info = DetailTypeId.Info,
    Trace = DetailTypeId.Trace,
    Debug = DetailTypeId.Debug,
    Error = DetailTypeId.Error,

    RunOperation = 2000,

    EventReceived = 1000,
    CloseEventWaitingInstance = 1004,
    CreateInstance = 1005,
    CreateWorkItem = 1006,
    CompleteWorkItem = 1007,
    CreateActivityInstance = 1008,
    LockProcessInstance = 1009,
    UnlockProcessInstance = 1010,
    CancelActivity = 1011,
    TerminateProcessAndActivityInstances = 1012,
    CompleteParent = 1013,
    TerminateProcess = 1014,
}