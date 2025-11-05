using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Task = System.Threading.Tasks.Task;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class BusinessRuleTaskRuntime(ProcessVersionRuntime processVersion, BusinessRuleTask task) 
    : TaskRuntime(processVersion, task)
{
    public BusinessRuleTask taskDefinition => Activity as BusinessRuleTask;

    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        var outputData = Run(ai, inputData);
        Complete(ai, outputData);
        await Task.CompletedTask;
    }

    private LocalParameters Run(ActivityInstance ai, LocalParameters inputData)
    {
        //TODO: integration between process and rule engine.../service/url/
        return inputData;
    }
}
