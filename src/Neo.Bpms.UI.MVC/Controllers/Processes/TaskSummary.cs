using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Model.BPMN.Processes;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ProcessController
{
    [HttpGet]
    public ActionResult TaskSummary(long aiId, long piId, string caller, int callerPage)
    {
        // perform the query.
        IdentityUser user = GetUser();
        ProcessInstanceRecordDb pi = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).FetchProcessInstanceRecord(piId);
        Process processDefinition = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessVersionRuntimeByDbId(pi.ProcessVersionId).definition;
        List<ActivityInstanceRecord> activityInstances = DataStorage.FetchActivityInstanceRecords(null,
                $"(ProcessInstanceId=={pi.Id})", 0,
                "CreationTime" /*todo*/, null, null);
        ActivityInstanceRecord ai = activityInstances.FirstOrDefault(a => a.Id == aiId);
        FlowNode task = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetFlowNodeRunTime(ai.BPMNFlowNodeId)?.flowNode;
        if (ai.ActualOwnerId != user.Id && !user.CheckProcessAccess(processDefinition.Id))
            throw new Exception("Access denied!");// todo specific exception for this.
        TaskSummaryViewModel taskSummary = new()
        {
            ActualOwnerId = ai.ActualOwnerId,
            UserGroupId = ai.UserGroupId,
            ActivityDescription = ai.Description,
            ActivityName = task?.Name ?? "Undefined Flow Element Id",
            EntityDisplayText = GetEntityDisplay(processDefinition, pi.EntityPKV),
            EntityId = pi.EntityPKV,
            CreationTime = ai.CreationTime,
            StartTime = ai.StartTime,
            CloseTime = ai.CompletionTime,
            StateDisplayText = GetStateDisplay(processDefinition, pi.EntityPKV),
            PiActivities = activityInstances
        };
        return View(taskSummary);
    }

    private static Process GetProcess(ProcessInstanceRecordDb pi)
    {
        Process processDefinition = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessVersionRuntimeByDbId(pi.ProcessVersionId).definition ?? throw new Exception($"{pi.ProcessVersionId} definition not found");
        return processDefinition;
    }

    private static IList<ActivityInstanceRecord> GetAisByPiId(string piId)
    {
        return QueryUtility<ActivityInstanceRecord>
             .SelectFields(nameof(ActivityInstanceRecord.Id),
                 nameof(ActivityInstanceRecord.ActualOwnerId),
                 nameof(ActivityInstanceRecord.UserGroupId),
                 nameof(ActivityInstanceRecord.CreationTime),
                 nameof(ActivityInstanceRecord.StartTime),
                 nameof(ActivityInstanceRecord.CloseTime),
                 nameof(ActivityInstanceRecord.userTaskStateId),
                 nameof(ActivityInstanceRecord.Description),
                 nameof(ActivityInstanceRecord.Closed))
             .Where($"{nameof(ActivityInstanceRecord.ProcessInstanceId)}= '{piId}'")
             .ToList<ActivityInstanceRecord>();
    }

    private string GetStateDisplay(Process processDefinition, string pkv)
    {
        _ = processDefinition.Entity?.GetField(processDefinition.StateProperty);
        return "!!!!!!!!!!!!!کامل نشده";
    }
    private string GetEntityDisplay(Process processDefinition, string pkv)
    {
        _ = processDefinition.Entity?.GetField(processDefinition.StateProperty);
        return "کامل نشده!!!!!!!!!!!!";
    }
}
