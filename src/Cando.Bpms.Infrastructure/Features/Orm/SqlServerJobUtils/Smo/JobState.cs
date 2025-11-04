namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

public enum JobState
{
    NotFound = 0,
    Executing = 1,
    WaitingForWorkerThread = 2,
    BetweenRetries = 3,
    Idle = 4,
    Suspended = 5,
    WaitingForStepToFinish = 6,
    PerformingCompletionAction = 7
}
