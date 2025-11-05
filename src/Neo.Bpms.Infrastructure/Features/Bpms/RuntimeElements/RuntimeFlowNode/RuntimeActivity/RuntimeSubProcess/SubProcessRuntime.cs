using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeSubProcess;
public abstract class SubProcessRuntime(ProcessVersionRuntime processVersion, SubProcess subProcess) 
    : ActivityRuntime(processVersion, subProcess)
{
    public SubProcess SubProcessDefinition => flowNode as SubProcess;

    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        var startFlow = SubProcessDefinition.flowElements.Values
            .OfType<FlowNode>()
            .FirstOrDefault(f => !f.HasIncoming);
        var caught = false;
        if (startFlow != null && ProcessVersion.TryGetFlowNodeRuntime(startFlow.Id, out var eventRunTime))
        {
            switch (eventRunTime)
            {
                case CatchRuntime catchRunTime:
                    if (startFlow is StartEvent)
                    {
                        catchRunTime.Catch(ai.pi, ai, inputData);
                        caught = true;
                    }

                    break;
                case ActivityRuntime activityRunTime:
                    activityRunTime.ReceiveToken(ai.pi, inputData, null);
                    caught = true;
                    break;
            }
        }

        if (!caught)
            Complete(ai, inputData);
        await Task.CompletedTask;
    }
}
