namespace Neo.Bpms.Domain.Entities.WorkManagement;

public class TaskAddressing
{
    public string ProcessId { get; set; }
    public string ProcessVersion { get; set; }
    public string TaskId { get; set; }
    public long ProcessInstanceId { get; set; }
    public long ActivityInstanceId { get; set; }
}

public class FlowNodeAddressing
{
    public long FlowNodeId { get; set; }
    public long ProcessInstanceId { get; set; }
    public long ActivityInstanceId { get; set; }
}
