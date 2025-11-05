using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public class UserTaskInstance : ActivityInstance
{
    public UserTaskInstance(long instanceId, UserTaskRuntime userTask, ProcessInstance pi,
        ActivityInstanceStateId activityState = ActivityInstanceStateId.Ready,
        UserTaskInstanceStateId userTaskState = UserTaskInstanceStateId.Created)
        : base(instanceId, userTask, pi, activityState, userTaskState)
    {
    }

    public UserTaskInstance(ElasticObject userTaskInstanceRecord, UserTaskRuntime userTask, ProcessInstance pi)
        : base(userTaskInstanceRecord, userTask, pi)
    {
        ConfirmationStepId = userTaskInstanceRecord.GetLong(nameof(ActivityInstanceRecord.ConfirmationStepId));
        WorkAllocationFactor = userTaskInstanceRecord.GetLong(nameof(ActivityInstanceRecord.WorkAllocationFactor));
    }

    public UserTaskInstance(ActivityInstanceRecordDb userTaskInstanceRecord, UserTaskRuntime userTask,
        ProcessInstance pi)
        : base(userTaskInstanceRecord, userTask, pi)
    {
        ConfirmationStepId = userTaskInstanceRecord.ConfirmationStepId;
        WorkAllocationFactor = userTaskInstanceRecord.WorkAllocationFactor;
    }

    public UserTask UserTask => activity as UserTask;

    //public TimeSpan ExecutionTime => userTaskState < UserTaskInstanceStateId.Started ? TimeSpan.Zero : (userTaskState == UserTaskInstanceStateId.Started ? DateTime.Now : CompletionTime) - StartTime;
    //public TimeSpan RemainingExecutionTime => userTaskState < UserTaskInstanceStateId.Started ? TimeSpan.Zero : (userTaskState == UserTaskInstanceStateId.Started ? StartTime - DateTime.Now + AllowedExecutionTime : TimeSpan.Zero);
    public override TimeSpan RemainingActiveTime => userTaskState < UserTaskInstanceStateId.Completed
        ? CreationTime - DateTime.Now + AllowedActiveTime
        : TimeSpan.Zero;
    //public double ExecutionPerformanceRatio => AllowedExecutionTime.Milliseconds > 0 ? ExecutionTime.Milliseconds * 100 / AllowedExecutionTime.Milliseconds : 0;

    public double WorkAllocationFactor;

    //		public ActivityInstance ai;
    public long ConfirmationStepId;

    /*
		public enum eTimeState
		{
			None, Normal, Critical, TimedOut, 
		}

		public eTimeState timeState { 
			get 
			{
				if (AllowedExecutionTime.Milliseconds == 0) return eTimeState.None;
				var rt = RemainingExecutionTime.Milliseconds;
				if (rt < 0) return eTimeState.TimedOut;
				if (rt < AllowedExecutionTime.Milliseconds / 4) return eTimeState.Critical;
				return eTimeState.Normal;
			} 
		}
		public DateTime DueTime 
		{ 
			get 
			{
				try
				{
					var dt1 = CreationTime + AllowedActiveTime;
					if (Started)
					{
						DateTime dt2 = StartTime + AllowedExecutionTime;
						return dt2 < dt1 ? dt2 : dt1;
					}
					return dt1;
				}
				catch(Exception)
				{
					return DateTime.Now;
				}

			} 
		}*/
    public bool Started => userTaskState == UserTaskInstanceStateId.Started;
    public bool IsNoResourceWork => userTaskState == UserTaskInstanceStateId.Created;

    public bool IsInputWork => userTaskState is UserTaskInstanceStateId.OfferedToASingleResource or
                               UserTaskInstanceStateId.OfferedToMultipleResources or
                               UserTaskInstanceStateId.AllocatedToASingleResource;

    public bool IsSuspendedWork => userTaskState == UserTaskInstanceStateId.Suspended;

    public enum eUserFunctions
    {
        System_Create = 1, //None to Created
        System_OfferSingle, //Created to OfferedToASingleResource
        System_OfferMultiple, //Created to OfferedToMultipleResources
        System_Allocate, //Created to AllocatedToASingleResource

        Admin_OfferSingle = 11, //Created to OfferedToASingleResource
        Admin_OfferMultiple, //Created to OfferedToMultipleResources
        Admin_Allocate, //Created to AllocatedToASingleResource
        Admin_WithdrawAndAllocate, //OfferedToMultipleResources to AllocatedToASingleResource

        Resource_Allocate = 21, //OfferedToASingleResource to AllocatedToASingleResource
        Resource_WithdrawAndAllocate, //OfferedToMultipleResources to AllocatedToASingleResource
        Resource_StartOffered, //OfferedToASingleResource to Started
        Resource_WithdrawAndStart, //OfferedToMultipleResources to Started
        Resource_StartAllocated, //AllocatedToASingleResource to Started

        Resource_Suspend = 31, //Started to Suspended
        Resource_Resume, //Suspended to Started
        Resource_Complete, //Started to Completed
        Resource_Fail //Started to Failed
    };

    public void OfferedToASingleResource(string userId, long userGroupId)
    {
        SetUserTaskState(UserTaskInstanceStateId.OfferedToASingleResource, userId, userGroupId);
    }

    public void AllocatedToASingleResource(string userId, long userGroupId)
    {
        SetUserTaskState(UserTaskInstanceStateId.AllocatedToASingleResource, userId, userGroupId);
    }

    public void SetUserTaskState(UserTaskInstanceStateId newState, string userId, long userGroupId)
    {
        userTaskState = newState;
        ActualOwnerId = userId;
        UserGroupId = userGroupId;
    }
}
