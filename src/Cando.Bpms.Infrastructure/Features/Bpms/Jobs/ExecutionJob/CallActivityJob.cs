using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class CallActivityJob : ExecutionJob
{
    public CallActivityRuntime CallActivityRuntime { get; set; }
    public LocalParameters InputData { get; set; }
    public ActivityInstance ParentAi { get; set; }
    public ProcessVersionRuntime SubProcessVersion { get; set; }
    public FlowNodeRunTime SubProcessStarter { get; set; }

    internal override void Execute()
    {
        CallActivityRuntime.CallActivityJob(ParentAi, InputData, SubProcessVersion, SubProcessStarter);
    }
}
