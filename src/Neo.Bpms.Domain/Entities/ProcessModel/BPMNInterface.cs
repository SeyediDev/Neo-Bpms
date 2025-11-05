namespace Neo.Bpms.Domain.Entities.ProcessModel;

[States(typeof(StateBaseEntityId))]
public class BPMNInterface : BaseProcessModelStateBasedEntity
{
    [MaxLength(256)]
    [DisplayNameAndEnName("InterfaceId")]
    public string InterfaceId;

    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    public string Name;
}
