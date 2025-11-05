using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class TaskSummaryViewModel
{
    public string ActivityName { get; set; }
    public string EntityDisplayText { get; set; }
    public string EntityId { get; set; }
    public string ActualOwnerId { get; set; }
    public long UserGroupId { get; set; }
    public string ActivityDescription { get; set; }
    public string StateDisplayText { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CloseTime { get; set; }
    public string CloseTimeToText => CloseTime != null ? CloseTime.Value.ToReadableText(ProjectDefinition.Project.DefaultCalendar) : "";

    public TimeSpan DoingDuration => CloseTime != null && StartTime != null ? CloseTime.Value - StartTime.Value : TimeSpan.MinValue;
    public TimeSpan WaitingDuration => StartTime != null ? StartTime.Value - CreationTime : TimeSpan.MinValue;
    public IList<ActivityInstanceRecord> PiActivities { private get; set; }

    public IEnumerable<WorkItemViewModel> ActiveTasks => PiActivities.Where(ai => !ai.Closed).Select(AiToWorkItem());
    public IEnumerable<WorkItemViewModel> ClosedTasks => PiActivities.Where(ai => ai.Closed).Select(AiToWorkItem());

    private Func<ActivityInstanceRecordDb, WorkItemViewModel> AiToWorkItem()
    {
        return ai =>
             new WorkItemViewModel
             {
                 CreationTime = ai.CreationTime,
                 CompletionTime = ai.CompletionTime,
                 ActualOwnerId = ai.ActualOwnerId,
                 UserGroupId = ai.UserGroupId,
                 Description = ai.Description,
                 //ActivityName = ai.FlowNodeId,
                 EntityId = EntityId,
                 ActivityInstanceId = ai.Id,
                 //ActivityType = (Activity.eActivityType)ai.ActivityInstanceTypeId,
                 //                TaskId = 
             };
    }
}
