namespace Neo.Bpms.Domain.Entities.ProcessModel;

[NotInMeta]
public abstract class BPMNProcessDb : BaseProcessModelStateBasedEntity
{
    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("کد فرآیند")]
    public string ProcessId;

    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("نام فرآیند")]
    public string Name;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("فرآیند")]
public class BPMNProcess : BPMNProcessDb
{
    [DisplayNameAndEnName("وضعیت")]
    public EntityStateName State;
}
