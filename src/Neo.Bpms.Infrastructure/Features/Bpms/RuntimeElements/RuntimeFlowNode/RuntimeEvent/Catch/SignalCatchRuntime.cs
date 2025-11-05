using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

internal class SignalCatchRuntime : CatchRuntime
{
    internal SignalCatchRuntime(ProcessVersionRuntime processVersion, CatchEvent catchEvent,
        SignalEventDefinition signalDefinition) :
        base(processVersion, catchEvent)
    {
        _signal = signalDefinition;
    }

    private readonly SignalEventDefinition _signal;

    internal bool SignalReceived(ExecutionInstance execution, LocalParameters signalData)
    {
        var catchingEvent = new CatchingSignal(this, _signal, execution, null, null, signalData);
        return catchingEvent.EventReceived(CatchEvent.Location);
    }
}
