namespace Neo.Bpms.Domain.Modeling.Entities.ProcessModel;

[NotInMeta]
public abstract class BPMNProcessVersionDb : BaseProcessModelStateBasedEntity
{
    [DisplayNameAndEnName("شناسه فرآیند")]
    public long ProcessId;

    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("نسخه فرآیند")]
    public string Version;

    [DisplayNameAndEnName("شناسه موجودیت")]
    public long? MetaEntityId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    public string Name;

    [MaxLength(41)]
    public string CheckInUserId;

    public bool AdministratorLock;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("نسخه فرآیند")]
public class BPMNProcessVersion : BPMNProcessVersionDb
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("فرآیند")]
    public BPMNProcess Process;

    [DisplayNameAndEnName("موجودیت")]
    public MetaModelEntity MetaEntity;

    [InDisplayString]
    [Formula("Process.Name")]
    [DisplayNameAndEnName("نام فرآیند")]
    public string ProcessName => Process?.Name;

    [Formula("MetaEntity.MetaNamespace.MetaNamespaceId")]
    [MaxLength(256)]
    [DisplayNameAndEnName("کد فضای نامی")]
    public string ProcessNamespaceId => MetaEntity?.MetaNamespace?.MetaNamespaceId;

    [Formula("MetaEntity.MetaEntityId")]
    [MaxLength(256)]
    [DisplayNameAndEnName("کد موجودیت")]
    public string ProcessEntityId => MetaEntity?.MetaEntityId;
}
