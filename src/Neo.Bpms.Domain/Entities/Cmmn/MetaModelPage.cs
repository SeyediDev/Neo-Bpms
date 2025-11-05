namespace Neo.Bpms.Domain.Entities.Cmmn;

[NotInMeta]
public abstract class MetaModelPageDb : BaseCmmnStateBasedEntity
{
    [DisplayNameAndEnName("شناسه موجودیت")]
    public long MetaEntityId;

    [DisplayNameAndEnName("شناسه نوع صفحه")]
    public MetaModelPageTypeId PageTypeId;

    [MaxLength(256)]
    [DisplayNameAndEnName("کد صفحه")]
    public string PageId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    public string Name;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("صفحه موجودیت")]
public class MetaModelPage : MetaModelPageDb
{
    [DisplayNameAndEnName("وضعیت")]
    public EntityStateName State;

    [DisplayNameAndEnName("موجودیت")]
    public MetaModelEntity MetaEntity;

    [DisplayNameAndEnName("نوع صفحه")]
    public MetaModelPageType PageType;
}
