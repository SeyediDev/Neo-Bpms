namespace Neo.Bpms.Domain.Entities.CmmnConfig;

public class TimerStateLog : BaseCmmnConfigEntity
{
    [Required]
    public DateTime EventDate;

    [Required]
    [MaxLength(80)]
    public string Name;

    [Required]
    [MaxLength(80)]
    public string ProcessName;

    [Required]
    [MaxLength(80)]
    public string MachineName;

    public DateTime? StartTime;
    public DateTime? StopTime;
    public DateTime? ExceptionTime;
    public string Exception;
}
