namespace Neo.Bpms.Domain.Entities.ProcessModel;

[NotInMeta]
public abstract class BPMNOperationDb : BaseProcessModelStateBasedEntity
{
    [DisplayNameAndEnName("شناسه رابط")]
    public long InterfaceId;

    [MaxLength(256)]
    [DisplayNameAndEnName("OperationId")]
    public string OperationId;

    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    public string Name;
}

[States(typeof(StateBaseEntityId))]
public class BPMNOperation : BPMNOperationDb
{
    [DisplayNameAndEnName("شناسه رابط")]
    public BPMNInterface Interface;
}
