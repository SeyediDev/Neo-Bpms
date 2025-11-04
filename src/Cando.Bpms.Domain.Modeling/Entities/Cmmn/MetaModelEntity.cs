namespace Neo.Bpms.Domain.Modeling.Entities.Cmmn;

[NotInMeta]
public abstract class MetaModelEntityDb : BaseCmmnStateBasedEntity
{
    [DisplayNameAndEnName("شناسه فضای نامی")]
    public long MetaNamespaceId;

    [MaxLength(256)]
    [DisplayNameAndEnName("کد موجودیت")]
    public string MetaEntityId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    [InDisplayString]
    public string Name;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("موجویت")]
public class MetaModelEntity : MetaModelEntityDb
{
    [DisplayNameAndEnName("فضای نامی")]
    public MetaModelNamespace MetaNamespace;
}
