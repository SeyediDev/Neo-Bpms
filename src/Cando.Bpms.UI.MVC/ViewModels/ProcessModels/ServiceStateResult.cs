using Neo.Bpms.Domain.Entities.Bpmn.Execution;

namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class ServiceStateResult
{
    public long ActivityInstanceId { get; set; }
    public UserTaskInstanceStateId ServiceTaskState { get; set; }
    public int? Progress { get; set; }
}