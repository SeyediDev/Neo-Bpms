using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public abstract partial class ActivityRuntime
{
    private bool IsCompletedLoop(ActivityInstance ai, LocalParameters inputData)
    {
        switch (Activity?.loopCharacteristics?.loopType)
        {
            case LoopCharacteristics.eLoopType.Standard:
                return IsCompletedStandardLoop(ai, inputData);
            case LoopCharacteristics.eLoopType.MultiInstance:
                return IsCompletedMultiInstanceLoop(ai, inputData);
            default:
                return true;
        }
    }

    private void CreateAndStartLoopInstance(ProcessInstance pi, long loopCounter, LocalParameters inputData)
    {
        var loopInstance = CreateInstanceOfLoop(pi, loopCounter, inputData);
        StartInstanceOfLoop(loopInstance);
    }

    private LoopInstance CreateInstanceOfLoop(ProcessInstance pi, long loopCounter, LocalParameters inputData)
    {
        var loopAi = CreateActivityInstance(pi, loopCounter);
        return new LoopInstance
        {
            Ai = loopAi,
            InputData = FetchInputData(pi, loopAi, inputData)
        };
    }

    private static void StartInstanceOfLoop(LoopInstance loopInstance)
    {
        loopInstance.Ai.pi.Execution.AddJob(new StartInstanceJob
        {
            ActivityInstance = loopInstance.Ai,
            InputData = loopInstance.InputData
        });
    }
}
