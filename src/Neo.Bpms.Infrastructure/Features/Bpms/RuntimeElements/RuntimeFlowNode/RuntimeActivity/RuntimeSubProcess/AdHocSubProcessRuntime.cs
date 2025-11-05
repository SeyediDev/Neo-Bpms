using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Task = System.Threading.Tasks.Task;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeSubProcess;

public class AdHocSubProcessRuntime(ProcessVersionRuntime processVersion, AdHocSubProcess subProcess) 
    : SubProcessRuntime(processVersion, subProcess)
{
    public AdHocSubProcess adhocSubProcessDefinition = subProcess;
    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await Task.CompletedTask;
    }
}
