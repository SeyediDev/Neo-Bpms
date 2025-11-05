using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class WorkItemViewModel
{
    public WorkItemViewModel()
    {
    }

    public WorkItemViewModel(ElasticObject record)
    {
        ProcessId = record.GetString("__WorkflowId");
        ProcessVersion = record.GetString("__WFVersion");
        ActivityInstanceId = record.GetLong("__ActivityInstanceId");
        ProcessInstanceId = record.GetLong("__ProcessInstanceId");
        TaskId = record.GetString("__FlowNodeId");
        ActivityName = "";
        ActualOwnerId = record.GetString("__ActualOwnerId");
        UserGroupId = record.GetLong("__UserGroupId");
        CreationTime = record.GetDateTime("__CreationTime");
        StartTime = record.GetNullableDateTime("__StartTime");
        CompletionTime = record.GetNullableDateTime("__CompletionTime");
        UserTaskState = (UserTaskInstanceStateId)record.GetLong("__userTaskStateId");
        Description = record.GetString("__Description");

        EntityPkv = record.GetString("__EntityPKV");
        ActualOwnerName = record.GetString("__UserName");
        UserGroupName = record.GetString("__UserGroupName");
        Record = record;
        Forms = [];
    }

    public long ProcessInstanceId { get; set; }
    public long ActivityInstanceId { get; set; }
    public UserTaskInstanceStateId UserTaskState { get; set; }
    public string ProcessId { get; set; }
    public string ProcessVersion { get; set; }
    public string TaskId { get; set; }
    public string ProcessName { get; set; }
    public string ActivityName { get; set; }
    public string? Description { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public string ActualOwnerId { get; set; }
    public long UserGroupId { get; set; }
    public string ActualOwnerName { get; set; }
    public string UserGroupName { get; set; }
    public List<WorkItemFormInfo> Forms { get; set; }
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string EntityPkv { get; set; }
    public string EntityDescription { get; set; }
    public FlowNodeTypeId? FlowNodeType { get; set; }
    public Activity.eActivityType? ActivityType { get; set; }
    public Event.eEventType? EventType { get; set; }
    public string DisplayFields { get; set; }
    public ElasticObject Record { get; set; }


    public string WorkItemClass
    {
        get
        {
            switch (UserTaskState)
            {
                case UserTaskInstanceStateId.None:
                    if (EventType != null || FlowNodeType == FlowNodeTypeId.ParallelGateway)
                        return "";
                    return "danger";
                case UserTaskInstanceStateId.Created:
                    if (ActivityType == Activity.eActivityType.CallActivitySubProcess)
                        return "info";
                    else if (ActivityType == Activity.eActivityType.UserTask)
                        return "warning";
                    break;
                case UserTaskInstanceStateId.OfferedToASingleResource:
                    break;
                case UserTaskInstanceStateId.OfferedToMultipleResources:
                    break;
                case UserTaskInstanceStateId.AllocatedToASingleResource:
                    break;
                case UserTaskInstanceStateId.Started:
                    return "info";
                case UserTaskInstanceStateId.Suspended:
                    return "warning";
                case UserTaskInstanceStateId.Completed:
                    return "success";
                case UserTaskInstanceStateId.Failed:
                    return "danger";
                default:
                    return "danger";
            }

            return string.Empty;
        }
    }

    public string ReadableUserTaskState
    {
        get
        {
            switch (UserTaskState)
            {
                case UserTaskInstanceStateId.None:
                    if (EventType != null)
                    {
                        return EventType switch
                        {
                            Event.eEventType.Message => "منتظرِ پیام",
                            Event.eEventType.Signal => "منتظرِ سیگنال",
                            Event.eEventType.Timer => "منتظرِ تایمر",
                            Event.eEventType.Condition => "منتظرِ شرط",
                            _ => "منتظرِ رویداد",
                        };
                    }
                    if (FlowNodeType != null)
                    {
                        switch (FlowNodeType)
                        {
                            case FlowNodeTypeId.ParallelGateway:
                                return "منتظر مسیرهای موازی";
                        }
                    }
                    return "danger";
                case UserTaskInstanceStateId.Created:
                    if (ActivityType == Activity.eActivityType.CallActivitySubProcess)
                        return "در جریان";
                    else return ActivityType == Activity.eActivityType.UserTask ? "ناموفق در تخصیص" : "در صفِ انتظار";
                case UserTaskInstanceStateId.OfferedToASingleResource:
                    return "پیشنهاد شده به فرد";
                case UserTaskInstanceStateId.OfferedToMultipleResources:
                    return "پیشنهاد شده به گروه";
                case UserTaskInstanceStateId.AllocatedToASingleResource:
                    return "تخصیص یافته";
                case UserTaskInstanceStateId.Started:
                    return "شروع شده";
                case UserTaskInstanceStateId.Suspended:
                    return "تعلیق شده";
                case UserTaskInstanceStateId.Completed:
                    return "تکمیل شده";
                case UserTaskInstanceStateId.Failed:
                    return "ناموفق";
                default:
                    return "-";
            }
        }
    }

    public string ReadableCreationDateTime => CreationTime.ToReadableText(ProjectDefinition.Project.DefaultCalendar);
    public string ReadableCreationDate => CreationTime.ToReadableText(ProjectDefinition.Project.DefaultCalendar, ReadablePrecision.Date);
    public string ReadableCompletionDateTime => CompletionTime?.ToReadableText(ProjectDefinition.Project.DefaultCalendar);

    public string GetAttributes()
    {
        string workItemClass = "selectable-table-row table-" + WorkItemClass;
        return $@"title=""{Description}"" id=""ai-row-{ActivityInstanceId}"" data-processid=""{ProcessId}""
			data-activityid=""{TaskId}"" data-piid=""{ProcessInstanceId}"" data-version=""{ProcessVersion}""
			data-aiid=""{ActivityInstanceId}""
			class=""{workItemClass}""";
    }
}

public enum WorkItemsQueryType
{
    MyWorkItems = 1,
    Process = 2,
    MyProcess = 3,
}

public enum WorkItemsPageType
{
    MyWorkItems = 1,
    WorkItems
}

public class WorkItemFormInfo
{
    public Form.eFormType FormType { get; set; }
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string FormId { get; set; }
    public string FormSubjectId { get; set; }
    public string Name { get; set; }
    public string TaskId { get; set; }
}
