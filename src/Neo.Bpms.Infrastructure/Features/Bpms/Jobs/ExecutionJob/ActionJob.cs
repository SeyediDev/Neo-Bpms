namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class ActionJob(Action action) : ExecutionJob
{
    public Action Action = action;

    internal override void Execute()
    {
        Action();
    }
}
