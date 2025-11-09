using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.CallActivity;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeSubProcess;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    private FlowNodeRunTime AddActivity(ProcessVersionRuntime processVersion, Activity activity)
    {
        FlowNodeRunTime result = null;
        SubProcessRuntime subProcessRuntime = null;
        switch (activity)
        {
            case BusinessRuleTask businessRuleTask:
                result = new BusinessRuleTaskRuntime(processVersion, businessRuleTask);
                break;
            case ManualTask manualTask:
                result = new ManualTaskRuntime(processVersion, manualTask);
                break;
            case ReceiveTask receiveTask:
                var receiveTaskRuntime = new ReceiveTaskRuntime(processVersion, receiveTask);
                result = receiveTaskRuntime;
                if (!AddToCatchMessage(receiveTaskRuntime.MessageDefinition, receiveTaskRuntime))
                {
                    return null;
                }

                break;
            case ScriptTask scriptTask:
                result = new ScriptTaskRuntime(processVersion, scriptTask);
                break;
            case SendTask sendTask:
                result = new SendTaskRuntime(processVersion, sendTask);
                break;
            case ServiceTask serviceTask:
                result = new ServiceTaskRuntime(processVersion, serviceTask);
                //if (!AddToCatchMessage(serviceTask.operationRef?.inMessageRef, result))
                //	return null;
                break;
            case UserTask userTask:
                UserTaskRuntime rt = new(processVersion, userTask);
                if (!processVersion.UserTasks.ContainsKey(activity.Id))
                {
                    processVersion.UserTasks.Add(activity.Id, rt);
                }
                else
                {
                    Logger.LogError($"Dupplicate Task {activity.Id} in process {processVersion.definition.Id}");
                }

                UiEntity entity = ProjectDefinition.Project.GetEntity(processVersion.definition.EntityNamespaceId,
                    processVersion.definition.EntityId) as UiEntity;
                if (entity?.getForm(rt.UserTask.FormId) is not Form entityForm)
                {
                    Logger.LogError($"Can not find form {rt.UserTask.FormId} in UserTask {rt.UserTask.Id} in process {processVersion.definition.Id}");
                }

                if ((userTask.ActivityAndLaneResources?.Count ?? 0) == 0)
                {
                    Logger.LogError(
                        $"Can not find default performer for UserTask {userTask.Id} in process {processVersion.definition.Id}");
                }

                result = rt;
                break;
            case Domain.Models.Bpmn.Processes.Activities.Tasks.Task task:
                result = new TaskRuntime(processVersion, task);
                break;
            case TransactionSubProcess transactionSubProcess:
                subProcessRuntime = new TransactionSubProcessRuntime(processVersion, transactionSubProcess);
                break;
            case AdHocSubProcess adHocSubProcess:
                subProcessRuntime = new AdHocSubProcessRuntime(processVersion, adHocSubProcess);
                break;
            case SubProcess subProcess:
                subProcessRuntime = subProcess.triggeredByEvent
                    ? new EventSubProcessRuntime(processVersion, subProcess)
                    : (SubProcessRuntime)new EmbededSubProcessRuntime(processVersion, subProcess);
                break;
            case CallActivity callActivity:
                switch (callActivity.ActivityType)
                {
                    case Activity.eActivityType.CallActivitySubProcess:
                        {
                            if (!ProjectDefinition.Project.ProcessExists(callActivity.CalledElementId ?? ""))
                            {
                                Logger.LogError(
                                    $"Can not find process {callActivity.CalledElementId} in CallActivity {callActivity.Id} in process {processVersion.definition.Id}");
                            }

                            result = new CallActivityRuntime(processVersion, callActivity);
                            break;
                        }

                    case Activity.eActivityType.CallActivityGlobalTask:
                        result = new CallActivityRuntime(processVersion, callActivity);
                        break;
                }

                break;
        }

        if (subProcessRuntime != null)
        {
            foreach (var fel1 in subProcessRuntime.SubProcessDefinition.flowElements)
            {
                AddFlowElement(subProcessRuntime.ProcessVersion, fel1.Value);
            }

            result = subProcessRuntime;
        }

        return result;
    }
}
