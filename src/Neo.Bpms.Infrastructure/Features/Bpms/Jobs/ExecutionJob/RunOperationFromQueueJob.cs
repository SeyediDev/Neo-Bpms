using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class RunOperationFromQueueJob : ExecutionJob
{
    internal BPMNOperationRuntime BPMNOperationRuntime { get; set; }

    internal override void Execute()
    {
        BPMNOperationRuntime.AsyncRunTaskFromQueue(false);
    }
}
