using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.PartialModels.WorkItems;

public class WorkItemsFilterModel
{
    public WorkItemsFilter currentFilter { get; set; }
    public WorkItemsPageType pageType { get; set; }
    public List<FormDataRow> activityList { get; set; }
    public List<List<ProcessComboItem>> processList { get; set; }
}