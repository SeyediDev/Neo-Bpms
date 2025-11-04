using Task = System.Threading.Tasks.Task;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public partial class UserTaskRuntime(ProcessVersionRuntime processVersion, UserTask task) : TaskRuntime(processVersion, task)
{
    public UserTask UserTask => Activity as UserTask;
    public long? RenderingFormDbId { get; set; }
    public long? RenderingIndexFormDbId { get; set; }
    public string RenderingFormId => UserTask.FormId;
    public string RenderingIndexFormId => UserTask.IndexFormId;

    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await Task.CompletedTask;
    }

    internal UserTaskInstance CreateProcessInstanceAndCreateUserTask(ExecutionInstance execution,
        string entityPkv, LocalParameters inputData)
    {
        var startEvent = UserTask.incoming?.Where(seq =>
                seq.sourceRef is StartEvent s && s.TriggerType == Event.eEventType.None)
            .Select(seq => seq.sourceRef as StartEvent)
            .FirstOrDefault();
        var pi = ProcessVersion.CreateInstance(execution, inputData, entityPkv, null, startEvent?.dataOutputs);
        execution.AuditTrail.ProcessInstanceId = pi.Id;
        AuditTrace($"CreateProcessInstanceAndCreateUserTask(Process:{pi.Process.Id})", pi, null);
        return CreateUserTaskInstance(pi, 0);
    }

    internal bool CreateProcessInstanceAndCompleteUserTask(AuditTrail auditTrail,
        string entityPkv, string workDescription, TimeSpan executionDuration, LocalParameters localParameters,
        out long? wid)
    {
        var execution = CreateExecution(auditTrail, workDescription);
        var wi = CreateProcessInstanceAndCreateUserTask(execution, entityPkv, localParameters);
        wi.AddAuditDetail(BPMNAuditDetailTypeId.CompleteWorkItem, "CreateProcessInstanceAndCompleteUserTask");
        wi.StartTime = execution.AuditTrail.DateTime - executionDuration;
        wid = wi.Id;
        return CompleteUserTask(wi, localParameters);
    }

    private static ExecutionInstance CreateExecution(AuditTrail auditTrail, string workDescription)
    {
        return new ExecutionInstance(auditTrail, workDescription);
    }

    internal bool CompleteWorkItemOfUserTask(AuditTrail auditTrail, long workItemId,
        string workDescription, LocalParameters inputData)
    {
        var execution = CreateExecution(auditTrail, workDescription);
        var wi = FetchUserTaskInstance(workItemId, execution);
        return CompleteUserTask(wi, inputData);
    }

    internal bool SetWorkItemStartTime(long workItemId, Form form, IdentityUser user, long userGroupId)
    {
        var auditTrail = new AuditTrail(TriggerTypeId.SetTaskStartTime, user.Id, user, userGroupId)
        {
            MetaEntityId = form.entity.DbId,
            MetaFormId = form.DbId,
            FormId = form.Id,
            FlowNodeInstanceId = workItemId,
        };
        var execution = CreateExecution(auditTrail, null);
        var wi = FetchUserTaskInstance(workItemId, execution);
        if (wi.StartTime == null || wi.StartTime == DateTime.MinValue)
        {
            auditTrail.EntityPkv = wi.pi.EntityPkv;
            AuditTrace($"SetTaskStartTime(TaskId:{wi.activity.Id} EntityPkv:{wi.pi.EntityPkv})", wi.pi, wi);
            wi.StartTime = DateTime.Now;
            wi.Save();
            wi.pi.Execution.DoJobs();
            DataStorage.SaveAudit(auditTrail);
        }

        DataStorage.UnlockProcessInstance(wi.pi, "UTR.Unlock.0");
        return true;
    }

    internal bool ChangeStateByUser(AuditTrail auditTrail, long workItemId,
        string workDescription, UserTaskInstanceStateId newState)
    {
        var execution = CreateExecution(auditTrail, workDescription);
        var wi = FetchUserTaskInstance(workItemId, execution);
        AuditTrace($"ChangeStateByUser(TaskId:{workItemId})", wi.pi, wi);
        wi.SetUserTaskState(newState, execution.AuditTrail?.User?.Id, execution.AuditTrail?.UserGroupId ?? 0);
        if (newState == UserTaskInstanceStateId.Completed)
            CompleteUserTask(wi, null);
        else
        {
            wi.Save();
            wi.pi.Execution.DoJobs();
            DataStorage.UnlockProcessInstance(wi.pi, "UTR.UnLock.1");
        }

        return true;
    }

    internal bool ChangeUser(ExecutionInstance execution, long workItemId, UserTaskInstanceStateId newState,
        IdentityUser newUser, long newUserGroupId)
    {
        var wi = FetchUserTaskInstance(workItemId, execution);
        wi.SetUserTaskState(newState,
            newUser == null ? execution.AuditTrail?.User?.Id : newUser.Id,
            newUser == null ? execution.AuditTrail?.UserGroupId ?? 0 : newUserGroupId);
        AuditTrace($"ChangeUser(TaskId:{workItemId})", wi.pi, wi);
        wi.Save();
        wi.pi.Execution.DoJobs();
        DataStorage.UnlockProcessInstance(wi.pi, "UTR.Unlock.0");
        return true;
    }

    protected override ActivityInstance CreateActivityInstance(ProcessInstance pi, long loopCounter)
    {
        var wi = CreateUserTaskInstance(pi, loopCounter);
        DoWorkDistribution(wi);
        return wi;
    }

    private UserTaskInstance FetchUserTaskInstance(long workItemId, ExecutionInstance execution)
    {
        var air = DataStorage.FetchFlowInstanceRecord(workItemId);
        var pi = DataStorage.FetchPi(ProcessVersion, air.ProcessInstanceId, execution);
        pi = DataStorage.LockControl(pi, "FetchUserTaskInstance", out var changed);
        if (changed)
            air = DataStorage.FetchFlowInstanceRecord(workItemId);
        pi.AuditTrail.ProcessInstanceId = pi.Id;
        var wi = new UserTaskInstance(air, this, pi);
        DataStorage.LoadDataRecord(ProcessVersion, wi.pi.EntityPkv, wi.pi);
        return wi;
    }

    private UserTaskInstance CreateUserTaskInstance(ProcessInstance pi, long loopCounter)
    {
        var wi = new UserTaskInstance(0, this, pi);
        wi.Init(loopCounter);
        wi.WorkAllocationFactor = calcWorkAllocationFactor(); //todo
        wi.ActualOwnerId = wi.AuditTrail.User?.Id; //todo
        wi.UserGroupId = wi.AuditTrail.UserGroupId;
        wi.Save();
        pi.AuditTrail.FlowNodeInstanceId = wi.Id;
        wi.AddAuditDetail(BPMNAuditDetailTypeId.CreateWorkItem, "Create work item");
        return wi;
    }

    private bool CompleteUserTask(UserTaskInstance wi, LocalParameters inputData)
    {
        wi.Complete();
        wi.ActualOwnerId = wi.AuditTrail.User?.Id;
        wi.UserGroupId = wi.AuditTrail.UserGroupId;
        AuditTrace($"CompleteUserTask(TaskId:{wi.activity.Id} EntityPkv:{wi.pi.EntityPkv})",
            wi.pi, wi);
        wi.Save();
        Complete(wi, inputData);
        DataStorage.SaveProcessInstance(wi.pi, wi, "UTR.5", true, DataStorage.LockChangeRequest.Unlock);
        return true;
    }
}
