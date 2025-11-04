
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.Scheduler;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
public class TimerCatchRuntime : CatchRuntime
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly TimerRoutine _timer;
    public TimerEventDefinition TimerEventDefinition { get; set; }
    public TimerCatchRuntime(ProcessVersionRuntime processVersion, CatchEvent catchEvent,
        TimerEventDefinition timerEventDefinition) :
        base(processVersion, catchEvent)
    {
        TimerEventDefinition = timerEventDefinition;
        _timer = CatchEvent.Location == CatchEventLocation.Start && !CatchEvent.InSubProcess
            ? (TimerRoutine)new StartEventTimerEngine(this)
            : new TimerCatch(this);
        _timer.Initialize(1, $"{processVersion.definition.Name}.{CatchEvent.Name}", timerEventDefinition.SecondsResolution);
    }
}