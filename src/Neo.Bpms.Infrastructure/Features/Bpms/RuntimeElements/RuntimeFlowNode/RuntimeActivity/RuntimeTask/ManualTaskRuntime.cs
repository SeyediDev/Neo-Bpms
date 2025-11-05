using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class ManualTaskRuntime(ProcessVersionRuntime processVersion, ManualTask task) : TaskRuntime(processVersion, task)
{
    public ManualTask taskDefinition => Activity as ManualTask;

    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        var outputData = Run(ai, inputData);
        Complete(ai, outputData);
        await Task.CompletedTask;
    }

    private LocalParameters Run(ActivityInstance ai, LocalParameters inputData)
    {
        return inputData;
    }
}
