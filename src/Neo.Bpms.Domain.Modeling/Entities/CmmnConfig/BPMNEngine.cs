namespace Neo.Bpms.Domain.Modeling.Entities.CmmnConfig;

[DisplayNameAndEnName("موتور جریان کار")]
public class BPMNEngine: BaseCmmnConfigEntity
{
    [MaxLength(80)]
    public string ProcessName;

    [MaxLength(80)]
    public string MachineName;

    public DateTime? StartTime;
    public DateTime? StopTime;
}
