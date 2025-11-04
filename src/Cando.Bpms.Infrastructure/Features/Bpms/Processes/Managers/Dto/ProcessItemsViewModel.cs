namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class ProcessItemsViewModel
{
    public ProcessItemsViewModel(ElasticObject item)
    {
        ProcessId = item.GetString("ProcessId");
        Count = item.GetLong("Count");
        NotStartedCount = item.GetLong("NotStartedCount");
        ProcessName = ProjectDefinition.Project.GetBpmnDefinition(ProcessId,
            null)?.Process?.Name;
    }
    public string ProcessId { get; set; }
    public string ProcessName { get; set; }
    public long NotStartedCount { get; set; }
    public long Count { get; set; }
}
