using Neo.Bpms.Domain.Entities.ProcessData;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    public static void SaveFlowNodeInstance(FlowNodeInstance flowNodeInstance, string description)
    {
        ActivityInstanceRecordDb air = null;
        switch (flowNodeInstance)
        {
            case GatewaySyncWaitingInstance gatewaySyncWaitingInstance:
                air = GetGatewayWaitingInstanceInstance(gatewaySyncWaitingInstance);
                break;
            case EventWaitingInstance eventWaitingInstance:
                air = GetEventWaitingInstanceInstance(eventWaitingInstance);
                break;
            case UserTaskInstance userTaskInstance:
                air = GetUserTaskInstanceRecord(userTaskInstance, description);
                break;
            case ActivityInstance activityInstance:
                air = GetActivityInstanceRecord(activityInstance, description);
                break;
        }

        if (air == null)
        {
            flowNodeInstance.LogError($"Invalid {flowNodeInstance.FlowNode.GetType().Name} air.");
            return;
        }

        ApplyUtility au = ApplyUtility<ActivityInstanceRecord>.NewInTrail(flowNodeInstance.AuditTrail);
        bool b = air.Id <= 0 ? au.Insert(air) : au.Update(air);
        flowNodeInstance.SetId(air.Id);
        if (!b)
        {
            flowNodeInstance.LogError(
                $"can not save {flowNodeInstance.FlowNode.GetType().Name} instance. process:{flowNodeInstance.pi.Process.Id}, command : {au.CommandTxt}");
        }
    }

    private static ActivityInstanceRecordDb GetUserTaskInstanceRecord(UserTaskInstance userTaskInstance,
        string description)
    {
        ActivityInstanceRecordDb air = GetActivityInstanceRecord(userTaskInstance, description);
        air.ActualOwnerId = userTaskInstance.ActualOwnerId; //todo
        air.UserGroupId = userTaskInstance.UserGroupId;
        air.userTaskStateId = userTaskInstance.userTaskState; //todo
        air.WorkAllocationFactor = userTaskInstance.WorkAllocationFactor;
        air.ConfirmationStepId = userTaskInstance.ConfirmationStepId;
        return air;
    }

    private static ActivityInstanceRecordDb GetActivityInstanceRecord(ActivityInstance ai, string description)
    {
        ActivityInstanceRecord air = new()
        {
            Id = ai.Id,
            BPMNFlowNodeId = ai.FlowNodeRunTime.DbId,
            ProcessInstanceId = ai.pi.Id,
            MachineId = ai.MachineId,
            ActualOwnerId = ai.ActualOwnerId,
            UserGroupId = ai.UserGroupId,
            StateId = getActivityState(ai.state),
            userTaskStateId = ai.userTaskState,
            Closed = ai.Closed,
            CloseTime = ai.CloseTime,
            AllowedExecutionTime = ai.AllowedExecutionTime,
            CompletionTime = ai.CompletionTime,
            LoopCounter = ai.LoopCounter,
            ReceivedTokenCount = ai.ReceivedTokenCount,
            StartTime = ai.StartTime,
            AllowedActiveTime = ai.AllowedActiveTime,
            CreationTime = ai.CreationTime,
            MainProcessId = ai.pi.ProcessVersion.ProcessDbId,
            Description = description ?? ai.Description ?? ai.pi.Execution.Description,
        };
        for (ActivityInstance pai = ai.pi.ParentAi; pai != null; pai = pai.pi.ParentAi)
        {
            air.MainProcessId = pai.pi.ProcessVersion.ProcessDbId;
        }

        if (!string.IsNullOrEmpty(air.Description) && air.Description.Length >= 1024)
        {
            air.Description = air.Description[..1023];
        }

        if (ai.activity is Domain.Models.Bpmn.Processes.Activities.Tasks.Task task)
        {
            air.Priority = task.priorityLevel;
            if (!string.IsNullOrEmpty(task.priorityLevelProperty))
            {
                air.Priority = ai.Data.GetDouble(task.priorityLevelProperty);
            }
        }

        //todo save resource id (or other properties) in activity instance record (and its db object)
        //air.StateId = getScopeState(ai.state);
        return air;
    }

    private static ActivityInstanceRecordDb GetEventWaitingInstanceInstance(EventWaitingInstance ei)
    {
        ActivityInstanceRecord air = new()
        {
            BPMNFlowNodeId = ei.FlowNodeRunTime.DbId,
            ProcessInstanceId = ei.pi.Id,
            AllowedActiveTime = ei.AllowedActiveTime,
            Closed = ei.Closed,
            CloseTime = ei.CloseTime,
            CreationTime = ei.CreationTime,
            Id = ei.Id,
            StateId = getScopeState(ei.state)
        };

        return air;
    }

    private static ActivityInstanceRecordDb GetGatewayWaitingInstanceInstance(GatewaySyncWaitingInstance gwi)
    {
        ActivityInstanceRecord air = new()
        {
            BPMNFlowNodeId = gwi.FlowNodeRunTime.DbId,
            ProcessInstanceId = gwi.pi.Id,
            ReceivedTokens = Serialize(gwi.ReceivedTokens),
            waitingForStart = gwi.waitingForStart,
            activationCount = gwi.activationCount,
            AllowedActiveTime = gwi.AllowedActiveTime,
            Closed = gwi.Closed,
            CloseTime = gwi.CloseTime,
            CreationTime = gwi.CreationTime,
            Id = gwi.Id,
            StateId = getScopeState(gwi.state)
        };
        return air;
    }
}
