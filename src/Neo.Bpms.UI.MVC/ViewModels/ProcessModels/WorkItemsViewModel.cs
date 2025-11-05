using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.WorkManagement;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class WorkItemsViewModel
{
    public WorkItemsViewModel(IList<WorkItemViewModel> workItems, WorkItemsFilter cartableFilter,
        ElasticObject entityFilter, long recordsCount)
    {
        WorkItems = workItems;
        CartableFilter = cartableFilter;
        EntityFilter = entityFilter;
        RecordsCount = recordsCount;
    }

    public IList<WorkItemViewModel> WorkItems { get; }
    public WorkItemsFilter CartableFilter { get; }
    public ElasticObject EntityFilter { get; }
    public long RecordsCount { get; }
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

public class WorkItemHistoryViewModel
{
    private readonly WorkItemViewModel _workItemViewModel;

    public WorkItemHistoryViewModel(WorkItemViewModel workItemViewModel)
    {
        _workItemViewModel = workItemViewModel;
    }

    public string ActivityName => _workItemViewModel.ActivityName;
    public string ActualOwnerName => _workItemViewModel.ActualOwnerName;
    public string UserGroupName => _workItemViewModel.UserGroupName;
    public string CompletionTime => _workItemViewModel.ReadableCompletionDateTime;
    public string CreationTime => _workItemViewModel.ReadableCreationDateTime;
    public string? Description => _workItemViewModel.Description;
    public string ProcessName => _workItemViewModel.ProcessName;
    public string UserTaskState => _workItemViewModel.ReadableUserTaskState;
}
