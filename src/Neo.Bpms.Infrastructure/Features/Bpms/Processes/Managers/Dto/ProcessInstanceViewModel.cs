namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class ProcessInstanceViewModel
{
    public ProcessInstanceViewModel()
    {

    }

    public ProcessInstanceViewModel(ProcessInstance pi)
    {
        Id = pi.Id;
        ClosedTime = pi.CloseTime?.ToString("g") ?? "";
        State = pi.state.ToString();
        Entity = pi.EntityId;
        EntityPkv = pi.EntityPkv;
    }
    public long Id { get; set; }
    public string ClosedTime { get; set; }
    public string State { get; set; }

    public string Entity { get; set; }
    public string EntityPkv { get; set; }
    //        public  State { get; set; }
}
