namespace Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;

public class EntityChangedReporterConfig
{
    public EntityAddress EntityAddress { get; set; }
    public bool CallCaller { get; set; }
    public string CallerName { get; set; }
}
