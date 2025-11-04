using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity;
public class CallActivityRuntime(ProcessVersionRuntime processVersion, CallActivity callActivity) : ActivityRuntime(processVersion, callActivity)
{
    public CallActivity CallActivity = callActivity;

    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await Task.Run(() =>
        {
            var pi = ai.pi;
            AuditTrace($"Start activity {CallActivity.Id}, input data:{inputData}", pi, ai);
            var bFound = false;
            switch (CallActivity.ActivityType)
            {
                case Domain.Entities.Bpmn.Processes.Activities.Activity.eActivityType.CallActivitySubProcess:
                    bFound = CallActivitySubProcess(ai, inputData, CallActivity.CalledElementId);
                    break;
                case Domain.Entities.Bpmn.Processes.Activities.Activity.eActivityType.CallActivityGlobalTask:
                    if (!ProcessVersion.repository.globalTasks.TryGetValue(CallActivity.CalledElementId, out var gt) ||
                        gt == null) break;
                    var versionRuntime = gt.versions.Values.FirstOrDefault();
                    versionRuntime?.StartActivity(ai, inputData);
                    break;
            }

            if (!bFound)
            {
                AuditTrace($"Can not find process {CallActivity.CalledElementId} in activity {CallActivity.Id}", pi, ai);
                Complete(ai, inputData);
            }
        });
    }

    protected bool CallActivitySubProcess(ActivityInstance ai,
        LocalParameters inputData, string subProcessIdVersionId)
    {
        if (!FetchProcessVersion(subProcessIdVersionId, out var subProcessVersion))
            return false;
        var subProcessStarter = subProcessVersion.GetStarts().FirstOrDefault()
                                ?? (FlowNodeRunTime)subProcessVersion.GetActivities()
                                    .FirstOrDefault(a => !a.HasIncoming);
        if (subProcessStarter != null)
        {
            DataStorage.UnlockProcessInstance(ai.pi, "CAR.StartSubProcesses.Unlock");
            ai.pi.Execution.AddJob(new CallActivityJob
            {
                CallActivityRuntime = this,
                ParentAi = ai,
                InputData = inputData,
                SubProcessVersion = subProcessVersion,
                SubProcessStarter = subProcessStarter,
            });
        }

        return subProcessStarter != null;
    }

    public void CallActivityJob(ActivityInstance parentAi, LocalParameters inputData,
        ProcessVersionRuntime subProcessVersion, FlowNodeRunTime subProcessStarter)
    {
        var entityPkv = subProcessVersion.FetchEntityPkv(inputData);
        if (!string.IsNullOrEmpty(parentAi.Description))
            parentAi.pi.Execution.Description = parentAi.Description;
        var subPi = subProcessVersion.CreateInstance(parentAi.pi.Execution, inputData,
            entityPkv, parentAi.Id, null);
        AuditTrace(
            $"Start sub process {subProcessVersion.definition.Id}, flowNode: {flowNode.Id}, input data:{inputData}, SubPI:{subPi.Id}",
            parentAi.pi, parentAi);
        subProcessStarter?.ReceiveToken(subPi, inputData);
        subPi.Execution.DoJobs();
        DataStorage.UnlockProcessInstance(subPi, "CAR.StartSubProcesses.Unlock");
    }

    private bool FetchProcessVersion(string subProcessIdVersionId, out ProcessVersionRuntime subProcessVersion)
    {
        BusinessProcessVersion.FetchVersionId(subProcessIdVersionId, out var subProcessId,
            out var subProcessVersionId);
        subProcessVersion = null;
        if (!ProcessVersion.repository.processesRunTimes.TryGetValue(subProcessId, out var subProcess) ||
            subProcess == null)
            return false;
        subProcessVersion = subProcess.GetVersion(subProcessVersionId);
        return subProcessVersion != null;
    }
}
