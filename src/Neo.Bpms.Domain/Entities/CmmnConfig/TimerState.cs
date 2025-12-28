namespace Neo.Bpms.Domain.Entities.CmmnConfig;

[States(typeof(StateBaseEntityId))]
public class TimerState : BaseCmmnConfigStateBasedEntity 
{
    [Required]
    [MaxLength(80)]
    public string ProcessName { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; }

    [Required]
    [MaxLength(80)]
    public string MachineName { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public DateTime? ExceptionTime { get; set; }
    public string Exception { get; set; }

    [FAttr_IsFormulaAttribute("IF((StartTime>StopTime),true,false)")]
    public bool Running { get; set; }

    [FAttr_IsFormulaAttribute("IF((ExceptionTime>StartTime),true,false)")]
    public bool HasException { get; set; }
}
