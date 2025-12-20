using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.WorkManagement;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class WorkItemsViewModel(IList<WorkItemViewModel> workItems, WorkItemsFilter cartableFilter,
    ElasticObject entityFilter, long recordsCount)
{
    public IList<WorkItemViewModel> WorkItems { get; } = workItems;
    public WorkItemsFilter CartableFilter { get; } = cartableFilter;
    public ElasticObject EntityFilter { get; } = entityFilter;
    public long RecordsCount { get; } = recordsCount;
    //todo instead of this where clause check for microservices
    public IEnumerable<TaskAddressing> ServiceTasks =>
        WorkItems.Where(m => m.ActivityType == Activity.eActivityType.ServiceTask &&
                             m.UserTaskState <= UserTaskInstanceStateId.Started)
                 .Select(m => new TaskAddressing
                 {
                     ActivityInstanceId = m.ActivityInstanceId,
                     ProcessId = m.ProcessId,
                     ProcessInstanceId = m.ProcessInstanceId,
                     ProcessVersion = m.ProcessVersion
                 });
    public string AlreadySelectedsJson { get; set; }

}

public class WorkItemHistoryViewModel(WorkItemViewModel workItemViewModel)
{
    private readonly WorkItemViewModel _workItemViewModel = workItemViewModel;

    public string ActivityName => _workItemViewModel.ActivityName;
    public string ActualOwnerName => _workItemViewModel.ActualOwnerName;
    public string UserGroupName => _workItemViewModel.UserGroupName;
    public string CompletionTime => _workItemViewModel.ReadableCompletionDateTime;
    public string CreationTime => _workItemViewModel.ReadableCreationDateTime;
    public string? Description => _workItemViewModel.Description;
    public string ProcessName => _workItemViewModel.ProcessName;
    public string UserTaskState => _workItemViewModel.ReadableUserTaskState;
}
