using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.DataSynchronization;

public class EntityChangedReporterConfig
{
    public EntityAddress EntityAddress { get; set; }
    public bool CallCaller { get; set; }
    public string CallerName { get; set; }
}
