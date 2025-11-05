namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class CompleteParentActivityInstanceJob : ExecutionJob
{
    public ActivityInstance ParentAi { get; set; }
    public LocalParameters ProcessOutputData { get; set; }
    public string? Description { get; set; }

    internal override void Execute()
    {
        if (ParentAi.pi.Locked)
        {
            ParentAi = DataStorage.WaitForUnlockFlowNodeInstance(ParentAi) as ActivityInstance;
            if (ParentAi == null)
            {
                return;
            }
        }

        ParentAi.pi.Execution = new ExecutionInstance(ParentAi.AuditTrail, Description);
        DataStorage.LockProcessInstance(ParentAi.pi, "CompleteParentActivityInstance.Lock");
        ParentAi.ActivityRuntime.Complete(ParentAi, ProcessOutputData);
        ParentAi.pi.Execution.DoJobs();
        DataStorage.UnlockProcessInstance(ParentAi.pi, "CompleteParentActivityInstance.Unlock");
    }
}
