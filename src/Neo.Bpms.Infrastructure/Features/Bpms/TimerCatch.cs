using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.Scheduler;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public class TimerCatch(TimerCatchRuntime timerCatchRuntime) : TimerRoutine
{
    public override bool doTimerRoutine(DateTime dt)
    {
        CatchingTimer.TimerReceived(timerCatchRuntime);
        return true;
    }
}