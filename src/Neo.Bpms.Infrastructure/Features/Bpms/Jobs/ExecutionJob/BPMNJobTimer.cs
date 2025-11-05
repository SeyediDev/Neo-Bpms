using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.Scheduler;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

public class BPMNJobTimer : TimerRoutine
{
    public override bool doTimerRoutine(DateTime dt)
    {
        return true;
    }
}
