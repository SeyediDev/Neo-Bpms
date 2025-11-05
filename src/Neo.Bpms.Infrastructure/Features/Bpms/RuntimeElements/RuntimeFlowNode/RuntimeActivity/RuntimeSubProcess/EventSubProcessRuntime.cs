using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Task = System.Threading.Tasks.Task;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeSubProcess;

public class EventSubProcessRuntime(ProcessVersionRuntime processVersion, SubProcess subProcess) : SubProcessRuntime(processVersion, subProcess)
{
    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await Task.CompletedTask;
    }
}
