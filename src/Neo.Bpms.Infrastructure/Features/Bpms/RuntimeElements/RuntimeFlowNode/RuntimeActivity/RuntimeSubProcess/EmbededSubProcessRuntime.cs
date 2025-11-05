using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeSubProcess;

public class EmbededSubProcessRuntime(ProcessVersionRuntime processVersion, SubProcess subProcess)
    : SubProcessRuntime(processVersion, subProcess)
{
}
