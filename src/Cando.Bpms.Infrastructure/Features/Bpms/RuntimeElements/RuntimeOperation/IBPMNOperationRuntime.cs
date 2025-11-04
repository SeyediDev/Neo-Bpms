namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

public interface IBPMNOperationRuntime
{
    BPMNOperationRuntime BPMNOperationRuntime { get; set; }

    void CheckDataInputAvailabilityAndStartIfNeeded(ActivityInstance ai,
        bool fetchInputData, LocalParameters inputData);

    void Free(FlowNodeInstance ai);
}