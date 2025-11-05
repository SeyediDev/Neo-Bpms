using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class TaskRuntime(ProcessVersionRuntime processVersion, Domain.Models.Bpmn.Processes.Activities.Tasks.Task task) 
    : ActivityRuntime(processVersion, task)
{
    internal async override Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        LocalParameters outputData = inputData;
        Complete(ai, outputData);
        await Task.CompletedTask;
    }
}
