using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

public class ConditionalCatchRuntime(ProcessVersionRuntime processVersion, CatchEvent catchEvent,
    ConditionalEventDefinition conditionDefinition) : CatchRuntime(processVersion, catchEvent)
{
    public ConditionalEventDefinition conditionDefinition = conditionDefinition;
}