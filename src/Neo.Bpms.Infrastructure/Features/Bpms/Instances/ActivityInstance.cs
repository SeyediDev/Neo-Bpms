using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public class ActivityInstance : FlowNodeInstance2
{
    public ActivityInstance(long instanceId, ActivityRuntime activityRuntime, ProcessInstance pi,
        ActivityInstanceStateId activityState = ActivityInstanceStateId.Ready,
        UserTaskInstanceStateId userTaskState = UserTaskInstanceStateId.Created)
        : base(instanceId, activityRuntime, pi, activityState)
    {
        this.userTaskState = userTaskState;
    }

    public ActivityInstance(ElasticObject activityInstanceRecord, ActivityRuntime activityRuntime, ProcessInstance pi)
        : base(activityInstanceRecord, activityRuntime, pi)
    {
        userTaskState = (UserTaskInstanceStateId)activityInstanceRecord.GetLong("userTaskStateId");
        AllowedExecutionTime = activityInstanceRecord.GetTimeSpan("AllowedExecutionTime");
        CompletionTime = activityInstanceRecord.GetNullableDateTime("CompletionTime");
        LoopCounter = activityInstanceRecord.GetLong("LoopCounter");
        ReceivedTokenCount = activityInstanceRecord.GetLong("ReceivedTokenCount");
        StartTime = activityInstanceRecord.GetNullableDateTime("StartTime");
        ActualOwnerId = activityInstanceRecord.GetString("ActualOwnerId");
        UserGroupId = activityInstanceRecord.GetLong("UserGroupId");
        AllowedActiveTime = activityInstanceRecord.GetTimeSpan("AllowedActiveTime");
        Description = activityInstanceRecord.GetString("Description");
    }

    public ActivityInstance(ActivityInstanceRecordDb activityInstanceRecord, ActivityRuntime activityRuntime,
        ProcessInstance pi)
        : base(activityInstanceRecord, activityRuntime, pi)
    {
        userTaskState = activityInstanceRecord.userTaskStateId;
        AllowedExecutionTime = activityInstanceRecord.AllowedExecutionTime;
        CompletionTime = activityInstanceRecord.CompletionTime;
        LoopCounter = activityInstanceRecord.LoopCounter;
        ReceivedTokenCount = activityInstanceRecord.ReceivedTokenCount;
        StartTime = activityInstanceRecord.StartTime;
        ActualOwnerId = activityInstanceRecord.ActualOwnerId;
        UserGroupId = activityInstanceRecord.UserGroupId;
        AllowedActiveTime = activityInstanceRecord.AllowedActiveTime;
    }

    public ActivityRuntime ActivityRuntime => (ActivityRuntime)FlowNodeRunTime;
    public string OperationName => ((IOperationContainer)FlowNodeRunTime.flowNode)?.operationRef?.Name;
    public new ActivityInstanceStateId state { get; private set; }
    public UserTaskInstanceStateId userTaskState { get; protected set; } //todo

    /// <summary>
    /// the “user” who picked/claimed the User task and became the actual owner of it. 
    /// The value is a literal representing the user’s id, email address etc.
    /// </summary>
    public string ActualOwnerId { get; set; }
    public long UserGroupId { get; set; }

    public string MachineId { get; set; }

    public DateTime? StartTime;
    public DateTime? CompletionTime;

    public TimeSpan AllowedExecutionTime;

    /*
		/// <summary>
		/// for Multi-instance Activity instance
		/// 
		/// This attribute is provided for the outer instance of the Multi-Instance Activity only. This attribute contains the total number of 
		/// inner instances created for the Multi-Instance Activity.
		/// The sum of numberOfTerminatedInstances, numberOfCompletedInstances, and numberOfActiveInstances always sums up to numberOfInstances.
		/// </summary>
		public long numberOfInstances
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// for Multi-instance Activity instance
		/// 
		/// This attribute is provided for the outer instance of the Multi-Instance Activity only. This attribute contains the number of
		/// currently active inner instances for the Multi-Instance Activity. In case of a sequential Multi-Instance Activity, this value can’t be
		/// greater than 1. For parallel Multi-Instance Activities, this value can’t be greater than the value contained in numberOfInstances
		/// </summary>
		public long numberOfActiveInstances
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// for Multi-instance Activity instance
		/// 
		/// This attribute is provided for the outer instance of the Multi-Instance Activity only. This attribute contains the number of already completed 
		/// inner instances for the Multi-Instance Activity.
		/// </summary>
		public long numberOfCompletedInstances
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// for Multi-instance Activity instance
		/// 
		/// This attribute is provided for the outer instance of the Multi-Instance Activity only. This attribute contains the number of terminated inner instances for the Multi-Instance Activity.
		/// </summary>
		public long numberOfTerminatedInstances
		{
			get { throw new NotImplementedException(); }
		}
		*/
    /// <summary>
    /// for loop activity instances and Multi-instance Activity instance
    /// 
    /// This attribute is provided for each generated (inner) instance of the Activity. It contains the sequence number of the generated instance, 
    /// i.e., if this value of some instance in n, the instance is the n-th instance that was generated.
    /// </summary>
    public long LoopCounter { get; set; }

    /// <summary>
    /// for 
    /// </summary>
    public long ReceivedTokenCount { get; set; }

    public Activity activity => (Activity)FlowNode;
    public string? Description { get; set; }

    /*
		public override TimeSpan ActiveTime => state == ActivityInstanceStateId.Ready
			? TimeSpan.Zero
			: (state == ActivityInstanceStateId.Active ? DateTime.UtcNow : CompletionTime) - StartTime;

		public override TimeSpan RemainingActiveTime => state == ActivityInstanceStateId.Active
			? StartTime - DateTime.UtcNow + AllowedExecutionTime
			: TimeSpan.Zero;

		public double ActivePerformanceRatio => AllowedActiveTime.Milliseconds > 0
			? ActiveTime.Milliseconds * 100 / AllowedActiveTime.Milliseconds
			: 0;
			*/

    protected override void SetState(object instanceState)
    {
        state = (ActivityInstanceStateId)Convert.ToInt16(instanceState);
    }

    protected override Property GetProperty(string name)
    {
        return GetProperty(activity, name);
    }

    internal bool IsCorrelated(LocalParameters content)
    {
        return true;
    }

    public bool IsExecuting()
    {
        return state switch
        {
            //case ActivityInstance.eActivityStates.Ready:
            //case ActivityInstance.eActivityStates.Active:
            //case ActivityInstance.eActivityStates.Completing:
            //case ActivityInstance.eActivityStates.Failing:
            //case ActivityInstance.eActivityStates.Terminating:
            //case ActivityInstance.eActivityStates.Compensating:
            //	break;
            ActivityInstanceStateId.Completed or ActivityInstanceStateId.Withdrawn or ActivityInstanceStateId.Failed or ActivityInstanceStateId.Terminated or ActivityInstanceStateId.Compensated => false,
            _ => true,
        };
    }

    private void Clean()
    {
        CompletionTime = DateTime.MinValue;
        CloseTime = DateTime.MinValue;
        Closed = false;
    }

    public void AllocateToASingleResource()
    {
        userTaskState = UserTaskInstanceStateId.AllocatedToASingleResource;
    }

    public void Born()
    {
        userTaskState = UserTaskInstanceStateId.Created;
        state = ActivityInstanceStateId.Ready;
    }

    public void ReBorn()
    {
        LogTrace($"ReBorn {Id}");
        Born();
        Clean();
        Free();
    }

    public void Start()
    {
        userTaskState = UserTaskInstanceStateId.Started;
        StartTime = DateTime.UtcNow;
        Clean();
    }

    public void Terminating()
    {
        CheckStartTime();
        state = ActivityInstanceStateId.Terminating;
    }

    public void Failing()
    {
        CheckStartTime();
        state = ActivityInstanceStateId.Failing;
    }

    public void Withdrawn()
    {
        CheckStartTime();
        state = ActivityInstanceStateId.Withdrawn;
        userTaskState = UserTaskInstanceStateId.Withdrawn;
        Close();
    }

    internal override void Complete()
    {
        CheckStartTime();
        userTaskState = UserTaskInstanceStateId.Completed;
        state = ActivityInstanceStateId.Completed;
        CompletionTime = DateTime.UtcNow;
        Close();
    }

    internal override void Close()
    {
        LogTrace($"ai close {Id}");
        base.Close();
    }

    public void Fail()
    {
        userTaskState = UserTaskInstanceStateId.Failed;
        state = ActivityInstanceStateId.Failed;
        CompletionTime = DateTime.UtcNow; //todo correct??
        Close();
    }

    internal override void Activate()
    {
        state = ActivityInstanceStateId.Active;
        Clean();
    }

    internal override void Cancel()
    {
        CheckStartTime();
        userTaskState = UserTaskInstanceStateId.Failed; //todo
        state = ActivityInstanceStateId.Terminated;
        Close();
    }

    internal void Compensation()
    {
        CheckStartTime();
        state = ActivityInstanceStateId.Compensated;
        Close();
        //todo
    }

    private void CheckStartTime()
    {
        if (StartTime == null)
        {
            StartTime = DateTime.UtcNow;
        }
    }

    public void Init(long loopCounter)
    {
        ActualOwnerId = AuditTrail.User?.Id; //todo
        CreationTime = DateTime.UtcNow;
        LoopCounter = loopCounter;
        Born();
    }
}
